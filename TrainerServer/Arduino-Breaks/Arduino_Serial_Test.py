import serial
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


def read_arduino(serial): 
    # Read the data from the Arduino
    arduino_data = serial.readline().decode('utf-8').strip()

    # Process the data as needed
    print('Received measurement:', arduino_data)

# Configure the serial port to match the Arduino's settings
arduino_port = find_arduino_port()
baud_rate = 9600
ser = serial.Serial(arduino_port, baud_rate)

while True:
    read_arduino(ser)