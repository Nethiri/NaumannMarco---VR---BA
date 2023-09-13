import http.server
import socketserver

# Constants
PORT = 8055
TAXC_BLTH_IP = ""

# Request handler
class TacxRequestHandler(http.server.SimpleHTTPRequestHandler):
    def do_GET(self):
        if self.path == "/hello":
            self.send_response(200)
            self.send_header("Content-type", "text/html")
            self.end_headers()
            self.wfile.write(b"Hello world and stuff :)")
        else:
            self.send_response(200)
            self.send_header("Content-type", "text/html")
            self.end_headers()
            self.wfile.write(bytes(f"Path was {self.path}", 'utf-8'))

# Create and start the server
with socketserver.TCPServer(("", PORT), TacxRequestHandler) as httpd:
    print(f"Server at http://localhost:{PORT}")
    httpd.serve_forever()