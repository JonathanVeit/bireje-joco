using BiReJeJoCo.Backend;
using UnityEngine;

namespace BiReJeJoCo
{
    public class BorderReset : SystemBehaviour
    {
        public void OnCollisionEnter(Collision collision)
        {
            var observed = collision.gameObject.GetComponentsInChildren<IPlayerObserved>();

            if (observed.Length > 0)
            {
                ResetPlayerCharacter(observed[0].Owner);
            }
        }
        public void OnTriggerEnter(Collider collider)
        {
            var observed = collider.gameObject.GetComponentsInChildren<IPlayerObserved>();

            if (observed.Length > 0)
            {
                ResetPlayerCharacter(observed[0].Owner);
            }
        }

        private void ResetPlayerCharacter(Player player)
        {
            Vector3 pos = default;
            var scene = matchHandler.MatchConfig.matchScene;
            var config = MapConfigMapping.GetMapping().GetElementForKey(scene);

            switch (localPlayer.Role)
            {
                case PlayerRole.Hunted:
                    pos = config.GetHuntedSpawnPoint(config.GetRandomHuntedSpawnPointIndex());
                    break;

                case PlayerRole.Hunter:
                    pos = config.GetHuntedSpawnPoint(config.GetRandomHunterSpawnPointIndex());
                    break;
            }

            localPlayer.PlayerCharacter.ControllerSetup.CharacterRoot.transform.position = pos;
            localPlayer.PlayerCharacter.ControllerSetup.Mover.SetVelocity(Vector3.zero);
            localPlayer.PlayerCharacter.ControllerSetup.RigidBody.velocity = Vector3.zero;
        }
    }
}