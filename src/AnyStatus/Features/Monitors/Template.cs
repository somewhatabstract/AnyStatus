namespace AnyStatus
{
    public class Template
    {
        public Template(Item item, string name, string description, string group)
        {
            Item = item;
            Name = name;
            Group = group;
            Description = description;
        }

        public Item Item { get; set; }

        public string Name { get; set; }

        public string Group { get; set; }

        public string Description { get; set; }
    }
}
