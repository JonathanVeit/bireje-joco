using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using JoVei.Base.Helper;
using System.Collections;

namespace JoVei.Base.PoolingSystem
{
    /// <summary>
    /// Dynamic = only instaniate if needed 
    /// Static  = instantiate once till limit is reached 
    /// Limitless = no repooling 
    /// </summary>
    public enum PoolType
    {
        Dynamic = 1,
        Static = 2,
        Limitless = 3,
    }
}