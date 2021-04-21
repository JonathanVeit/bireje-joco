using System;
using System.Linq;
using System.Collections.Generic;
using JoVei.Base;
using JoVei.Base.Helper;
using Photon.Pun;
using UnityEngine;
using Newtonsoft.Json;

namespace BiReJeJoCo.Backend
{
    public enum PhotonMessageTarget
    {
        All,
        Others,
        MasterClient,
        AllBuffered,
        OthersBuffered,
        AllViaServer,
        AllBufferedViaServer
    }

    [RequireComponent(typeof(PhotonView))]
    public class PhotonMessageHub : SystemBehaviour 
    {
        private void Awake()
        {
            DIContainer.RegisterImplementation<PhotonMessageHub>(this);
            DontDestroyOnLoad(this);
            photonView = GetComponent<PhotonView>();
        }

        private PhotonView photonView;

        /// <summary>
        /// All registered receivers with callback
        /// </summary>
        public Dictionary<string, List<MessageReceiver>> RegisteredReceiver { get; protected set; }
        = new Dictionary<string, List<MessageReceiver>>();

        /// <summary>
        /// Register a new receiver for a certain message
        /// </summary>
        public void RegisterReceiver<TMessage>(object receiver, Action<PhotonMessage> callback)
            where TMessage : PhotonMessage
        {
            var type = typeof(TMessage).ToString();

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
        public void UnregisterReceiver<TMessage>(object receiver, Action<PhotonMessage> callback)
            where TMessage : PhotonMessage
        {
            var type = typeof(TMessage).ToString();

            // search receiver
            var collection = RegisteredReceiver[type].FindAll(x => x.Receiver == receiver);

            // remove 
            foreach (var curMsgReceiver in collection.ToArray())
            {
                RemoveFromCollection(collection, (msgReceiver) =>
                {
                    return msgReceiver.Callback.Equals(callback);
                });
            }
        }

        /// <summary>
        /// Unregister a receiver from all messages of type
        /// </summary>
        public void UnregisterReceiver<TMessage>(object receiver)
            where TMessage : PhotonMessage
        {
            var type = typeof(TMessage).ToString();

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
        public void ShoutMessage<TMessage>(PhotonMessageTarget messageTarget, params object[] parameter)
            where TMessage : PhotonMessage
        {
            // create message
            var msg = (TMessage)Activator.CreateInstance(typeof(TMessage), parameter);

            ShoutMessage(msg, messageTarget);
        }

        /// <summary>
        /// Shout message to all registered receivers
        /// </summary>
        public void ShoutMessage<TMessage>(TMessage message, PhotonMessageTarget messageTarget)
            where TMessage : PhotonMessage
        {
            var type = message.GetType().ToString();
            var serializedMessage = JsonConvert.SerializeObject(message, new JsonSerializerSettings() { TypeNameHandling = TypeNameHandling.Auto });

            photonView.RPC("ShoutPhotonMessage", (RpcTarget) messageTarget, type, serializedMessage, photonConnectionWrapper.NickName);
        }

        [PunRPC]
        private void ShoutPhotonMessage(string messageType, string serializedMessage, string senderNick)
        {
            if (!RegisteredReceiver.ContainsKey(messageType)) return;

            // create log
            string log = string.Format("{0} shouts <color=blue>message</color> <{1}> to ", senderNick, messageType);

            var message = JsonConvert.DeserializeObject<PhotonMessage> (serializedMessage, new JsonSerializerSettings() { TypeNameHandling = TypeNameHandling.Auto });

            // call
            foreach (var curMsgReceiver in RegisteredReceiver[messageType])
            {
                curMsgReceiver.Callback?.Invoke(message);
                log += string.Format("\n-> {0}", curMsgReceiver.Receiver.GetType().Name);
            }

            if (globalVariables.GetVar<bool>("debug_mode")) DebugHelper.Print(LogType.Log, log);
        }

        protected override void OnBeforeDestroy()
        {
            DIContainer.UnregisterImplementation<PhotonMessageHub>();
        }

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
            public Action<PhotonMessage> Callback { get; private set; }

            public MessageReceiver(object receiver, Action<PhotonMessage> callback)
            {
                Receiver = receiver;
                Callback = callback;
            }
        }
        #endregion
    }
}