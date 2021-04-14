using System.Collections;
using System.Collections.Generic;

namespace JoVei.Base
{
    /// <summary>
    /// Stores variables between scenes 
    /// </summary>
    public class BaseGlobalVariableStorage : IGlobalVariableStorage, IInitializable
    {
        protected Dictionary<string, object> variables = new Dictionary<string, object>();

        public virtual IEnumerator Initialize(object[] parameters)
        {
            DIContainer.RegisterImplementation<IGlobalVariableStorage>(this);
            yield return null;
        }

        public virtual TVar GetVar<TVar>(string name)
        {
            return (TVar) variables[name];
        }

        public virtual void SetVar<TVar>(string name, TVar value)
        {
            if (!variables.ContainsKey(name))
            {
                variables.Add(name, value);
            }
            else
            {
                variables[name] = value;
            }
        }

        public virtual bool HasVar(string name)
        {
            return variables.ContainsKey(name);
        }

        public virtual void DeleteVar(string name)
        {
            variables.Remove(name);
        }

        public void CleanUp()
        {
        }
    }
}