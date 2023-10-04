import asyncio
import json
import bleak
import time
from pycycling.tacx_trainer_control import TacxTrainerControl
from pycycling.sterzo import Sterzo
import os
from datetime import datetime
from aiohttp import web

#Trainer Devices:
#Tax Trainer
tacxStatus = True
#Elite Stero
eliteStatus = True
#Break Arduino
breakStatus = False

#Tax and Elite are connected via Bluetooth
#their respective clear names (device names) are stored and used in global
TACX_DEVICE_NAME = "Tacx Neo 2 28482"
ELITE_DEVICE_NAME = "STERZO"

#Global variable Datapackage
#this variable is shipped on request and edited live
DATAPACKAGE = {
    "tacx_last_update": None,
    "tacx_elapsed_time": None,
    "tacx_distance_travelled": None,
    "tacx_speed": None,
    "tacx_basic_resistance":  None,
    "elite_angle": None,
    "elite_last_update": None,
    "break_front": None,
    "break_back": None
}

#Website
PORT = 8057

#function that is called every time the tacx sends a notification to the server
def tacx_package_handler(data):
    global DATAPACKAGE
    DATAPACKAGE["tacx_last_update"] = time.time()
    DATAPACKAGE["tacx_elapsed_time"] = data.elapsed_time
    DATAPACKAGE["tacx_distance_travelled"] = data.distance_travelled
    DATAPACKAGE["tacx_speed"] = data.speed
    #print(data)

#function that sis called every time the elite sends a notification to the server
def elite_package_handler(data):
    global DATAPACKAGE
    DATAPACKAGE["elite_angle"] = data
    DATAPACKAGE["elite_last_update"] = time.time()
    #print(data)


#Function to Resolve a device name to an appropriate address
async def find_device(name):
    print(f"Scanning for BLE device with name {name}...")
    devices = await bleak.BleakScanner.discover()
    #print(devices)
    address = None

    for device in devices:
        #print(device.name)
        if device.name == name:
            address = device.address
            break

    if address:
        print(f"Found device {name} with address {address}")
        return address
    else:
        print(f"Device {name} not found")
        return None

#=== TACX ===
#Function to connect to the Tacx trainer
async def tacx_connect(address=None):
    #check if address is existent
    if address is None:
        address = await find_device(TACX_DEVICE_NAME)
    
    try:
        client = bleak.BleakClient(address)
        await client.connect()
        if client.is_connected == False:
            print(f"Connection to address {address} was not established, trying again...")
            client = await tacx_connect(address)
            return client
        print(f"Tacx device has been connected under address {address}")
        return client
    
    except Exception as ex:
        print("ERROR!")
        print(ex)
        return False
    
async def tacx_get_trainer(client: bleak.BleakClient):
    if client.is_connected == False:
        print(f"Lost connection to Tacx device somewhere... trying to resolve")
        connected_client = await tacx_connect()
        if connected_client.is_connected == False:
            print("Could not resolve Tacx again!!! Program dies from here...")
            return False 
        trainer = TacxTrainerControl(client=connected_client)
        return trainer
    else:
        trainer = TacxTrainerControl(client=client)
        return trainer

async def tacx_set_resistance(resistance, client):
    if 0 <= resistance <= 200:
        try:
            trainer: TacxTrainerControl = await tacx_get_trainer(client)
            await trainer.set_basic_resistance(resistance=resistance)
            DATAPACKAGE["tacx_basic_resistance"] = resistance
            return True
        except Exception as ex:
            print("ERROR WHILE TRYING TO SET RESISTANCE!")
            print(ex)
            return False
    else:
        print(f"Resistance: {resistance} is outside of acceptable values 0-200")
        return False
    
async def tacx_set_data_page_handler(client):
    trainer: TacxTrainerControl = await tacx_get_trainer(client)
    if trainer is not None:
        trainer.set_general_fe_data_page_handler(tacx_package_handler)
        await trainer.enable_fec_notifications()
        return True
    else:
        print("Trainer is not available.")
        return False

async def tacx_disable_data_package_hanlder(client):
    trainer: TacxTrainerControl = await tacx_get_trainer(client)
    await trainer.disable_fec_notifications()

