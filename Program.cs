
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace dzSocetTolkServrClient_21112023
{
    internal class Program
    {
        //  Варианты для диалогов(выбрать любой один) :
        //   продавец/покупатель
       //public string serverIpStr = "25.45.70.14"; // ip адрес сокета сервера
       //public int serverPort = 2620; // порт сокета сервера
        static void RunServer(string serverIpStr, int serverPort)
        {
            // 0 конфигурация сервера
            
            Socket server = null;
            Socket client = null;
            try
            {
                // 1 подготовить endpoint для работы сервера 
                IPAddress serverIp = IPAddress.Parse(serverIpStr);
                IPEndPoint serverEndpoint = new IPEndPoint(serverIp, serverPort);
                // 2 создать сокет сервера и присоединить его к endpoint
                server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                server.Bind(serverEndpoint); // связывание с сервером и подключение к нему
                                             // 3 перевести сокет в режим прослушивания входящих подключений
                server.Listen(1);
                // 4 начать ожидание входящего подключения
                Console.WriteLine("server>Ожидание входящего подключения ....");
                client = server.Accept();
                Console.WriteLine($"server>Произошло подключение: {client.RemoteEndPoint}");
                Console.ReadLine();
               
                // 5.1 общение с клиентом
                string message = $"Вы подключились, время подключения {DateTime.Now}";
                client.Send(Encoding.UTF8.GetBytes(message));
                Console.WriteLine($"server> отправлено сообщение {message}");
                
                // начинаем общение
                // 5.2  получить сообщение от клиента 1 сообщение
                byte[] buffer = new byte[1024];
                int bytesRead = client.Receive(buffer);
                message = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                Console.WriteLine($"\nserver> получено сообщение   {message}");
                // сервер отправляет клиенту первое сообщение 
                message = "говорит сервер: Есть, 750 рублей, за 2 дешевле будет";
                client.Send(Encoding.UTF8.GetBytes(message));
                
                Console.WriteLine($"server> отправлено сообщение {message}");

                // 5.3  получить сообщение от клиента 2 сообщение
                buffer = new byte[1024];
                int bytesReadqv = client.Receive(buffer);
                message = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                Console.WriteLine($"\nserver> получено сообщение   {message}");
                // сервер отправляет клиенту второе сообщение
                message = "говорит сервер: Берите 4 лопаты и заплатите 2000р";
                client.Send(Encoding.UTF8.GetBytes(message));
                Console.WriteLine($"server> отправлено сообщение {message}");

                // завершаем работу сервера
                Console.WriteLine("server>Завершение работы сервера...");
                // завершение работы склиента
                client.Shutdown(SocketShutdown.Both);
                Console.WriteLine("client> Завершение работы клиента");
                Console.ReadLine();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"При работе сервера возникло исключение {ex.Message}");
            }
            finally
            {
                server?.Close();
                client?.Close();
            }
        }
        // Процедура запуска линейного алгоритма работы клиента (активного сокета)
        static void RunClient(string serverIpStr, int serverPort)
        {
            // СОЗДАДИМ ПРОСТЕЙШИЙ КЛИЕНТ
            // Клиент инициирует подключение к серверу
            // Вывод сообщение о том, что произошло подключение
            Socket client = null;
            // ХОД РАБОТЫ:
            try
            {
                // 1. Создадим сокет клиента 
                client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                // 2. Подготовить endpoint  сервера
                IPAddress serverIp = IPAddress.Parse(serverIpStr);
                IPEndPoint serverEndpoint = new IPEndPoint(serverIp, serverPort);

                // 3. Подключиться к серверу
                Console.WriteLine("client>Подключение к серверу...");
                client.Connect(serverEndpoint);
                Console.WriteLine($"client>Произошло подключение ...{client.LocalEndPoint}-> {client.RemoteEndPoint}");

                // 4.1 получить сообщение от сервера  1 сообщение
                byte[] buffer = new byte[1024]; // буфер для чтения сообщения
                int byteRead = client.Receive(buffer); // читам сообщения в буфер, результат кол-во байт
                string message = Encoding.UTF8.GetString(buffer, 0, byteRead); // декодируем байты в строку
                // отправка сообщения на сервер
                message = "говорит клиент: Здравствуйте.Лопаты есть?";
                client.Send(Encoding.UTF8.GetBytes((string)message));

                // 4.1 получить сообщение от сервера  2 сообщение
                buffer = new byte[1024]; // буфер для чтения сообщения
                byteRead = client.Receive(buffer); // читам сообщения в буфер, результат кол-во байт
                string messageqv = Encoding.UTF8.GetString(buffer, 0, byteRead); // декодируем байты в строку
                // отправка сообщения на сервер
                message = "";
                message = "говорит клиент: Давайте четыре";
                client.Send(Encoding.UTF8.GetBytes((string)message));

                Console.WriteLine("client>Завершение работы клиента...");
                Console.ReadLine();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"При работе клиента возникло исключение {ex.Message}");
            }
            finally
            {
                client?.Close();
            }
        }
        static void Main(string[] args)
        {
            // Алгоритм общения (линейный):
            // 1) После установки подключения сервер отправляет клиенту сообщение о том, что произошло подключение и время подключения
            // 2) Клиент отправляет серверу сообщение
            // 3) Сервер отправляет клиенту сообщение Goodbye, работа завершается
            // Запустим сервер и клиент в разных потоках
            string serverIpStr = "127.0.0.1"; // ip адрес сокета сервера
            int serverPort = 2620; // порт сокета сервера
            Thread serverThread = new Thread(() => RunServer(serverIpStr, serverPort));
            Thread clientThread = new Thread(() => RunClient(serverIpStr, serverPort));

            // запустить потоки
            serverThread.Start();
            clientThread.Start();

            // дождаться завершение потока
            serverThread.Join();
            clientThread.Join();
        }
    }
}
