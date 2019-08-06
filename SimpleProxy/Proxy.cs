using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace SimpleProxy
{
    class Proxy
    {
        public const int DEFAULT_PORT = 8080;
        public const string DEFAULT_ADDRESS = "127.0.0.1";

        private TcpListener Listener;
        private readonly string Host;
        private readonly int Port;
        private readonly string Address;
        private const string HEADER = "HTTP/1.1 200 OK\r\n\r\n";

        public Proxy(string host, string address=DEFAULT_ADDRESS, int port=DEFAULT_PORT)
        {
            Host = "http://" + host;
            Port = port;
            Address = address;
            Listener = new TcpListener(IPAddress.Parse(address), port);
        }

        ~Proxy()
        {
            if (Listener != null)
            {
                Listener.Stop();
            }
        }

        /// <summary>
        /// Начинает работу прокси сервера
        /// </summary>
        /// <param name="isOpenBrowser"></param>
        public void Start(bool isOpenBrowser=false)
        {
            Listener.Start();
            Console.WriteLine("Listening port: {0}\nConnected to: {1}", Port.ToString(), Host);
            Console.WriteLine("Press ESC to exit");
            if (isOpenBrowser) Process.Start(string.Format("http://{0}:{1}", Address, Port));
            while (true)
            {
                if (Console.KeyAvailable && Console.ReadKey(true).Key == ConsoleKey.Escape) break;
                if (Listener.Pending())
                {
                    Thread t = new Thread(ClientThread) { IsBackground = true };
                    t.Start(Listener.AcceptTcpClient());
                }
            }
        }

        private void ClientThread(Object arg)
        {
            using (MyTcpClient myClient = new MyTcpClient((TcpClient)arg))
            {
                string destinationUrl = Host + myClient.GetDestinationUrl();
                Console.WriteLine("Новый запрос: {0}", destinationUrl);

                string page = GetPage(destinationUrl);
                myClient.SendResponce(page);
            }
        }

        /// <summary>
        /// Возвращает содержимое страницы по адресу
        /// </summary>
        /// <param name="url">адрес страницы</param>
        private string GetPage(string url)
        {
            var Request = WebRequest.Create(url);
            HttpWebResponse Response;
            try
            {
                Response = (HttpWebResponse)Request.GetResponse();
            }
            catch (WebException)
            {
                Console.WriteLine("Ошибка обращения по адресу: {0}", url);
                return null;
            }

            if (Response.StatusCode == HttpStatusCode.OK)
            {
                using (var content = Response.GetResponseStream())
                using (var reader = new StreamReader(content, Encoding.UTF8))
                {
                    string strContent = reader.ReadToEnd();
                    string headers = Response.Headers.ToString();
                    Response.Close();
                    // Если HTML страница, то модифицируем ее
                    if (strContent.IndexOf("text/html") != -1)
                    {
                        return HEADER + HtmlModifier.Modify(strContent);
                    }
                    return HEADER + strContent;
                }
            }
            return null;
        }
    }
}
