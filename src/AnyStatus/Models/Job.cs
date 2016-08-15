namespace AnyStatus.Models
{
    public class Job : Item
    {
        public Job() : base()
        {
        }

        public Job(Item parent) : base(parent)
        {
        }

        public string Url { get; set; }

        public string UserName { get; set; }

        public string ApiToken { get; set; }
    }
}