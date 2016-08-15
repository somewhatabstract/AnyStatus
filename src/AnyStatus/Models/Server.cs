namespace AnyStatus.Models
{
    public class Server
    {
        public Server()
        {
        }

        public Server(string url)
        {
            Url = url;
        }

        public string Name { get; set; }

        public string UserName { get; set; }

        public string ApiToken { get; set; }

        public string Url { get; set; }
    }
}