#==ELITE===
async def elite_connect(address=None): 
    #check if address is existent
    if address is None:
        address = await find_device(ELITE_DEVICE_NAME)

    try:
        client = bleak.BleakClient(address)
        await client.connect()
        if client.is_connected == False:
            print(f"Connection to address {address} was not established, trying again...")
            client = await tacx_connect(address)
            return client
        print(f"Elite device has been connected under address {address}")
        return client
    
    except Exception as ex:
        print("ERROR!")
        print(ex)
        return False
    
async def elite_get_sterzo(client: bleak.BleakClient):
    if client.is_connected == False:
        print(f"Lost connection to Elte device somewhere... trying to resolve")
        connected_client = await elite_connect()
        if connected_client.is_connected == False: 
            print("Could not resolve Elite again!!! Program dies from here...")
            return False
        sterzo = Sterzo(client)
        return sterzo
    else: 
        sterzo = Sterzo(client)
        return sterzo

async def elite_set_data_page_handler(client):
    sterzo: Sterzo = await elite_get_sterzo(client)
    if sterzo is not None:
        sterzo.set_steering_measurement_callback(elite_package_handler)
        await sterzo.enable_steering_measurement_notifications()
        return True
    else:
        print("Sterzo is not available.")
        return False
    
async def elite_disable_data_package_handler(client):
    sterzo: Sterzo = await elite_get_sterzo(client)
    await sterzo.disable_steering_measurement_notifications()

async def keepAllive_async_input(prompt):
    loop = asyncio.get_event_loop()
    return await loop.run_in_executor(None, input, prompt)

async def keepAlliveEvent():
    while True:
        await asyncio.sleep(0)

        # Check if "C" is pressed
        user_input = await keepAllive_async_input("Press 'C' to stop the program: ")
        if user_input.lower() == 'c':
            break

#===WEBSERVER===
#http://localhost:8057/
#http://localhost:8057/set_resistance
async def serve_get_request(request):
    return web.Response(text=json.dumps(DATAPACKAGE), content_type="application/json", status=200)

async def set_resistance(request, trainer_client):
    if(tacxStatus == False):
        return web.Response(text=f"Error: Tacx device is not setup! tacxStatus=false", status=400)
    
    try:
        if trainer_client is None:
            return web.Response(text=f"Error: trainer_client is None!", status=400)
        data = await request.json()
        resistance = 0
        resistance = int(data.get('resistance'))
        if resistance is not None:
            resistance = int(resistance)
            if tacx_set_resistance(resistance=resistance, client=trainer_client) == False:
                return web.Response(text=f"Value could not be set! Only accepting 0-200! Or check console for other error!", status=400)
            return web.Response(text=f"Resistance set to: {resistance}", content_type="text/plain", status=200)
        else:
            return web.Response(text=f"resistance value was None!", status=400)
    except Exception as ex:
        print(ex)
        return web.Response(text=f"Error: {str(ex)}", content_type="text/plain", status=400)

async def main():
    tacx_client = None
    elite_client = None
    #Start
    if tacxStatus == True:
        tacx_client = await tacx_connect()
        await tacx_set_data_page_handler(tacx_client)
        await tacx_set_resistance(client=tacx_client, resistance=10)
    
    if eliteStatus == True:
        elite_client = await elite_connect()
        await elite_set_data_page_handler(elite_client)
    
    #WebServer
    app = web.Application()
    app.router.add_get('/', serve_get_request)
    app.router.add_post('/set_resistance', lambda request: set_resistance(request, tacx_client))
    runner = web.AppRunner(app)
    await runner.setup()
    site = web.TCPSite(runner, 'localhost', PORT)
    await site.start()
    print(f"HTTP server started up on port {PORT}")

    #Create a task for the keepAlliveEvent coroutine
    keepAllive = asyncio.create_task(keepAlliveEvent())
    await asyncio.gather(keepAllive)

    #what happens after the keepallive has been ended... eg programm shuts down
    if tacxStatus == True:
        await tacx_disable_data_package_hanlder(tacx_client)
        await tacx_set_resistance(client=tacx_client, resistance=0)
        await tacx_client.disconnect()
    
    if eliteStatus == True:
        await elite_disable_data_package_handler(elite_client)
        await elite_client.disconnect()
    
    await site.stop()

    print("Byeeeeeeeeeee!")



if __name__ == "__main__":
    asyncio.run(main())