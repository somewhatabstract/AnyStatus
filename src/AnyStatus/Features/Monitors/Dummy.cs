using System;
using System.ComponentModel;

namespace AnyStatus
{
    [Browsable(false)]
    [DisplayName("Dummy")]
    [Description("Dummy monitor for tests")]
    public class Dummy : Item, IScheduledItem
    {
        public int Counter { get; set; }

        public bool ThrowException { get; set; }
    }

    public class DummyHandler : IHandler<Dummy>
    {
        public void Handle(Dummy item)
        {
            if (item.ThrowException)
            {
                throw new Exception();
            }

            item.Counter += 1;
            item.State = State.Ok;
        }
    }
}
