using System;
using System.Linq;
using System.Collections.Generic;
using JoVei.Base;
using JoVei.Base.Helper;
using Photon.Pun;
using UnityEngine;

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
        private PhotonView photonView;
        private PhotonMessageFactory messageFactory;

        #region Initialization
        private void Awake()
        {
            DIContainer.RegisterImplementation<PhotonMessageHub>(this);
            DontDestroyOnLoad(this);
            
            photonView = GetComponent<PhotonView>();
            messageFactory = new PhotonMessageFactory();
        }

        protected override void OnBeforeDestroy()
        {
            DIContainer.UnregisterImplementation<PhotonMessageHub>();
        }
        #endregion

        /// <summary>
        /// All registered receivers with callback
        /// </summary>
        public Dictionary<byte, List<MessageReceiver>> RegisteredReceiver { get; protected set; }
        = new Dictionary<byte, List<MessageReceiver>>();


        /// <summary>
        /// Register a new receiver for a certain message
        /// </summary>
        public void RegisterReceiver<TMessage>(object receiver, Action<PhotonMessage> callback)
            where TMessage : PhotonMessage, new()
        {
            var code = messageFactory.GetMessageCode(new TMessage());

            // new message
            if (!RegisteredReceiver.ContainsKey(code))
            {
                RegisteredReceiver.Add(code, new List<MessageReceiver>());
            }

            // add receiver
            var msgReceiver = new MessageReceiver(receiver, callback);
            RegisteredReceiver[code].Add(msgReceiver);
        }

        /// <summary>
        /// Unregister a receiver from a certain message
        /// </summary>
        public void UnregisterReceiver<TMessage>(object receiver, Action<PhotonMessage> callback)
            where TMessage : PhotonMessage, new()
        {
            var code = messageFactory.GetMessageCode(new TMessage());

            // search receiver
            var collection = RegisteredReceiver[code].FindAll(x => x.Receiver == receiver);

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
            where TMessage : PhotonMessage, new()
        {
            var code = messageFactory.GetMessageCode(new TMessage());

            // all msg receivers
            var collection = RegisteredReceiver[code];

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
            var code = messageFactory.SerializeMessage(message, out string serializedMessage);
            photonView.RPC("ShoutPhotonMessage", (RpcTarget) messageTarget, serializedMessage, code);
        }


        /// <summary>
        /// Shout message to a specific player
        /// </summary>
        public void ShoutMessage<TMessage>(Player player, params object[] parameter)
            where TMessage : PhotonMessage
        {
            // create message
            var msg = (TMessage)Activator.CreateInstance(typeof(TMessage), parameter);

            ShoutMessage(msg, player);
        }

        /// <summary>
        /// Shout message to a specific player
        /// </summary>
        public void ShoutMessage<TMessage>(TMessage message, Player player)
            where TMessage : PhotonMessage
        {
            var code = messageFactory.SerializeMessage(message, out string serializedMessage);
            photonView.RPC("ShoutPhotonMessage", player.PhotonPlayer, serializedMessage, code);
        }


        [PunRPC]
        private void ShoutPhotonMessage(string serializedMessage, byte code, PhotonMessageInfo info)
        {
            if (!RegisteredReceiver.ContainsKey(code)) return;

            var message = messageFactory.DeserializeMessage(serializedMessage, code);
            string log = string.Format("{0} shouts <color=blue>message</color> <{1}> to ", info.Sender.NickName, message.GetType().Name);

            foreach (var curMsgReceiver in RegisteredReceiver[code])
            {
                curMsgReceiver.Callback?.Invoke(message);
                log += string.Format("\n-> {0}", curMsgReceiver.Receiver.GetType().Name);
            }

            if (globalVariables.GetVar<bool>("debug_mode")) DebugHelper.Print(LogType.Log, log);
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