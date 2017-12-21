using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Collections.Generic;

namespace SocketTcpServer
{
    namespace SocketTcpServer
    {
        class Program
        {
            const int port = 8888;
            static TcpListener listener;
            static void Main(string[] args)
            {
                try
                {
                    Server s = new Server();
                    listener = new TcpListener(IPAddress.Parse("127.0.0.1"), port);
                    listener.Start();
                    Console.WriteLine("Ожидание подключений...");

                    while (true)
                    {
                        Socket client = listener.AcceptSocket();
                        ClientObject clientObject = new ClientObject(client, s);
                        // создаем новый поток для обслуживания нового клиента
                        Thread clientThread = new Thread(new ThreadStart(clientObject.Process));
                        clientThread.Start();
                    }
                }
                catch (Exception ex)
                {
                    Console.ForegroundColor = ConsoleColor.DarkRed;
                    Console.WriteLine(ex.Message);
                }
            }
        }
        public class ClientObject
        {
            public Socket client;
            Server s;
            public ClientObject(Socket tcpClient, Server s)
            {
                client = tcpClient;
                this.s = s;
            }

            //public void Process()
            //{
            //    NetworkStream stream = null;
            //    string message = "";
            //    int step = 0;
            //    try
            //    {
            //        stream = client.GetStream();
            //        byte[] data = new byte[64]; // буфер для получаемых данных
            //        while (true)
            //        {
            //            stream = client.GetStream();
            //            // получаем сообщение
            //            step = 0;
            //            StringBuilder builder = new StringBuilder();
            //            int bytes = 0;
            //            if (client.Connected)
            //            do
            //            {
            //                bytes = stream.Read(data, 0, data.Length);
            //                builder.Append(Encoding.Unicode.GetString(data, 0, bytes));
            //            }
            //            while (stream.DataAvailable);
            //            else
            //            {
            //                Console.ForegroundColor = ConsoleColor.Yellow;
            //                Console.WriteLine(step);
            //            }
            //            message = builder.ToString();
            //            s.Connect(message);
            //            stream = client.GetStream();
            //            // получаем сообщение
            //            step = 1;
            //            builder = new StringBuilder();
            //            bytes = 0;
            //            if (client.Connected)
            //                do
            //                {
            //                    bytes = stream.Read(data, 0, data.Length);
            //                    builder.Append(Encoding.Unicode.GetString(data, 0, bytes));
            //                }
            //                while (stream.DataAvailable);
            //            else
            //            {
            //                Console.ForegroundColor = ConsoleColor.Yellow;
            //                Console.WriteLine(step);
            //            }
            //            message = builder.ToString();
            //            s.Param(message.Split('|')[0]);
            //            s.Image(message.Split('|')[0]);
            //            s.Rezult(message.Split('|')[0]);
            //            // отправляем обратно сообщение
            //            step = 2;
            //            data = Encoding.Unicode.GetBytes("Ответ сервера");
            //            if (client.Connected)
            //                stream.Write(data, 0, data.Length);
            //            else
            //            {
            //                Console.ForegroundColor = ConsoleColor.Yellow;
            //                Console.WriteLine(step);
            //            }
            //        }
            //    }
            //    catch (Exception ex)
            //    {
            //        //Console.ForegroundColor = ConsoleColor.DarkRed;
            //        //Console.WriteLine(ex.Message);

            //    }
            //}
            public void Process()
            {
                string mess = "Успешно";
                StringBuilder builder = new StringBuilder();
                int bytes = 0; // количество полученных байтов
                byte[] data = new byte[256]; // буфер для получаемых данных
                try
                {
                    while (true)
                    {
                        try
                        {
                            // получаем сообщение
                            builder = new StringBuilder();
                            bytes = 0; // количество полученных байтов
                            data = new byte[256]; // буфер для получаемых данных

                            do
                            {
                                bytes = client.Receive(data);
                                builder.Append(Encoding.Unicode.GetString(data, 0, bytes));
                            }
                            while (client.Available > 0);
                            string message = builder.ToString();
                            s.Connect(message);
                            Thread.Sleep(1);

                            builder = new StringBuilder();
                            bytes = 0; // количество полученных байтов
                            data = new byte[256]; // буфер для получаемых данных

                            do
                            {
                                bytes = client.Receive(data);
                                builder.Append(Encoding.Unicode.GetString(data, 0, bytes));
                            }
                            while (client.Available > 0);
                            message = builder.ToString();
                            s.Param(message.Split('|')[0]);
                            s.Image(message.Split('|')[0]);
                            s.Rezult(message.Split('|')[0]);
                            Thread.Sleep(1);

                        }
                        catch (Exception ex)
                        {
                          //  Console.ForegroundColor = ConsoleColor.Red;
                          //  Console.WriteLine(ex.Message);
                            mess = ex.Message;

                        }
                        data = Encoding.Unicode.GetBytes(mess);
                        client.Send(data);
                        Thread.Sleep(1);
                        // закрываем сокет                        
                    }
                }
                catch (Exception e)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(e.Message);
                }
                client.Shutdown(SocketShutdown.Both);
                client.Close();

            }
        }
        public class Server
        {
            Random rand = new Random();
            List<string> clients = new List<string>();
            public Server()
            {}
            public void Connect(string id) //подключение пользователя к серверу
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine(id + " подключен");
                clients.Add(id);
                Console.ForegroundColor = ConsoleColor.Blue;
                Console.WriteLine("Количество пользователей в системе: " + clients.Count.ToString());

            }
            public void Param(string id)//расчет параметров
            {
                Thread.Sleep(50);
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine(id + ": Параметры расчитаны");
            }
            public void Image(string id)//применение алгоритмов ресайзинга
            {
                Thread.Sleep(rand.Next(76, 1540));
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine(id + ": Алгоритм обработки завершен");
            }
            public void Rezult(string id)// передача ответа сервера
            {
                Thread.Sleep(rand.Next(0, 50));
                Console.ForegroundColor = ConsoleColor.DarkMagenta;
                Console.WriteLine(id + ": Передача результата");
                clients.Remove(id);
                Console.ForegroundColor = ConsoleColor.Blue;
                Console.WriteLine("Количество пользователей в системе: "+clients.Count.ToString());
            }
        }
    }
}
