using UnityEngine;

namespace BiReJeJoCo
{
    public class MatchSetup : SystemBehaviour
    {
        private void Start()
        {
            GameObject matchHandlerPrefab;
            if (localPlayer.IsHost)
                matchHandlerPrefab = MatchPrefabMapping.GetMapping().GetElementForKey("host_match_handler");
            else
                matchHandlerPrefab = MatchPrefabMapping.GetMapping().GetElementForKey("match_handler");

            Instantiate(matchHandlerPrefab);
        }
    }
}