namespace AnyStatus
{
    public class RefreshCommand : BaseItemCommand
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
            if (command.Item == null)
                return;

            _jobScheduler.Execute(command.Item);
        }
    }
}