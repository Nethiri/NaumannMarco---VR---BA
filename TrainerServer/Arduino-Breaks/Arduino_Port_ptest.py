import serial.tools.list_ports

def find_arduino_port():
    arduino_ports = [
        p.device
        for p in serial.tools.list_ports.comports()
        if 'Arduino' in p.description
    ]

    if arduino_ports:
        return arduino_ports[0]  # Assume the first Arduino found is the one you want
    else:
        raise Exception('Arduino not found. Check the connection and try again.')

if __name__ == "__main__":
    try:
        arduino_port = find_arduino_port()
        print(f'Arduino found on port: {arduino_port}')

        # Now you can use arduino_port to communicate with the Arduino
        # e.g., ser = serial.Serial(arduino_port, baud_rate)
    except Exception as e:
        print(f'Error: {str(e)}')