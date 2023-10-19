import bleak
import asyncio

async def find_device():
    devices = await bleak.BleakScanner.discover()
    print(devices)
    

async def main():
    await find_device() 

if __name__ == "__main__":
    asyncio.run(main())