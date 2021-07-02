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
            musicManager.Play(MusicState.BooUp);
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
                case Backend.PlayerRole.Hunter:
                    HandleHunterMatchMusic(controllerSetup);
                    break;

                case Backend.PlayerRole.Hunted:
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
            musicManager.Play(MusicState.MainMenu);
            tickSystem.Unregister(this);
        }
        private void OnLoadedLobbyScene(LoadedLobbySceneMsg msg)
        {
            musicManager.Play(MusicState.Lobby);
            tickSystem.Unregister(this);
        }
        private void OnLoadedGameScene(LoadedGameSceneMsg msg)
        {
            musicManager.Play(MusicState.MatchDefault);
            tickSystem.Register(this);

            startChasingCounter = new Counter(musicManager.MusicConfig.StartChaseMusicDelay);
            endChasingCounter = new Counter(musicManager.MusicConfig.EndChaseMusicDelay);
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