using BiReJeJoCo.Backend;
using BiReJeJoCo.UI;

namespace BiReJeJoCo.Character
{
    public interface IBehaviourMechanic
    {
        public void SetBehaviour(BaseBehaviour behaviour);
    }

    public abstract class BaseBehaviourMechanic<TBehaviour> : SystemBehaviour, IBehaviourMechanic, IPlayerObserved
        where TBehaviour : BaseBehaviour
    {
        public Player Owner => Controller.Player;
        protected PlayerControlled Controller { get; private set; }
        protected TBehaviour Behaviour { get; private set; }

        protected GameUI gameUI => uiManager.GetInstanceOf<GameUI>();

        public void Initialize(PlayerControlled controller)
        {
            Controller = controller;

            if (Owner.IsLocalPlayer)
                OnInitializeLocal();
            else
                OnInitializeRemote();
        }
        public void SetBehaviour(BaseBehaviour behaviour)
        {
            Behaviour = (TBehaviour) behaviour;
        }

        protected virtual void OnInitializeLocal() { }
        protected virtual void OnInitializeRemote() { }
    }
}