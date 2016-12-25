using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

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
            Validate(request);

            var genericHandlerType = handlerType.MakeGenericType(request.GetType());

            var handler = TinyIoCContainer.Current.Resolve(genericHandlerType);

            genericHandlerType.GetMethod("Handle").Invoke(handler, new[] { request });
        }

        public async Task SendAsync(object request, Type handlerType)
        {
            Validate(request);

            var genericHandlerType = handlerType.MakeGenericType(request.GetType());

            var handler = TinyIoCContainer.Current.Resolve(genericHandlerType);

            await (Task)genericHandlerType.GetMethod("HandleAsync").Invoke(handler, new[] { request });
        }

        public async Task TrySendAsync(object request, Type handlerType)
        {
            try
            {
                await SendAsync(request, handlerType);
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }
        }

        private void Validate(object request)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            var validatable = request as IValidatable;

            if (validatable != null && !validatable.IsValid())
                throw new ValidationException("Invalid item. Make sure all properties are valid.");
        }
    }
}
