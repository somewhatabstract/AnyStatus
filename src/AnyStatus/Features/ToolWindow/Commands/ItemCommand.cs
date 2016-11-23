namespace AnyStatus
{
    public class ItemCommand
    {
        public ItemCommand(Item item)
        {
            Item = item;
        }

        public Item Item { get; set; }
    }
}