using System;

namespace AnyStatus
{
    public interface IMediator
    {
        void Send(object request, Type handlerType);
    }
}