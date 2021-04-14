﻿using JoVei.Base;

namespace BiReJeJoCo
{    
    /// <summary>
    /// Our system behaviour with all additional systems that do not come from our code base
    /// </summary>
    public class SystemBehaviour : BaseSystemBehaviour
    {
        public PhotonWrapper photonWrapper => DIContainer.GetImplementationFor<PhotonWrapper>();
        public PhotonClient photonClient => DIContainer.GetImplementationFor<PhotonClient>();
    }
}