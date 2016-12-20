using System;

namespace AnyStatus
{
    public class Mediator : IMediator
    {
        private readonly ILogger _logger;

        public Mediator(ILogger logger)
        {
            _logger = Preconditions.CheckNotNull(logger, nameof(logger));
        }

        public void Send(object request)
        {
            Send(request, typeof(IHandler<>));
        }

        public void TrySend(object request)
        {
            TrySend(request, typeof(IHandler<>));
        }

        public void TrySend(object request, Type handlerType)
        {
            try
            {
                Send(request, handlerType);
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }
        }

        public void Send(object request, Type handlerType)
        {
            var genericHandlerType = handlerType.MakeGenericType(request.GetType());

            var handler = TinyIoCContainer.Current.Resolve(genericHandlerType);
            
            genericHandlerType.GetMethod("Handle").Invoke(handler, new[] { request });
        }
    }
}
