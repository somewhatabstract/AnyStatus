using System;

namespace AnyStatus
{
    public class Mediator : IMediator
    {
        public void Send(object request, Type handlerType)
        {
            var genericHandlerType = handlerType.MakeGenericType(request.GetType());

            var handler = TinyIoCContainer.Current.Resolve(genericHandlerType);

            genericHandlerType.GetMethod("Handle").Invoke(handler, new[] { request });
        }
    }
}
