using System;

namespace JoVei.Base.MessageSystem
{
    public interface IMessageHub
    {
        void RegisterReceiver<TMessage>(object receiver, Action<TMessage> callback) // register for certain message
            where TMessage : IMessage;

        void UnregisterReceiver<TMessage>(object receiver, Action<TMessage> callback) // unregister specific callback
             where TMessage : IMessage;

        void UnregisterReceiver<TMessage>(object receiver) // unregister from all messages of type
            where TMessage : IMessage;

        void UnregisterReceiver(object receiver); // unregister from all messages

        void ShoutMessage<TMessage>(object sender, params object[] parameter) // shout generic with parameters
             where TMessage : IMessage;

        void ShoutMessage<TMessage>(object sender, TMessage message) // shout specific with message
            where TMessage : IMessage;
    }
}
