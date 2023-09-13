import asyncio
import websockets
from pycycling.tacx_trainer_control import TacxTrainerControl
import bleak

PORT = 8057
TACX_DEVICE_NAME = "Tacx Neo 2 28482"

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
        print("A client has disconnected")
        connected_clients.remove(websocket)

async def send_update(data):
    # Send update to all connected WebSocket clients
    if connected_clients:
        for client in connected_clients:
            await client.send(str(data))
            # print("Send some Data!")

def package_handler(data):
    asyncio.create_task(send_update(data))
    # print(f"Tried sending package: {data}")

async def tacx_connector(address):
    try:
        async with bleak.BleakClient(address) as client:
            print(f"Trying to connect to Tacx under address {address}")
            while not client.is_connected:
                print("Still trying to connect")
            print(f"Tacx device has been connected to under address {address}")

            trainer = TacxTrainerControl(client=client)
            trainer.set_general_fe_data_page_handler(package_handler)
            # trainer.set_specific_trainer_data_page_handler(package_handler)
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
        address = await bleak.discover()
        address = address.address
        server = await websockets.serve(handle_connections, "localhost", PORT)
        tacx = await tacx_connector(address)

        await server.wait_closed()
    except Exception as ex:
        print("ERROR!")
        print(ex)
        return

if __name__ == "__main__":
    asyncio.run(main())