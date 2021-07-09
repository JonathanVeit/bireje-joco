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

        public void Show(MatchResult result)
        {
            base.Show();

            rehostButton.gameObject.SetActive(localPlayer.IsHost);
            quitButton.gameObject.SetActive(localPlayer.IsHost);

            hunterResultLabel.text = result.winner == PlayerRole.Hunter ? "VICTORY" : "DEFEAT";
            huntedResultLabel.text = result.winner == PlayerRole.Hunted ? "VICTORY" : "DEFEAT";

            SpawnPlayer();
            UpdateCoralDisplay();

            Cursor.lockState = CursorLockMode.Confined;
            messageHub.ShoutMessage(this, new BlockPlayerControlsMsg(Character.InputBlockState.Menu));
        }

        private void SpawnPlayer() 
        {
            hunterList.Clear();
            huntedList.Clear();

            foreach (var curPlayer in playerManager.GetAllPlayer())
            {
                if (curPlayer.Role == PlayerRole.Hunter)
                {
                    hunterList.Add().Initialize(curPlayer.NickName);
                }
                else if (curPlayer.Role == PlayerRole.Hunted)
                {
                    huntedList.Add().Initialize(curPlayer.NickName);
                }
            }
        }

        private void UpdateCoralDisplay() 
        {
            var corals = collectablesManager.GetAllCollectablesAs<DestroyableCoral>(x => x is DestroyableCoral);

            var coralsPerLevel = new Dictionary<int, int>();
            int totalcorals = 0;
            foreach (var coral in corals)
            {
                var level = matchHandler.MatchConfig.mapConfig.GetFloorIndex(coral.transform.position);

                if (!coralsPerLevel.ContainsKey(level))
                    coralsPerLevel.Add(level, 0);
                
                coralsPerLevel[level]++;
                totalcorals++;
            }

            foreach (var entry in coralsPerLevel)
            {
                var level = entry.Key;
                var amount = entry.Value;
                var delta = amount / (float)totalcorals;

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
            (matchHandler as HostMatchHandler).CloseMatch(CloseMatchMode.LeaveLobby);
        }
        #endregion
    }
}