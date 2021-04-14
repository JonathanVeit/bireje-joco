using JoVei.Base.Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JoVei.Base.Backend
{
    /// <summary>
    /// Interaface for classes that manage the communication with the backend api
    /// </summary>
    public interface IBackendManager : IInitializable
    {
        IBaseBackendAPI backendAPI { get; set; }

        void LoadAPI();
    }
}
