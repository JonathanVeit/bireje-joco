using BiReJeJoCo.Backend;
using BiReJeJoCo.Character;
using JoVei.Base;
using JoVei.Base.Helper;
using System;

namespace BiReJeJoCo.Audio
{
    public class GameMusicObserver : SystemAccessor, ITickable
    {
        #region Initialization
        public GameMusicObserver()
        {
            systemsLoader.OnAllSystemsLoaded += () => 
            {
                musicManager.Play(MusicState.BooUp, () =>
                {
                    musicManager.Play(MusicState.MainMenu);
                });
            };
            ConnectEvents();
        }

        private void ConnectEvents()
        {
            messageHub.RegisterReceiver<LoadedMainSceneMsg>(this, OnLoadedMainScene);
            messageHub.RegisterReceiver<LoadedLobbySceneMsg>(this, OnLoadedLobbyScene);
            messageHub.RegisterReceiver<LoadedGameSceneMsg>(this, OnLoadedGameScene);
        }
        #endregion

        private CharacterControllerSetup controllerSetup => GetControllerSetup();

        private Counter startChasingCounter;
        private Counter endChasingCounter;

        public void Tick(float deltaTime)
        {
            if (controllerSetup == null)
                return;

            switch (localPlayer.Role)
            {
                case PlayerRole.Hunter:
                    HandleHunterMatchMusic(controllerSetup);
                    break;

                case PlayerRole.Hunted:
                    HandleHuntedMatchMusic(controllerSetup);
                    break;
            }
        }

        private void HandleHunterMatchMusic(CharacterControllerSetup controllerSetup)
        {
            HandleChasingMusic(MusicState.HunterChasing, () =>
            {
                return controllerSetup.GetBehaviourAs<HunterBehaviour>().ShockMechanic.IsHittingHunted;
            });
        }
        private void HandleHuntedMatchMusic(CharacterControllerSetup controllerSetup)
        {
            HandleChasingMusic(MusicState.HuntedChasing, () => 
            {
                return controllerSetup.GetBehaviourAs<HuntedBehaviour>().ResistanceMechanic.IsDecreasing;
            });
        }

        private void HandleChasingMusic(MusicState chasingState, Func<bool> chasingCondition)
        {
            // should chase?
            if (chasingCondition())
            {
                // reset end chasing counter
                endChasingCounter.SetValue(0);

                // already chase 
                if (musicManager.CurrentState == chasingState)
                    return;

                // start chasing 
                startChasingCounter.CountUp(() =>
                {
                    musicManager.Play(chasingState);
                });
            }
            // should play default?
            else
            {
                // reset start chasing counter
                startChasingCounter.SetValue(0);

                // already chase 
                if (musicManager.CurrentState == MusicState.MatchDefault)
                    return;

                // start chasing 
                endChasingCounter.CountUp(() =>
                {
                    musicManager.Play(MusicState.MatchDefault);
                });
            }
        }
        
        #region Events     
        private void OnLoadedMainScene(LoadedMainSceneMsg msg)
        {
            if (musicManager.CurrentState != MusicState.Lobby)
                musicManager.Play(MusicState.Lobby);
        }
        private void OnLoadedLobbyScene(LoadedLobbySceneMsg msg)
        {
            if (musicManager.CurrentState != MusicState.Lobby)
                musicManager.Play(MusicState.Lobby);
        }
        private void OnLoadedGameScene(LoadedGameSceneMsg msg)
        {
            musicManager.Play(MusicState.MatchDefault);

            startChasingCounter = new Counter(musicManager.MusicConfig.StartChaseMusicDelay);
            endChasingCounter = new Counter(musicManager.MusicConfig.EndChaseMusicDelay);

            photonMessageHub.RegisterReceiver<FinishMatchPhoMsg>(this, OnMatchFinished);
            tickSystem.Register(this);
        }

        private void OnMatchFinished(PhotonMessage msg)
        {
            var castedMsg = msg as FinishMatchPhoMsg;

            if (castedMsg.result.winner == PlayerRole.Hunter)
            {
                musicManager.Play(MusicState.HunterWin, () => 
                {
                    musicManager.Play(MusicState.ResultScreen);
                });
            }
            else
            {
                musicManager.Play(MusicState.HuntedWins, () =>
                {
                    musicManager.Play(MusicState.ResultScreen);
                });
            }

            photonMessageHub.UnregisterReceiver(this);
            tickSystem.Unregister(this);
        }
        #endregion

        #region Helper
        private CharacterControllerSetup GetControllerSetup()
        {
            if (localPlayer.PlayerCharacter != null)
            {
                return localPlayer.PlayerCharacter.ControllerSetup;
            }

            return null;
        }
        #endregion
    }
}