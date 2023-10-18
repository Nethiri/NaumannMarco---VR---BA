import aiohttp
import asyncio
import json
import os
import time

async def make_get_request():
    async with aiohttp.ClientSession() as session:
        async with session.get('http://localhost:8057/') as response:
            if response.status == 200:
                data = await response.json()
                #print("Received data:")
                print('\033c', end='')
                print(json.dumps(data, indent=2))
                return data
                #print(json.dumps(data))
            else:
                print(f"Error: {response.status}")
                return None

async def main():
    angleMin = 0
    angleMax = 0
    while True:
        try:
            start = time.time()
            val = await make_get_request()
            stop = time.time()
            if val is not None:
                angle = val["elite_angle"]
                if angle < angleMin: 
                    angleMin = angle
                if angle > angleMax: 
                    angleMax = angle
            
            print(f"Duration: {stop-start}" )
            print(f"Min: {angleMin} and Max: {angleMax}")
            await asyncio.sleep(0.008)
        except:
            pass

if __name__ == "__main__":
    asyncio.run(main())
