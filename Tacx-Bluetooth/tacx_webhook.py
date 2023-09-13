import asyncio
import websockets
from datetime import datetime
from bleak import BleakClient
from pycycling.tacx_trainer_control import TacxTrainerControl
import pygatt

PORT = 8057
TACX_DEVICE_NAME = "Tacx Neo 2 28482"

def scan_for_device(device_name, scan_duration=10):
    adapter = pygatt.GATTToolBackend()

    try:
        adapter.start()
        print(f"Scanning for BLE device with name {device_name}...")
        devices = adapter.scan(run_as_root=True, timeout=scan_duration)
        for device in devices:
            if device["name"] == device_name:
                _tacx_blth_address = device['address']
                print(f"Found device {device_name} with adress {_tacx_blth_address}")
                return _tacx_blth_address
        print(f"Device {device_name} not found")
    finally:
        adapter.stop()

# List of connections
connected_clients = set()

async def handle_connections(websocket, path):
    # Add a new client to the set of connected WebSocket clients
    connected_clients.add(websocket)
    print("A client has connected")
    try:
        async for message in websocket:
            pass
    finally:
        # Remove the client when the WebSocket connection is closed
        print("A client has disconected")
        connected_clients.remove(websocket)

async def send_update(data):
    # Send update to all connected WebSocket clients
    if connected_clients:
        for client in connected_clients:
            await client.send(str(data))
            #print("Send some Data!")


def package_handler(data):
    asyncio.create_task(send_update(data))
    #print(f"Tried sending package: {data}")

async def tacx_connector(address):
    try:
        async with BleakClient(address) as client:
            print(f"Trying to connect to Tacx under adress {address}") 
            while(client.is_connected == False):
                print("Still trying to connect")
            print(f"Tax device has been contected to under adress {address}")

            trainer = TacxTrainerControl(client=client)
            trainer.set_general_fe_data_page_handler(package_handler)
            #trainer.set_specific_trainer_data_page_handler(package_handler)
            await trainer.enable_fec_notifications()
            await trainer.set_basic_resistance(0)
            stop_event = asyncio.Event()
            async def send_packages(): 
                while not stop_event.is_set():
                    await asyncio.sleep(0)
            package_sender_task = asyncio.create_task(send_packages())
            await package_sender_task
            await trainer.disable_fec_notifications()
    except Exception as ex: 
        print("ERROR!")
        print(ex)
        return
    finally:
        return trainer


async def main():
    try:
        address = scan_for_device(TACX_DEVICE_NAME, 10)
        server = await websockets.serve(handle_connections, "localhost", PORT)
        tacx = await tacx_connector(address)
        
        await server.wait_closed()
    except Exception as ex:
        print("ERROR!")
        print(ex)
        return
    



if __name__ == "__main__":
    asyncio.run(main())