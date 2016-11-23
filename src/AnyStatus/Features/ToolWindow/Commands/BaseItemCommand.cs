namespace AnyStatus
{
    public class BaseItemCommand
    {
        public BaseItemCommand(Item item)
        {
            Item = item;
        }

        public Item Item { get; set; }
    }
}