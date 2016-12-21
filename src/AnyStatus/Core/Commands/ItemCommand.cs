namespace AnyStatus
{
    public abstract class ItemCommand
    {
        public ItemCommand(Item item)
        {
            Item = Preconditions.CheckNotNull(item, nameof(item));
        }

        public Item Item { get; set; }
    }
}