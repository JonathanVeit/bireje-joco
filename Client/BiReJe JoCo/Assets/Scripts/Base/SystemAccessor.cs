using JoVei.Base;

namespace BiReJeJoCo
{
    /// <summary>
    /// Our system accessor with all additional systems that do not come from our code base
    /// </summary>
    public class SystemAccessor : BaseSystemAccessor
    {
        public PhotonWrapper photonWrapper => DIContainer.GetImplementationFor<PhotonWrapper>();
        public PhotonClient photonClient => DIContainer.GetImplementationFor<PhotonClient>();
    }
}