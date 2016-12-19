using System;

namespace AnyStatus
{
    public class RefreshCommand : ItemCommand
    {
        public RefreshCommand(Item item) : base(item) { }
    }

    public class RefreshCommandHandler : IHandler<RefreshCommand>
    {
        private readonly IJobScheduler _jobScheduler;

        public RefreshCommandHandler(IJobScheduler jobScheduler)
        {
            _jobScheduler = Preconditions.CheckNotNull(jobScheduler, nameof(jobScheduler)); ;
        }

        public void Handle(RefreshCommand command)
        {
            if (command == null)
                throw new InvalidOperationException();

            _jobScheduler.Execute(command.Item);
        }
    }
}