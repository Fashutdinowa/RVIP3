using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace SocketTcpClient
{
    class Program
    {
        const int port = 8888;
        const string address = "127.0.0.1";
        static void Main(string[] args)
        {
            Console.Write("Количество пользователей: ");
            int count = Convert.ToInt32(Console.ReadLine());
            int k = 1;
            Client client = new Client(port, address, k);
            System.Timers.Timer t = new System.Timers.Timer();
            t.Elapsed += (s, ev) =>
            {
                try
                {
                    new Thread(delegate ()
                    {
                        new Client(port, address, k);
                    }).Start();//создаем поток на коиента изапускаем его
                            k++;
                    if (k == count)//проверка на количество созданных потоков
                                t.Stop();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            };
            t.Interval = 1000; //интервал в мс
            t.Start(); //запускаем таймер


            Console.ReadKey();
        }
    }
    public class Client
    {
        string id;
        int image; //размер изображения в мб
        Random rand = new Random();
        Socket TCPclient;
        public Client(int port, string address, int id)
        {
            this.id = "Пользователь " + id.ToString();
            try
            {
                IPEndPoint ipPoint = new IPEndPoint(IPAddress.Parse(address), port);

                TCPclient = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                // подключаемся к удаленному хосту
                TCPclient.Connect(ipPoint);
                // преобразуем сообщение в массив байтов
                byte[] data = Encoding.Unicode.GetBytes(this.id);
                // отправка сообщения
                TCPclient.Send(data);
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine(this.id + ": Подключение успешно");
                Console.ForegroundColor = ConsoleColor.White;
                Run();
            }
            catch (Exception e)
            {
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.WriteLine(this.id + ": " + e.Message);
                TCPclient.Shutdown(SocketShutdown.Both);
                TCPclient.Close();
            }

        }
        public void Run() //работа клиента
        {

            Thread Data = new Thread(AddParamet);
            Thread Image = new Thread(AddImage);
            Data.Start();
            Image.Start();
            Data.Join();
            Image.Join();
            // преобразуем сообщение в массив байтов
            byte[] data = Encoding.Unicode.GetBytes(this.id+"|"+this.image);
            // отправка сообщения
            TCPclient.Send(data);
            Thread Rezultat = new Thread(Rezult);
            Rezultat.Start();

        }
        private void AddParamet() //настройка параметров
        {
            Thread.Sleep(rand.Next(0, 50));
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(id + ": Параметры настроены");
        }
        private void AddImage() //загрузка изображения
        {
            image = rand.Next(50, 1000);
            Thread.Sleep(image * 1);
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(id + ": Изображение " + image + "Мб загружено");
        }
        private void Rezult()
        {
            // получаем ответ
            byte[] data = new byte[256]; // буфер для ответа
            StringBuilder builder = new StringBuilder();
            int bytes = 0; // количество полученных байт
            try
            {
                do
                {
                    bytes = TCPclient.Receive(data, data.Length, 0);
                    builder.Append(Encoding.Unicode.GetString(data, 0, bytes));
                }
                while (TCPclient.Available > 0);
            }
            catch(Exception  e)
            {
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.WriteLine(id + ": " + e.Message);
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.WriteLine(id + ": " + "Завершение работы");
                return;
                TCPclient.Shutdown(SocketShutdown.Both);
                TCPclient.Close();

            }
            Console.ForegroundColor = ConsoleColor.DarkMagenta;
            Console.WriteLine(id + ": Отображение результата"+": "+builder.ToString());
            //TCPclient.Shutdown(SocketShutdown.Both);
            //TCPclient.Close();
        }
       
    }
}