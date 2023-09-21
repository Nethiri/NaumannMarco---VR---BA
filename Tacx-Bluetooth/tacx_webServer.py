import asyncio
import json
import bleak
import time
from aiohttp import web
from pycycling.tacx_trainer_control import TacxTrainerControl
from datetime import datetime

PORT = 8057
BASIC_RESISTANCE = 0
TACX_DEVICE_NAME = "Tacx Neo 2 28482"
DATAPACKAGE = {
    "undefined"
}

def package_handler(data):
    global DATAPACKAGE
    DATAPACKAGE = {
        "last_update": time.time(),
        "elapsed_time": data.elapsed_time,
        "distance_travelled": data.distance_travelled,
        "speed": data.speed,
        "basic_resistance":  BASIC_RESISTANCE
    }

async def tacx_connector(address):
    if address is None:
        print(f"Can't connect to address: {address}")
        return None

    try:
        client = bleak.BleakClient(address)
    #async with bleak.BleakClient(address) as client:
        print(f"Trying to connect to Tacx under address {address}")
        while not client.is_connected:
            await client.connect()
            print("Still trying to connect")
            await asyncio.sleep(1)
        print(f"Tacx device has been connected to under address {address}")
        trainer = TacxTrainerControl(client=client)
        trainer.set_general_fe_data_page_handler(package_handler)
        await trainer.set_basic_resistance(BASIC_RESISTANCE)
        await trainer.enable_fec_notifications()
        #await asyncio.sleep(10)

        return trainer
    except Exception as ex:
        print("ERROR!")
        print(ex)
        return None


async def find_device(name):
    print(f"Scanning for BLE device with name {name}...")
    devices = await bleak.BleakScanner.discover()
    address = None

    for device in devices:
        if device.name == name:
            address = device.address
            break

    if address:
        print(f"Found device {name} with address {address}")
        return address
    else:
        print(f"Device {name} not found")
        return None
    
async def set_resistance(request, trainer):
    print("Trying to do post request")
    print(request)
    try:
        if trainer is None:
            return web.Response(text="Trainer is not set up correctly", content_type="text/plain", status=400)
        data = await request.json()
        resistance = 0
        resistance = int(data.get('resistance'))
        if resistance is not None:
            resistance = int(resistance)  # Convert to int
            # todo set tacx trainer resistance
            if 0 <= resistance < 201:
                global BASIC_RESISTANCE 
                BASIC_RESISTANCE = resistance
                await trainer.set_basic_resistance(BASIC_RESISTANCE)
                return web.Response(text=f"Resistance set to: {resistance}", content_type="text/plain", status=200)
            return web.Response(text=f"Resistance failed to set to: {resistance}, out of bounds", content_type="text/plain", status=406)
        else:
            return web.Response(text="Invalid resistance value", content_type="text/plain", status=400)
    except Exception as ex:
        print(ex)
        return web.Response(text=f"Error: {str(ex)}", content_type="text/plain", status=500)


async def serve_get_request(request):
    #print("Received a GET request.")
    return web.Response(text=json.dumps(DATAPACKAGE), content_type="application/json")

async def keepAlliveEvent():
    while True:
        await asyncio.sleep(0)

async def main():
    address = await find_device(TACX_DEVICE_NAME)
    trainer = await tacx_connector(address=address)
    
    if(trainer == None):
        print("Couldnt build up server strucutre, see errors above")
        return

    app = web.Application()
    app.router.add_get('/', serve_get_request)
    app.router.add_post('/set_resistance', lambda request: set_resistance(request, trainer))
    
    runner = web.AppRunner(app)
    await runner.setup()
    
    site = web.TCPSite(runner, 'localhost', PORT)
    await site.start()
    
    print(f"HTTP server started on port {PORT}")

    #Create a task for the keepAlliveEvent coroutine
    keepAllive = asyncio.create_task(keepAlliveEvent())

    # Wait for the HTTP server to finish (you can add other tasks if needed)
    await asyncio.gather(keepAllive)


    
if __name__ == "__main__":
    asyncio.run(main())