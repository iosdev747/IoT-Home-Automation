import socket
import sys

sock = socket.socket(socket.AF_INET, socket.SOCK_STREAM)

server_address = ('localhost', 10000)
sock.bind(server_address)

# Listen for incoming connections
sock.listen(1)

while True:
    # Wait for a connection
    connection, client_address = sock.accept()
    try:
        # Receive the data in small chunks and retransmit it
        while True:
            data = connection.recv(16).decode()
            if data:
                connection.sendall(data.encode())
            else:
                break            
    finally:
        # Clean up the connection
        connection.close()
