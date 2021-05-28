using BiReJeJoCo.Backend;

namespace BiReJeJoCo.Character
{
    public abstract class BaseBehaviour: SystemBehaviour, IPlayerObserved
    {
        public Player Owner => controller.Player;
        protected PlayerControlled controller;

        public void Initialize(PlayerControlled controller)
        {
            this.controller = controller;
            FindMechanics();
            OnBehaviourInitialized();
        }
        private void FindMechanics() 
        {
            foreach (var mechanic in GetComponentsInChildren<IBehaviourMechanic>())
            {
                mechanic.SetBehaviour(this);
            }
        }

        protected virtual void OnBehaviourInitialized() { }
    }
}