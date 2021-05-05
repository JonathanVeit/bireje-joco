using JoVei.Base.MessageSystem;
using BiReJeJoCo.Backend;

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

    public class OnLoadedLobbySceneMsg : BaseMessage
    {

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

    public class OnAddedPlayerMsg : Message<Player>
    {
        public OnAddedPlayerMsg(Player player) : base(player) { }
    }

    public class OnRemovedPlayerMsg : Message<Player>
    {
        public OnRemovedPlayerMsg(Player player) : base(player) { }
    }
    #endregion

    #region Game Related
    public class OnLoadedGameSceneMsg : BaseMessage
    {
    }

    public class OnPlayerCharacterSpawnedMsg : BaseMessage
    {
        
    }

    public class OnGameMenuOpenedMsg : BaseMessage
    { 
    }

    public class OnGameMenuClosedMsg : BaseMessage
    {
    }
    #endregion

    #region Player Inputs
    public class OnPlayerPressedTriggerMsg : BaseMessage
    {
        
    }
    #endregion
}
