using System;

namespace AnyStatus
{
    public class Template
    {
        public Template(Item item, string name, string description = "")
        {
            Item = item;
            Name = name;
            Description = description;
        }

        public Item Item { get; set; }

        //public Type Type { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }
    }
}
