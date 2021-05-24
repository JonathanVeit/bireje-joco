using JoVei.Base.MessageSystem;
using BiReJeJoCo.Backend;
using BiReJeJoCo.Character;

namespace BiReJeJoCo
{
    #region Connection Releated
    public class ConnectedToPhotonMsg : BaseMessage
    {
    }

    public class DisconnectedFromPhotonMsg : Message<string>
    {
        public DisconnectedFromPhotonMsg(string reason) : base(reason) { }
    }

    public class ConnectedToPhotonMasterMsg : BaseMessage
    {
    }

    public class PhotonRoomListUpdatedMsg : BaseMessage
    {
        public Photon.Realtime.RoomInfo[] rooms;

        public PhotonRoomListUpdatedMsg(Photon.Realtime.RoomInfo[] lobbies)
        {
            this.rooms = lobbies;
        }
    }


    public class JoinedPhotonLobbyMsg : BaseMessage
    {
    }

    public class LeftPhotonLobbyMsg : BaseMessage
    {
    }
    #endregion

    #region Lobby Related
    public class LobbyListUpdatedMsg : BaseMessage
    {
        public LobbyInfo[] lobbies;

        public LobbyListUpdatedMsg(LobbyInfo[] lobbies)
        {
            this.lobbies = lobbies;
        }
    }

    public class LobbyCreatedMsg : BaseMessage
    {
    }

    public class FailedToHostLobbyMsg : Message<string>
    {
        public FailedToHostLobbyMsg(string reason) : base(reason) { }
    }

    public class JoinedLobbyMsg : Message<string>
    {
        public JoinedLobbyMsg(string lobbyId) : base(lobbyId) { }
    }

    public class LoadedLobbySceneMsg : BaseMessage
    {

    }

    public class JoinLobbyFailedMsg : Message<string>
    {
        public JoinLobbyFailedMsg(string lobbyId) : base(lobbyId) { }
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
