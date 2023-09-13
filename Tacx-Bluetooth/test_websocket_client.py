import asyncio
import websockets
import aioconsole
import re

pattern = r"speed=([-+]?\d*\.\d+|\d+)"
async def receive_updates():
    async with websockets.connect("ws://localhost:8057") as websocket:
        while True:
            # Receive and print updates from the server
            message = await websocket.recv()
            print(f"Received: {message}")
            match = re.search(pattern, message)
            print(f"Speed: {match.group(1)}" )

async def input_listener():
    while True:
        key = await aioconsole.ainput()
        if key == 'c':
            print("Disconnecting from WebSocket...")
            break

async def main():
    await asyncio.gather(receive_updates(), input_listener())

if __name__ == "__main__":
    asyncio.run(main())
