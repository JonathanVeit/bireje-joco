using System;
using System.Linq;
using System.Collections.Generic;
using JoVei.Base.Helper;

namespace JoVei.Base.MessageSystem
{
    /// <summary>
    /// Implementation of IMessageHub
    /// </summary>
    public abstract class BaseMessageHub : BaseSystemAccessor, IMessageHub
    {
        /// <summary>
        /// All registered receivers with callback
        /// </summary>
        public Dictionary<Type, List<MessageReceiver>> RegisteredReceiver { get; protected set; }
        = new Dictionary<Type, List<MessageReceiver>>();

        /// <summary>
        /// Register a new receiver for a certain message
        /// </summary>
        public void RegisterReceiver<TMessage>(object receiver, Action<TMessage> callback)
            where TMessage : IMessage
        {
            var type = typeof(TMessage);
            
            // new message
            if (!RegisteredReceiver.ContainsKey(type))
            {
                RegisteredReceiver.Add(type, new List<MessageReceiver>());
            }

            // add receiver
            var msgReceiver = new MessageReceiver(receiver, callback);
            RegisteredReceiver[type].Add(msgReceiver);
        }


        /// <summary>
        /// Unregister a receiver from a certain message
        /// </summary>
        public void UnregisterReceiver<TMessage>(object receiver, Action<TMessage> callback)
            where TMessage : IMessage
        {
            var type = typeof(TMessage);

            // search receiver
            var collection = RegisteredReceiver[type];

            // remove 
            foreach (var curMsgReceiver in collection.ToArray())
            {
                RemoveFromCollection(collection, (msgReceiver) =>
                {
                    return msgReceiver.Receiver == receiver && msgReceiver.Callback.Equals(callback);
                });
            }
        }

        /// <summary>
        /// Unregister a receiver from all messages of type
        /// </summary>
        public void UnregisterReceiver<TMessage>(object receiver)
            where TMessage : IMessage
        {
            var type = typeof(TMessage);

            // all msg receivers
            var collection = RegisteredReceiver[type];

            // remove 
            RemoveFromCollection(collection, (msgReceiver) => 
            {
                return msgReceiver.Receiver.Equals(receiver);
            });
        }

        /// <summary>
        /// Unregister a receiver from all messages
        /// </summary>
        public void UnregisterReceiver(object receiver)
        {
            // store all collections
            var collections = RegisteredReceiver.Values.ToArray();

            // remove from all
            foreach (var curCollection in collections)
            {
                RemoveFromCollection(curCollection, (msgReceiver) => 
                {
                    return msgReceiver.Receiver.Equals(receiver);
                });
            }
        }


        /// <summary>
        /// Shout message to all registered receivers
        /// </summary>
        public void ShoutMessage<TMessage>(object sender, params object[] parameter)
            where TMessage : IMessage
        {
            // create message
            var msg = (TMessage)Activator.CreateInstance(typeof(TMessage), parameter);

            ShoutMessage(sender, msg);
        }

        /// <summary>
        /// Shout message to all registered receivers
        /// </summary>
        public void ShoutMessage<TMessage>(object sender, TMessage message)
            where TMessage : IMessage
        {
            var type = message.GetType();
            if (!RegisteredReceiver.ContainsKey(type)) return;

            // create log
            string log = string.Format("{0} shouts message <{1}> to ", sender, message.Name);

            // call
            foreach (var curMsgReceiver in RegisteredReceiver[type].ToArray())
            {
                (curMsgReceiver.Callback as Action<TMessage>)?.Invoke(message);
                log += string.Format("\n-> {0}", curMsgReceiver.Receiver.GetType().Name);
            }

            // print log
            if (WriteLog) DebugHelper.Print(UnityEngine.LogType.Log, log);
        }

        #region Abstract Member
        protected abstract bool WriteLog { get; }
        #endregion

        #region Helper
        protected void RemoveFromCollection(List<MessageReceiver> collection, Func<MessageReceiver, bool> condition)
        {
            foreach (var curReceiver in collection.ToArray())
            {
                if (condition(curReceiver))
                    collection.Remove(curReceiver);
            }
        }

        public struct MessageReceiver
        {
            public object Receiver { get; private set; }
            public object Callback { get; private set; }

            public MessageReceiver(object receiver, object callback)
            {
                Receiver = receiver;
                Callback = callback;
            }
        }
        #endregion
    }
}
