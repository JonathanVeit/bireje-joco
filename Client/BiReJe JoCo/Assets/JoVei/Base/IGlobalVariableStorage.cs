using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JoVei.Base
{
    /// <summary>
    /// Stores variables between scenes 
    /// </summary>
    public interface IGlobalVariableStorage
    {
        TVar GetVar<TVar>(string name);
        void SetVar<TVar>(string name, TVar value);
        bool HasVar(string name);
        void DeleteVar(string name);
    }
}