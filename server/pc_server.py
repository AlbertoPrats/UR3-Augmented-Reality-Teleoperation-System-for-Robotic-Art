import urlib
from xmlrpc.server import SimpleXMLRPCServer
import socket


class server:
    def __init__(self):
        self.sock = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
        self.port = 50000
        self.cola_mensajes = []
        self.mensaje = []
        self.orden = 'Nada'
        self.coordenadas = []
        self.angulos = ' 0.695 3.115 0.053'   #valores de rotacion para el workspace
        self.conn = None
        print("\nRecibiendo mensaje desde Unity en formato: Orden/X1 Y1 Z1/X2 Y2 Z2/... \n\nPosibles ordenes: \nNada (no hace nada) \nExit (sale del programa) \nDibujar (sigue las coordenadas que recibe)")

    def conectar_unity(self):
        server_address = ('10.42.0.1', 1100)
        self.sock.bind(server_address)
        print("Conectando a Unity...")
        self.sock.listen(5)
        self.conn, self.addr = self.sock.accept()
        print(f'Conectado desde {self.addr}')
        print("Conectado a Unity")

    def desconectar_unity(self):
        self.conn.close()
        self.sock.close()

    def recibir_mensaje(self):
        if self.conn == None:
            self.conectar_unity()
            return False
        else:
            # Recibir y procesar datos del cliente
            data = self.conn.recv(1024)
            #print(f'Recibido: {data.decode()}')
            self.cola_mensajes.append(data.decode())
            # Enviar una respuesta al cliente
            self.conn.sendall(b'Mensaje recibido correctamente')
            return True

    def descifrar(self):
        #ejemplo = "Dibujar/1 2 3/2 3 4/2.1 3.1 4.1"
        orden = 'Nada'
        coordenadas = []
        self.mensaje = self.cola_mensajes.pop(0)
        parte1 = self.mensaje.split("/", 1)
        orden = parte1[0]
        #print("orden: ", orden)
        parte2 = parte1[1].split("/")
        for i in range(len(parte2)):
            parte2[i] = parte2[i] + self.angulos        
            extracto = parte2[i].split(" ")
            for j in range(len(extracto)):
                if len(extracto) != 6:
                    print("Se ha detectado una sección del mensaje sin tres coordenadas. Asegura que para cada posición existen las coordendas (x y z)")
                    orden = 'Nada'
                    coordenadas = []
                    return False
                extracto[j] = (float(extracto[j]))
            coordenadas.append(extracto)
        
        #print("array de coordenadas: ", coordenadas)
        self.coordenadas = coordenadas
        return orden


    def get_decision(self, n):
        if n ==1:
            if (self.recibir_mensaje()):
                orden = self.descifrar()
            else:
                orden = 'Nada'
        else:
            orden = 'Nada'
        if orden == 'Exit':
            decision = -1
        elif orden == 'Nada':
            decision = 0
        elif orden == 'Dibujar':
            decision = len(self.coordenadas)
            print("Se van a ejecutar ", decision, "movimientos")
        else:
            decision = 0
            print("Se ha recibido una orden no válida")

        n = 0
        return decision

    def coord_parser(self):
        if len(self.coordenadas) < 1:
            print("No hay coordenadas para parsear")
        else:
            #print("Parseando...")
            pose = self.coordenadas.pop(0)
            print("Moving to pose: " + str(pose))
            return urlib.listToPose(pose)

    def run(self):
        server = SimpleXMLRPCServer(("", self.port), allow_none=True, logRequests=False)
        server.RequestHandlerClass.protocol_version = "HTTP/1.1"
        print("\nListening on port ", self.port)
        server.register_function(self.get_decision, "get_decision")
        server.register_function(self.coord_parser, "coord_parser")

        server.serve_forever()

        self.conectar_unity()

servidor = server()
try:
    servidor.run()
except:
    servidor.desconectar_unity()