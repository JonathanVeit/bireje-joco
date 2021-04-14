using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace JoVei.Base
{
    /// <summary>
    /// Provides access to instances of a class and its subclasses
    /// </summary>
    public class InstanceProvider<TBase> 
        where TBase : Component
    {
        protected static Dictionary<string, List<TBase>> Instances { get; set; } = new Dictionary<string, List<TBase>>();

        protected static void RegisterInstance(TBase instance)
        {
            string name = instance.GetType().FullName;

            if (!Instances.ContainsKey(name))
                Instances.Add(name, new List<TBase>());
            else
                Instances[name].RemoveAll(InstanceIsNull);

            if (!Instances[name].Contains(instance))
                Instances[name].Add (instance);
        }

        protected static void RemoveInstance(TBase instance)
        {
            if (!Instances.ContainsKey(instance.GetType().FullName))
                return;

            Instances[instance.GetType().FullName].Remove(instance);
        }

        public static TElement GetInstanceOf<TElement>() 
            where TElement : TBase
        {
            return GetInstancesOf<TElement>()[0];
        }

        public static TElement[] GetInstancesOf<TElement>() 
            where TElement : TBase
        {
            string name = typeof(TElement).FullName;

            if (Instances.ContainsKey(name)) Instances[name].RemoveAll(InstanceIsNull);
            if (!Instances.ContainsKey(name) || Instances[name].Count == 0)
            {
                Refresh();

                if (!Instances.ContainsKey(name))
                {
                    throw new NullReferenceException(string.Format("No instance found for ui element {0}", name));
                }
            }

            var result = new TElement[Instances[name].Count];
            for (int i = 0; i < Instances[name].Count; i++)
            {
                result[i] = Instances[name][i] as TElement;
            }

            return result;
        }

        #region Helper        
        protected static void Refresh() 
        {
            // find all elements 
            var allInstances = UnityEngine.Object.FindObjectsOfType<TBase>(true).ToList();

            // register new elements
            foreach (var curInstance in allInstances)
            {
                RegisterInstance(curInstance);
            }
        }

        private static bool InstanceIsNull(TBase instance)
        {
            return instance == null;
        }
        #endregion
    }
}