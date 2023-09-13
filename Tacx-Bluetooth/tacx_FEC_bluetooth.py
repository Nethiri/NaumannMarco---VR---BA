import asyncio
from bleak import BleakClient

async def handle_notifications(sender, data):
    # Process FEC notifications received from the trainer
    print(f"Received data from {sender}: {data}")

async def connect_to_tacx_trainer(device_address):
    async with BleakClient(device_address) as client:
        await client.is_connected()
        # Subscribe to FEC notifications (specific to your Tacx trainer)
        await client.start_notify(fec_notification_uuid, handle_notifications)
        await asyncio.sleep(60)  # Run for 60 seconds
        await client.stop_notify(fec_notification_uuid)

if __name__ == "__main__":
    device_address = "C9:BD:0B:8D:56:C7"
    loop = asyncio.get_event_loop()
    loop.run_until_complete(connect_to_tacx_trainer(device_address))
