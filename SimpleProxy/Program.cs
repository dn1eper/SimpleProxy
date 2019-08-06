namespace SimpleProxy
{
    class Program
    {
        static void Main(string[] args)
        {
            string host = args.Length > 0 ? args[0] : "habrahabr.ru";
            Proxy proxy = new Proxy(host);
            proxy.Start(true);
        }
    }
}
