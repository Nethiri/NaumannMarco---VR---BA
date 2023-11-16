import aiohttp
import asyncio
import json
import os
import time

async def make_get_request():
    async with aiohttp.ClientSession() as session:
        async with session.get('http://localhost:8057/') as response:
            if response.status == 200:
                return await response.json()
            else:
                print(f"Error: {response.status}")
                return None

async def main():
    while True:
        try:
            start = time.time()
            val = await make_get_request()
            stop = time.time()

            if val is not None:
                os.system('cls' if os.name == 'nt' else 'clear')  # Clear the console
                print("Received data:")
                print(json.dumps(val, indent=2))
                # You can add more specific information from 'val' if needed
                # print(f"Specific Data: {val['specific_key']}")
            
            print(f"Duration: {stop - start}\n")
            await asyncio.sleep(0.08) #0.008 is possible
        except Exception as e:
            print(f"Error: {e}")

if __name__ == "__main__":
    asyncio.run(main())
