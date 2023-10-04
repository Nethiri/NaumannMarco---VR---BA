import asyncio
from bleak import BleakClient
import asyncio
import json
import bleak
import time
from aiohttp import web
from pycycling.tacx_trainer_control import TacxTrainerControl
from datetime import datetime
import os

from pycycling.sterzo import Sterzo

count = 0

async def run(address):

    async with BleakClient(address) as client:
        
        def steering_handler(steering_angle):
            global count 
            count += 1
            print(steering_angle)

        await client.is_connected()
        sterzo = Sterzo(client)
        sterzo.set_steering_measurement_callback(steering_handler)
        await sterzo.enable_steering_measurement_notifications()
        await asyncio.sleep(100)
        global count
        print(count)
    

        
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

async def main():
    os.environ["PYTHONASYNCIODEBUG"] = str(1)

    device_address = await find_device("STERZO")
    print(device_address)
    await run(device_address)


if __name__ == "__main__":
   asyncio.run(main())