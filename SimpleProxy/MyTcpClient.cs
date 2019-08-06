using System;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SimpleProxy
{
    class MyTcpClient : IDisposable
    {
        TcpClient Client;

        public MyTcpClient(TcpClient client)
        {
            Client = client;
        }

        public void Dispose()
        {
            if (Client != null)
            {
                Client.Close();
                Client.Dispose();
            }
        }

        /// <summary>
        /// Возвращает адрес запрашиваемой страницы
        /// </summary>
        /// <returns></returns>
        public string GetDestinationUrl()
        {
            byte[] RequestBytes = GetRequestBytes();
            string Request = Encoding.UTF8.GetString(RequestBytes);
            string pattern = @"^GET (\S*)";
            string match = Regex.Match(Request, pattern).Groups[1].Value;
            return match;
        }

        /// <summary>
        /// Отправляет ответ пользователю
        /// </summary>
        /// <param name="response">Строка ответа</param>
        public void SendResponce(string response)
        {
            if (response != null)
            {
                byte[] data = Encoding.UTF8.GetBytes(response);
                Client.GetStream().Write(data, 0, data.Length);
            }
        }

        /// <summary>
        /// Получает данные запроса, отправленного пользователем
        /// </summary>
        private byte[] GetRequestBytes()
        {
            byte[] buffer = new byte[Client.ReceiveBufferSize];
            int count = 0;

            try
            {
                count = Client.GetStream().Read(buffer, 0, buffer.Length);
            }
            catch (IOException e)
            {
                Console.WriteLine("Ошибка чтения запроса: {0}", e.ToString());
                return null;
            }
            return buffer;
        }
    }
}
