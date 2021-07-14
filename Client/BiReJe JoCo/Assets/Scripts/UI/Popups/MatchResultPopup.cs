using BiReJeJoCo.Backend;
using JoVei.Base.UI;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BiReJeJoCo.UI
{
    public class MatchResultPopup : Popup
    {
        [Header("UI Elements")]
        [SerializeField] Text hunterResultLabel;
        [SerializeField] Text huntedResultLabel;
        [SerializeField] Button rehostButton;
        [SerializeField] Button quitButton;
        [SerializeField] UIList<ResultPlayerEntry> hunterList;
        [SerializeField] UIList<ResultPlayerEntry> huntedList;
        [SerializeField] UIBarHandler[] coralBars;

        protected override void OnSystemsInitialized()
        {
            base.OnSystemsInitialized();
        }
        protected override void OnBeforeDestroy()
        {
            base.OnBeforeDestroy();
            messageHub.UnregisterReceiver(this);
        }

        public void Show(MatchResult result)
        {
            base.Show();

            rehostButton.gameObject.SetActive(localPlayer.IsHost);

            hunterResultLabel.text = result.winner == PlayerRole.Hunter ? "VICTORY" : "DEFEAT";
            huntedResultLabel.text = result.winner == PlayerRole.Hunted ? "VICTORY" : "DEFEAT";

            SpawnPlayer();
            UpdateCoralDisplay();

            Cursor.lockState = CursorLockMode.Confined;
            messageHub.ShoutMessage(this, new BlockPlayerControlsMsg(Character.InputBlockState.Menu));
            messageHub.RegisterReceiver<PlayerLeftLobbyMsg>(this, OnPlayerLeftLobby);
        }

        private void SpawnPlayer() 
        {
            hunterList.Clear();
            huntedList.Clear();

            foreach (var curPlayer in playerManager.GetAllPlayer())
            {
                if (curPlayer.Role == PlayerRole.Hunter)
                {
                    hunterList.Add().Initialize(curPlayer);
                }
                else if (curPlayer.Role == PlayerRole.Hunted)
                {
                    huntedList.Add().Initialize(curPlayer);
                }
            }
        }

        private void UpdateCoralDisplay() 
        {
            var corals = collectablesManager.GetAllCollectablesAs<DestroyableCoral>(x => x is DestroyableCoral);

            var coralsPerLevel = new Dictionary<int, int>();
            foreach (var coral in corals)
            {
                var level = matchHandler.MatchConfig.mapConfig.GetFloorIndex(coral.transform.position);

                if (!coralsPerLevel.ContainsKey(level))
                    coralsPerLevel.Add(level, 0);
                
                coralsPerLevel[level]++;
            }

            foreach (var entry in coralsPerLevel)
            {
                var level = entry.Key;
                var amount = entry.Value;
                var delta = amount / (float)matchHandler.MatchConfig.Mode.maxCorals;

                if (level - 1 > coralBars.Length)
                {
                    Debug.LogError($"MatchResultPopup has no UIBarhandler for level {level}.");
                    break;
                }

                coralBars[level].SetValue(delta);
            }
        }

        public override void Hide()
        {
            base.Hide();

            Cursor.lockState = CursorLockMode.Locked;
            messageHub.ShoutMessage(this, new UnblockPlayerControlsMsg(Character.InputBlockState.Free));
        }

        #region UI Input
        public void Rehost() 
        {
            (matchHandler as HostMatchHandler).CloseMatch(CloseMatchMode.ReturnToLobby);
        }

        public void Quit()
        {
            if (localPlayer.IsHost)
            {
                (matchHandler as HostMatchHandler).CloseMatch(CloseMatchMode.LeaveLobby);
            }
            else
            {
                matchHandler.LeaveLobby();
            }
        }
        #endregion

        #region Events
        private void OnPlayerLeftLobby(PlayerLeftLobbyMsg msg)
        {
            for (int i = 0; i < hunterList.Count; i++)
            {
                if (hunterList[i].DisplayedPlayer.Id == msg.Param1)
                {
                    huntedList[i].SetLeaved();
                    return;
                }
            }

            for (int i = 0; i < huntedList.Count; i++)
            {
                if (hunterList[i].DisplayedPlayer.Id == msg.Param1)
                {
                    huntedList[i].SetLeaved();
                    return;
                }
            }
        }
        #endregion
    }
}