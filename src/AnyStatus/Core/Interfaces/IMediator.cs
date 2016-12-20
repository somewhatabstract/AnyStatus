using System;

namespace AnyStatus
{
    public interface IMediator
    {
        void Send(object request);

        void TrySend(object request);

        void Send(object request, Type handlerType);

        void TrySend(object request, Type handlerType);
    }
}