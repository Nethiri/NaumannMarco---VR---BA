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
                #print(json.dumps(data))
            else:
                print(f"Error: {response.status}")

async def main():
    while True:
        try:
            start = time.time()
            await make_get_request()
            stop = time.time()
            print(f"Duration: {stop-start}" )
            await asyncio.sleep(0.008)
        except:
            pass

if __name__ == "__main__":
    asyncio.run(main())
