// Cliente de chat con soporte para múltiples salas y mensajes privados
using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;

public class ClienteChatSalas
{
    public static void Main()
    {
        Console.OutputEncoding = Encoding.UTF8;
        Console.ForegroundColor = ConsoleColor.Green;
        Console.Write("Ingresa tu nombre: ");
        Console.ResetColor();
        string nombre = Console.ReadLine();

        try
        {
            using TcpClient cliente = new TcpClient("127.0.0.1", 5000);
            using NetworkStream stream = cliente.GetStream();

            byte[] nombreBytes = Encoding.UTF8.GetBytes(nombre);
            stream.Write(nombreBytes, 0, nombreBytes.Length);

            Thread hiloRecepcion = new Thread(() => RecibirMensajes(stream));
            hiloRecepcion.Start();

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Conectado al servidor. Usa /crear, /unirse, /salas, /usuarios, /privado, /salir.");
            Console.ResetColor();

            while (true)
            {
                string mensaje = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(mensaje)) continue;
                byte[] mensajeBytes = Encoding.UTF8.GetBytes(mensaje);
                stream.Write(mensajeBytes, 0, mensajeBytes.Length);

                if (mensaje == "/salir")
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Has salido del chat.");
                    Console.ResetColor();
                    break;
                }
            }
        }
        catch (Exception e)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Error al conectar con el servidor: " + e.Message);
            Console.ResetColor();
        }
    }

    private static void RecibirMensajes(NetworkStream stream)
    {
        byte[] buffer = new byte[1024];

        try
        {
            while (true)
            {
                int bytesLeidos = stream.Read(buffer, 0, buffer.Length);
                if (bytesLeidos == 0) break;

                string mensaje = Encoding.UTF8.GetString(buffer, 0, bytesLeidos);

                if (mensaje.StartsWith("[Sistema]:"))
                {
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.WriteLine(mensaje);
                }
                else if (mensaje.StartsWith("[Privado]"))
                {
                    Console.ForegroundColor = ConsoleColor.Magenta;
                    Console.WriteLine(mensaje);
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine(mensaje);
                }
                Console.ResetColor();
            }
        }
        catch (Exception)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Desconectado del servidor.");
            Console.ResetColor();
        }
    }
}
