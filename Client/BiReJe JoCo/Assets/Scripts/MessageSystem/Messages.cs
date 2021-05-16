using JoVei.Base.MessageSystem;
using BiReJeJoCo.Backend;
using BiReJeJoCo.Character;

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

    public class LoadedLobbySceneMsg : BaseMessage
    {

    }

    public class JoinLobbyFailedMsg : Message<string>
    {
        public JoinLobbyFailedMsg(string lobbyName) : base(lobbyName) { }
    }

    public class LeftLobbyMsg : BaseMessage
    {
    }

    public class PlayerJoinedLobbyMsg : Message<string>
    {
        public PlayerJoinedLobbyMsg(string playerId) : base(playerId) { }
    }

    public class PlayerLeftLobbyMsg : Message<string>
    {
        public PlayerLeftLobbyMsg(string playerId) : base(playerId) { }
    }

    public class AddedPlayerMsg : Message<Player>
    {
        public AddedPlayerMsg(Player player) : base(player) { }
    }

    public class RemovedPlayerMsg : Message<Player>
    {
        public RemovedPlayerMsg(Player player) : base(player) { }
    }

    public class HostSwitchedMsg : BaseMessage
    {
        
    }
    #endregion

    #region Game Related
    public class LoadedGameSceneMsg : BaseMessage
    {
    }

    public class PlayerCharacterSpawnedMsg : BaseMessage
    {
        
    }

    public class PauseMenuOpenedMsg : BaseMessage
    { 
    }

    public class PauseMenuClosedMsg : BaseMessage
    {
    }

    public class PPSSwitchMsg : BaseMessage 
    { 
    }

    public class BlockPlayerControlsMsg : BaseMessage 
    {
        public InputBlockState blockState;

        public BlockPlayerControlsMsg(InputBlockState blockState)
        {
            this.blockState = blockState;
        }
    }

    public class UnblockPlayerControlsMsg : BaseMessage 
    {
        public InputBlockState blockState;

        public UnblockPlayerControlsMsg(InputBlockState blockState)
        {
            this.blockState = blockState;
        }
    }

    public class HuntedScannedItemMsg : BaseMessage
    {
        public string itemId;

        public HuntedScannedItemMsg(string itemId)
        {
            this.itemId = itemId;
        }
    }

    public class ItemCollectedByPlayerMsg : BaseMessage
    {
        public string itemId;

        public ItemCollectedByPlayerMsg(string itemId) 
        {
            this.itemId = itemId;
        }
    }
    #endregion
}
