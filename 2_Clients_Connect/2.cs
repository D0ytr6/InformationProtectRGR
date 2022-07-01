using System;
using System.Numerics;
using System.Text;
using System.Net;
using System.Net.Sockets;

namespace RSAENCRYPT
{
    class Program
    {
        static int port = 8005; // порт 
        static string address = "127.0.0.1"; // адрес сервера
        static int GCD(int a, int b)
        {
            while (b != 0)
            {
                var t = b;
                b = a % b;
                a = t;
            }
            return a;
        }

        static void startServer() {

            int n = 1921, v = 43, J = 0, G = 101;
            int r = 85;
            BigInteger T;
            double val;
            BigInteger J2;
            

            //val = (Math.Pow(G, v)) % n;
            //Console.WriteLine(val);

            double s0 = G;

            for (int i = 2; i <= v; i++)
            {
                s0 = s0 * G;
            }

            s0 = s0 % n;

            Console.WriteLine(s0);


            int step1 = 0;
            for (double k = 0; k < n; k++){
                step1 = (int)((s0 * k) - 1);
                if (step1 % n == 0)
                {
                    Console.WriteLine("Calculated J value = " + k.ToString());
                    J = (int)k;
                }
            }

            int a = GCD(J, n);
            Console.WriteLine(a);
            string str_values = "n" + n + "v" + v + "J" + J;
            //Console.WriteLine(a);
            IPEndPoint ipPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), port);

            // создаем сокет
            Socket listenSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                // связываем сокет с локальной точкой, по которой будем принимать данные
                listenSocket.Bind(ipPoint);

                // начинаем прослушивание
                listenSocket.Listen(10);

                //Console.WriteLine("Сервер запущен. Ожидание подключений...");

                while (true)
                {
                    Socket handler = listenSocket.Accept();
                    int bytes = 0;
                    byte[] data = new byte[256]; // буфер для данных

                    // отправляем ответ
                    data = Encoding.Unicode.GetBytes(str_values);
                    handler.Send(data);
                    Console.WriteLine("Send n, v, j values to B user");

                    int step = (int)r;
                    
                    for (int i = 2; i <= v; i++)
                    {
                        r = r * step;
                    }

                    T = r % n;
                    Console.WriteLine("Calculated T value = " + T.ToString());

                    string str_T = T.ToString();
                    data = Encoding.Unicode.GetBytes(str_T);
                    handler.Send(data);
                    Console.WriteLine("Send T value to B user");

                    bytes = handler.Receive(data);
                    string string_d = Encoding.Unicode.GetString(data, 0, bytes);
                    Console.WriteLine("Get d value" + string_d);
                    int d = Convert.ToInt32(string_d);
                   
                    r = 85;
                    
                    double D = r * Math.Pow(G, d);
                    D = D % n;
                    Console.WriteLine("Calculated D value = " + D.ToString());
                    string strD = D.ToString();
                    data = Encoding.Unicode.GetBytes(strD);
                    handler.Send(data);
                    Console.WriteLine("Send D value to user B");

                    // закрываем сокет
                    handler.Shutdown(SocketShutdown.Both);
                    handler.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

        }

        static void startClient() {

            int n = 0, v = 0, j = 0;
            int d = 13;

            try
            {
                IPEndPoint ipPoint = new IPEndPoint(IPAddress.Parse(address), port);

                Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                // подключаемся к удаленному хосту
                socket.Connect(ipPoint);
                byte[] data = new byte[256];

                // получаем ответ
                //data = new byte[256]; // буфер для ответа
                StringBuilder builder = new StringBuilder();
                int bytes = 0; // количество полученных байт

                bytes = socket.Receive(data, data.Length, 0);
                string values = Encoding.Unicode.GetString(data, 0, bytes);
                Console.WriteLine(values);

                string temp_str = "";
                temp_str = values.Substring(1, 4);
                n = Convert.ToInt32(temp_str);
                temp_str = values.Substring(6, 2);
                v = Convert.ToInt32(temp_str);
                temp_str = values.Substring(9, 4);
                j = Convert.ToInt32(temp_str);

                Console.WriteLine("Get values: n, v, j" + n.ToString() + v.ToString() + j.ToString());

                bytes = socket.Receive(data, data.Length, 0);
                string value_T = Encoding.Unicode.GetString(data, 0, bytes);
                Console.WriteLine("Get value T" + value_T.ToString());

                
                string string_d = d.ToString();
                data = Encoding.Unicode.GetBytes(string_d);
                socket.Send(data);
                Console.WriteLine("Send d value" + d.ToString());

                bytes = 0;
                data = new byte[256];
                bytes = socket.Receive(data, data.Length, 0);
                string value_D = Encoding.Unicode.GetString(data, 0, bytes);
                int D = Convert.ToInt32(value_D);
                Console.WriteLine("Get D value = " + D.ToString());
                
                BigInteger s1 = D;
                BigInteger s2 = j;
                for (int i = 2; i <= v; i++)
                {
                    s1 = s1 * D;
                }

                for (int i = 2; i <= d; i++)
                {
                    s2 = s2 * j;
                }

                BigInteger T = s1 * s2;
                T = T % n;

                Console.WriteLine("Calculated T1 value = " + T.ToString());

                // закрываем сокет
                socket.Shutdown(SocketShutdown.Both);
                socket.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            Console.Read();

        }

        
        static void Main(string[] args)
        {
            Console.WriteLine("Chose a type of program! Sever(1) or Client(2)");
            int result = Convert.ToInt32(Console.ReadLine());
            if (result == 1) {
                startServer();
            }
            if (result == 2) {
                startClient();
            }
        }
    }
}
