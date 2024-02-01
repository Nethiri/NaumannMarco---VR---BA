import requests
import json

# Set the resistance value you want
resistance_value = 10

# Define the data payload
data = {
    'resistance': resistance_value
}
# Define the URL
url = 'http://localhost:8057/set_resistance'

# Send the POST request
response = requests.post(url, json=data)

# Print the response
print('Response:', response.status_code)
print('Response Content:', response.text)

#{'resistance': 10}