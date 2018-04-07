import socket
import sys

# Create a TCP/IP socket
sock = socket.socket(socket.AF_INET, socket.SOCK_STREAM)

print("Enter Main HUB IP(default = 127.0.0.1):")
addr = input()

if not addr:
    addr = 'localhost'

print("Enter location (longitude and latitude):")
x = input()
y = input()

# Connect the socket to the port where the server is listening
server_address = ('localhost', 10000)
sock.connect(server_address)

try:
    
    # Send data
    message = x+','+y
    sock.sendall(message.encode())

    # Look for the response
##    amount_received = 0
##    amount_expected = len(message)
##    
##    while amount_received < amount_expected:
##        data = sock.recv(16).decode()
##        amount_received += len(data)

finally:
    sock.close()

    
