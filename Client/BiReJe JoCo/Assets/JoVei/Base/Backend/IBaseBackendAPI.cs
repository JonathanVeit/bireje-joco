using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JoVei.Base.Backend
{
    /// <summary>
    /// Interface for classes that communicate with the backend service
    /// </summary>
    public interface IBaseBackendAPI
    {
        IEnumerator<bool> Login();
    }
}