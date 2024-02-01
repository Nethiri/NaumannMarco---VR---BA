import asyncio
import json
import bleak
import time
from pycycling.tacx_trainer_control import TacxTrainerControl
from pycycling.sterzo import Sterzo
from pycycling.tacx_trainer_control import RoadSurface
import os
import serial
import serial.tools.list_ports
from datetime import datetime
from aiohttp import web


#Trainer Devices:
#Tax Trainer
tacxStatus = True
#Elite Stero
eliteStatus = True
#Break Arduino
arduinoStatus = True

#Tax and Elite are connected via Bluetooth
#their respective clear names (device names) are stored and used in global
TACX_DEVICE_NAME = "Tacx Neo 2 28482"
ELITE_DEVICE_NAME = "STERZO"
ARDUINO_BAUD_RATE = 9600

#Global variable Datapackage
#this variable is shipped on request and edited live
DATAPACKAGE = {
    "tacx_last_update": None,
    "tacx_elapsed_time": None,
    "tacx_distance_travelled": None,
    "tacx_speed": None,
    "tacx_basic_resistance":  None,
    "tacx_road_feel_type": None,
    "tacx_road_feel_intesity": None,
    "elite_angle": None,
    "elite_last_update": None,
    "break_last_update": None,
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

    if address is None: 
        print("Connection to tacx abborted!")
        return False

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
    if client == False:
        print("Couldnt get tacx")
        return False

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
            print(f"Resistance {resistance} was set...")
            await trainer.set_basic_resistance(resistance=resistance)
            global DATAPACKAGE
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
    if trainer is not False:
        trainer.set_general_fe_data_page_handler(tacx_package_handler)
        await trainer.enable_fec_notifications()
        return True
    else:
        print("Trainer is not available.")
        return False

async def tacx_disable_data_package_hanlder(client):
    trainer: TacxTrainerControl = await tacx_get_trainer(client)
    if trainer is not False:
        await trainer.disable_fec_notifications()

async def tacx_define_road_surface(client, roadType=RoadSurface.SIMULATION_OFF, roadIntesity=255): 
    #check if within currently defined data range 0-50 (percent) - according to documentation, eveything above 50 might be dangerous to the device?
    if not (0 <= roadIntesity <= 100):
        print(f"Intensity value {roadIntesity} is outside allowed parameters!")
        return False

    #for testing purposes, lets quickly reset resistance to 0, may be changed later
    await tacx_set_resistance(0, client) 
    global DATAPACKAGE
    try:
        trainer: TacxTrainerControl = await tacx_get_trainer(client)
        if(roadType == RoadSurface.SIMULATION_OFF):
            await trainer.set_neo_modes( isokinetic_mode=False, isokinetic_speed=4.2,
                road_surface_pattern=roadType,
                road_surface_pattern_intensity=255
            )
 
            DATAPACKAGE["tacx_road_feel_type"] = roadType
            DATAPACKAGE["tacx_road_feel_intesity"] = None
            print(f"Road surface defined, roadType: {roadType}, intesity: {255}")
        else: 
            await trainer.set_neo_modes( isokinetic_mode=True, isokinetic_speed=4.2,
                road_surface_pattern=roadType,
                road_surface_pattern_intensity=roadIntesity
            )
            DATAPACKAGE["tacx_road_feel_type"] = roadType
            DATAPACKAGE["tacx_road_feel_intesity"] = roadIntesity
            print(f"Road surface defined, roadType: {roadType}, intesity: {roadIntesity}")
        return True
    except Exception as ex: 
        print("ERROR WHILE TRYING TO SET ROAD SURFACE!")
        print(ex)
        return False
#===ELITE===
async def elite_connect(address=None): 
    #check if address is existent
    if address is None:
        address = await find_device(ELITE_DEVICE_NAME)
        
    if address is None: 
        print("Connection to elite abborted!")
        return False

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
    if client == False:
        print("Couldnt get sterzo")
        return False

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
    if sterzo is not False:
        sterzo.set_steering_measurement_callback(elite_package_handler)
        await sterzo.enable_steering_measurement_notifications()
        return True
    else:
        print("Sterzo is not available.")
        return False
    
async def elite_disable_data_package_handler(client):
    sterzo: Sterzo = await elite_get_sterzo(client)
    if sterzo is not False:
        await sterzo.disable_steering_measurement_notifications()

#===General===
async def keepAllive_async_input(prompt):
    loop = asyncio.get_event_loop()
    return await loop.run_in_executor(None, input, prompt)

async def keepAlliveEvent():
    while True:
        await asyncio.sleep(0)

        # Check if "C" is pressed
        user_input = await keepAllive_async_input("Press 'C' to stop the program: \n")
        if user_input.lower() == 'c':
            break

#===Arduino===
def arduino_find_port(): 
    arduino_ports = [ 
        p.device
        for p in serial.tools.list_ports.comports()
        if 'Arduino' in p.description
    ]
    if arduino_ports:
        return arduino_ports[0]
    else:
        print("Arduino not found! Check connection and try again!")
        return None
    
async def arduino_task(arduino_port):
    arduino_port
    arduino_serial = serial.Serial(arduino_port, ARDUINO_BAUD_RATE)

    while True:
        try:
            while arduino_serial.in_waiting > 0:
                line = arduino_serial.readline().decode('utf-8')
                data = eval(line.strip())
                back_val = int(data.get("back"))
                front_val = int(data.get("front"))

                # Update DATAPACKAGE with resistance values
                global DATAPACKAGE
                DATAPACKAGE["break_last_update"] = time.time()
                DATAPACKAGE["break_back"] = back_val
                DATAPACKAGE["break_front"] = front_val
                #print(f"DATAPACKAGE: {back_val}")
                                    
        except Exception as ex:
            pass
            print("Exception in arduino_read:")
            print(ex)
            pass
        await asyncio.sleep(0.1)

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
            if await tacx_set_resistance(resistance=resistance, client=trainer_client) == False:
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
        await tacx_set_resistance(client=tacx_client, resistance=30)
        #await tacx_define_road_surface(client=tacx_client, roadType=RoadSurface.CONCRETE_PLATES, roadIntesity=50)
    
    if eliteStatus == True:
        elite_client = await elite_connect()
        await elite_set_data_page_handler(elite_client)
    
    if arduinoStatus == True:
        print("Setup Ardunino")
        arduino_port = arduino_find_port()
        if arduino_port is not None:
            asyncio.create_task(arduino_task(arduino_port=arduino_port))
            print("Arduino task is running...")
        else:
            print("Arduino failed to get setup!")

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
    if tacxStatus is True and tacx_client is not False:
        await tacx_disable_data_package_hanlder(tacx_client)
        await tacx_set_resistance(client=tacx_client, resistance=0)
        await tacx_client.disconnect()
    
    if eliteStatus is True and elite_client is not False:
        await elite_disable_data_package_handler(elite_client)
        await elite_client.disconnect()
    
    await site.stop()

    print("Byeeeeeeeeeee!")



if __name__ == "__main__":
    asyncio.run(main())