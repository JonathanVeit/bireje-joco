using JoVei.Base.MessageSystem;

namespace BiReJeJoCo
{
    #region Connection Releated
    public class OnConnectedToPhotonMsg : BaseMessage
    {
    }

    public class OnDisconnectedFromPhotonMsg : Message<string>
    {
        public OnDisconnectedFromPhotonMsg(string reason) : base(reason) { }
    }

    public class OnConnectedToPhotonMasterMsg : BaseMessage
    {
    }

    public class OnJoinedPhotonLobbyMsg : BaseMessage
    {
    }

    public class OnLeftPhotonLobbyMsg : BaseMessage
    {
    }
    #endregion

    #region Lobby Related
    public class OnLobbyCreatedMsg : BaseMessage
    {
    }

    public class OnFailedToHostLobbyMsg : Message<string>
    {
        public OnFailedToHostLobbyMsg(string reason) : base(reason) { }
    }

    public class OnJoinedLobbyMsg : Message<string>
    {
        public OnJoinedLobbyMsg(string lobbyName) : base(lobbyName) { }
    }

    public class OnJoinLobbyFailedMsg : Message<string>
    {
        public OnJoinLobbyFailedMsg(string lobbyName) : base(lobbyName) { }
    }

    public class OnLeftLobbyMsg : BaseMessage
    {
    }

    public class OnPlayerJoinedLobbyMsg : Message<string>
    {
        public OnPlayerJoinedLobbyMsg(string playerId) : base(playerId) { }
    }

    public class OnPlayerLeftLobbyMsg : Message<string>
    {
        public OnPlayerLeftLobbyMsg(string playerId) : base(playerId) { }
    }
    #endregion

    public class OnLoadedGameMsg : BaseMessage
    {
    }
}
