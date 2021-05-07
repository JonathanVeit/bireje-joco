using Photon.Pun;

namespace BiReJeJoCo.Backend
{
    public interface IPlayerObserved
    {
        Player Owner { get; }
        void Initialize(PlayerControlled controller);
    }
}