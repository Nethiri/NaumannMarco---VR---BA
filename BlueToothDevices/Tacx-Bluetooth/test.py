from pygatt import GATTToolBackend

# Initialize the Bluetooth backend (GATTTool in this case)
backend = GATTToolBackend()

try:
    backend.start()

    # Scan for nearby Bluetooth devices for a specified duration (e.g., 10 seconds)
    devices = backend.scan(timeout=10)

    # Iterate through discovered devices and print their names and addresses
    for device in devices:
        print("Device Name:", device['name'])
        print("Device Address (BD_ADDR):", device['address'])

finally:
    backend.stop()