using BiReJeJoCo.Backend;

namespace BiReJeJoCo.Character
{
    public abstract class BaseBehaviour : TickBehaviour, IPlayerObserved
    {
        public Player Owner => controller.Player;
        protected PlayerControlled controller;

        public void Initialize(PlayerControlled controller)
        {
            this.controller = controller;
            OnBehaviourInitialized();
        }
        protected virtual void OnBehaviourInitialized() { }
    }
}