// Servidor de chat con soporte para múltiples salas y mensajes privados
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

public class ServidorChatSalas
{
    private static readonly Dictionary<string, List<TcpClient>> salas = new();
    private static readonly Dictionary<TcpClient, string> nombres = new();
    private static readonly Dictionary<TcpClient, string> salaCliente = new();
    private static readonly object lockObj = new();

    public static void Main()
    {
        TcpListener servidor = new TcpListener(IPAddress.Any, 5000);
        servidor.Start();
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("Servidor iniciado en el puerto 5000");
        Console.ResetColor();

        while (true)
        {
            TcpClient cliente = servidor.AcceptTcpClient();
            Thread t = new Thread(() => ManejarCliente(cliente));
            t.Start();
        }
    }

    private static void ManejarCliente(TcpClient cliente)
    {
        try
        {
            NetworkStream stream = cliente.GetStream();
            byte[] buffer = new byte[1024];
            int bytesLeidos = stream.Read(buffer, 0, buffer.Length);
            string nombre = Encoding.UTF8.GetString(buffer, 0, bytesLeidos);

            lock (lockObj)
            {
                nombres[cliente] = nombre;
            }

            EnviarMensajeSistema(cliente, $"Bienvenido {nombre}! Usa /crear, /unirse, /salas, /usuarios, /privado, /salir");

            while ((bytesLeidos = stream.Read(buffer, 0, buffer.Length)) != 0)
            {
                string mensaje = Encoding.UTF8.GetString(buffer, 0, bytesLeidos).Trim();
                ProcesarMensaje(cliente, mensaje);
            }
        }
        catch
        {
            DesconectarCliente(cliente);
        }
    }

    private static void ProcesarMensaje(TcpClient cliente, string mensaje)
    {
        if (mensaje.StartsWith("/crear "))
        {
            CrearSala(cliente, mensaje[7..]);
        }
        else if (mensaje.StartsWith("/unirse "))
        {
            UnirseSala(cliente, mensaje[8..]);
        }
        else if (mensaje == "/salas")
        {
            ListarSalas(cliente);
        }
        else if (mensaje == "/usuarios")
        {
            ListarUsuarios(cliente);
        }
        else if (mensaje.StartsWith("/privado "))
        {
            EnviarPrivado(cliente, mensaje[9..]);
        }
        else if (mensaje == "/salir")
        {
            SalirSala(cliente);
            DesconectarCliente(cliente);
        }
        else
        {
            EnviarMensajeSala(cliente, mensaje);
        }
    }

    private static void CrearSala(TcpClient cliente, string sala)
    {
        lock (lockObj)
        {
            if (!salas.ContainsKey(sala))
                salas[sala] = new List<TcpClient>();

            SalirSala(cliente);
            salas[sala].Add(cliente);
            salaCliente[cliente] = sala;
            EnviarMensajeSistema(cliente, $"Sala '{sala}' creada y unida.");
        }
    }

    private static void UnirseSala(TcpClient cliente, string sala)
    {
        lock (lockObj)
        {
            if (!salas.ContainsKey(sala))
            {
                EnviarMensajeSistema(cliente, $"La sala '{sala}' no existe.");
                return;
            }
            SalirSala(cliente);
            salas[sala].Add(cliente);
            salaCliente[cliente] = sala;
            EnviarMensajeSistema(cliente, $"Te uniste a la sala '{sala}'.");
        }
    }

    private static void ListarSalas(TcpClient cliente)
    {
        lock (lockObj)
        {
            string lista = "Salas disponibles:\n" + string.Join("\n", salas.Keys);
            EnviarMensajeSistema(cliente, lista);
        }
    }

    private static void ListarUsuarios(TcpClient cliente)
    {
        lock (lockObj)
        {
            if (!salaCliente.ContainsKey(cliente))
            {
                EnviarMensajeSistema(cliente, "No estás en ninguna sala.");
                return;
            }
            string sala = salaCliente[cliente];
            string lista = "Usuarios en esta sala:\n";
            foreach (var c in salas[sala])
                lista += "- " + nombres[c] + "\n";
            EnviarMensajeSistema(cliente, lista);
        }
    }

    private static void EnviarPrivado(TcpClient remitente, string contenido)
    {
        int idx = contenido.IndexOf(' ');
        if (idx == -1)
        {
            EnviarMensajeSistema(remitente, "Formato inválido. Usa /privado <usuario> <mensaje>");
            return;
        }

        string destino = contenido[..idx];
        string mensaje = contenido[(idx + 1)..];

        TcpClient receptor = null;

        lock (lockObj)
        {
            foreach (var kv in nombres)
            {
                if (kv.Value == destino)
                {
                    receptor = kv.Key;
                    break;
                }
            }

            if (receptor == null)
            {
                EnviarMensajeSistema(remitente, "Usuario no encontrado.");
                return;
            }

            string texto = $"[Privado] {nombres[remitente]}: {mensaje}";
            byte[] datos = Encoding.UTF8.GetBytes(texto);
            receptor.GetStream().Write(datos, 0, datos.Length);
        }
    }

    private static void SalirSala(TcpClient cliente)
    {
        lock (lockObj)
        {
            if (salaCliente.TryGetValue(cliente, out string sala))
            {
                salas[sala].Remove(cliente);
                salaCliente.Remove(cliente);
            }
        }
    }

    private static void EnviarMensajeSistema(TcpClient cliente, string mensaje)
    {
        try
        {
            byte[] datos = Encoding.UTF8.GetBytes("[Sistema]: " + mensaje);
            cliente.GetStream().Write(datos, 0, datos.Length);
        }
        catch { }
    }

    private static void EnviarMensajeSala(TcpClient cliente, string mensaje)
    {
        lock (lockObj)
        {
            if (!salaCliente.TryGetValue(cliente, out string sala))
            {
                EnviarMensajeSistema(cliente, "Debes unirte a una sala primero.");
                return;
            }

            string texto = $"{nombres[cliente]}: {mensaje}";
            byte[] datos = Encoding.UTF8.GetBytes(texto);

            foreach (var c in salas[sala])
            {
                if (c == cliente) continue;
                try
                {
                    c.GetStream().Write(datos, 0, datos.Length);
                }
                catch { }
            }
        }
    }

    private static void DesconectarCliente(TcpClient cliente)
    {
        lock (lockObj)
        {
            SalirSala(cliente);
            nombres.Remove(cliente);
            cliente.Close();
        }
    }
}
