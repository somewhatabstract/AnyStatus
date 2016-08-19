namespace AnyStatus.Models
{
    public class Template
    {
        public Template()
        {
        }

        public Template(string name, Item item)
        {
            Name = name;
            Item = item;
        }

        public string Name { get; set; }

        public Item Item { get; set; }
    }
}
