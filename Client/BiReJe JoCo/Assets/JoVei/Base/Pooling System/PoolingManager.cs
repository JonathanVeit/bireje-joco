using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using JoVei.Base.Helper;
using System.Collections;

namespace JoVei.Base.PoolingSystem
{
    /// <summary>
    /// Implementation of IPoolingManager as a System
    /// This implementation is not abstract and ready to use 
    /// </summary>
    public class PoolingManager : IPoolingManager, IInitializable
    {
        #region Initialization
        public IEnumerator Initialize(object[] parameters)
        {
            CreateRoot();
            DIContainer.RegisterImplementation<IPoolingManager>(this);
            yield return null;
        }

        public void CleanUp() { }
        #endregion

        /// <summary>
        /// All current instances per poolId (activated & deactivated)
        /// </summary>
        public Dictionary<string, List<IPoolable>> Instances { get; private set; }
            = new Dictionary<string, List<IPoolable>>();

        /// <summary>
        /// Root transform for poolable instances
        /// </summary>
        public Transform Root { get; private set; }

        #region Pooling (Generic)
        /// <summary>
        /// Returns instance from the pool
        /// </summary>
        public TPoolable PoolInstanceAs<TPoolable>(IPoolable prefab, Vector3 position, Quaternion rotation)
            where TPoolable : IPoolable
        {
            return (TPoolable)PoolInstance(prefab, position, rotation);
        }

        /// <summary>
        /// Returns instance from the pool
        /// </summary>
        public TPoolable PoolInstanceAs<TPoolable>(IPoolable prefab, Transform parent)
            where TPoolable : IPoolable
        {
            return (TPoolable)PoolInstance(prefab, parent);
        }

        /// <summary>
        /// Returns instance from the pool
        /// </summary>
        public TPoolable PoolInstanceAs<TPoolable>(IPoolable prefab)
            where TPoolable : IPoolable
        {
            return (TPoolable)PoolInstance(prefab);
        }
        #endregion

        #region Pooling (GameObject)
        /// <summary>
        /// Returns instance from the pool
        /// GameObject must have a component that implements IPoolable
        /// </summary>
        public GameObject PoolInstance(GameObject prefab, Vector3 position, Quaternion rotation)
        {
            var instance = PoolInstance(prefab);
            instance.transform.position = position;
            instance.transform.rotation = rotation;
            return instance;
        }

        /// <summary>
        /// Returns instance from the pool
        /// GameObject must have a component that implements IPoolable
        /// </summary>
        public GameObject PoolInstance(GameObject prefab, Transform parent)
        {
            var instance = PoolInstance(prefab);
            instance.transform.position = parent.position;
            instance.transform.rotation = parent.rotation;
            instance.transform.SetParent(parent);
            return instance;
        }

        /// <summary>
        /// Returns instance from the pool
        /// GameObject must have a component that implements IPoolable
        /// </summary>
        public GameObject PoolInstance(GameObject prefab)
        {
            var poolable = prefab.GetComponent<IPoolable>();
            if (poolable == null)
                throw new System.ArgumentException(string.Format("Prefab {0} is missing an IPoolable component", prefab.name));

            return PoolInstance(poolable).RootObject;
        }
        #endregion

        #region Pooling (as IPooable)
        /// <summary>
        /// Returns instance from the pool
        /// </summary>
        public IPoolable PoolInstance(IPoolable prefab, Vector3 position, Quaternion rotation)
        {
            var instance = PoolInstance(prefab);
            instance.RootObject.transform.position = position;
            instance.RootObject.transform.rotation = rotation;
            return instance;
        }

        /// <summary>
        /// Returns instance from the pool
        /// </summary>
        public IPoolable PoolInstance(IPoolable prefab, Transform parent)
        {
            var instance = PoolInstance(prefab);
            instance.RootObject.transform.position = parent.position;
            instance.RootObject.transform.rotation = parent.rotation;
            instance.RootObject.transform.SetParent(parent);
            return instance;
        }

        /// <summary>
        /// Returns instance from the pool
        /// </summary>
        public IPoolable PoolInstance(IPoolable prefab)
        {
            // catch null ref
            if (prefab == null)
            {
                throw new System.ArgumentException("Prefab to pool is null");    
            }

            // initialize pool
            if (!Instances.ContainsKey(prefab.PoolId))
            {
                Instances.Add(prefab.PoolId, new List<IPoolable>());

                // static pool?
                if (prefab.PoolType == PoolType.Static)
                {
                    for (int i = 0; i < prefab.PoolLimit; i++)
                    {
                        CreateInstance(prefab, false);
                    }
                }
            }

            // search for inactive instances
            for (int i = 0; i < Instances[prefab.PoolId].Count; i++)
            {
                // cur instance 
                var curInstance = Instances[prefab.PoolId][i];

                // discard null refs
                if (curInstance.Equals(null))
                {
                    Instances[prefab.PoolId].RemoveAt(i);
                    DebugHelper.PrintFormatted(LogType.Warning, "Removed null reference from pool {0}. Consider using ClearPool.", prefab.PoolId);
                    continue;
                }

                if (!IsActive(curInstance))
                {
                    // activeate and readd the instance
                    Activate(curInstance);
                    Instances[prefab.PoolId].RemoveAt(i);
                    Instances[prefab.PoolId].Add(curInstance);
                    return curInstance;
                }
            }

            // can new instances still be created? -> create and return
            if (Instances[prefab.PoolId].Count < prefab.PoolLimit || prefab.PoolType == PoolType.Limitless)
                return CreateInstance(prefab, true);

            // reuse oldest active instance 
            return Repool(prefab.PoolId, 0);
        }
        #endregion

        #region Return / Release / Destroy
        /// <summary>
        /// Deactivates and returns the instance to its pool
        /// </summary>
        public void Return(IPoolable instance)
        {
            // catch null ref
            if (instance == null)
            {
                DebugHelper.Print("Instance to Return is null");
                return;
            }

            Deactivate(instance);
        }

        /// <summary>
        /// Deactivates and returns the instance to its pool
        /// </summary>
        public void Return(GameObject instance)
        {
            // catch null ref
            if (instance == null)
            {
                DebugHelper.Print("Instance to return is null");
                return;
            }

            var poolable = instance.GetComponent<IPoolable>();
            if (poolable == null)
                throw new System.ArgumentException(string.Format("Instance {0} is missing an IPoolable component", instance.name));

            Return(poolable);
        }

        /// <summary>
        /// Deactivates and returns all instances to the pool
        /// </summary>
        public void ReturnAll(string poolId)
        {
            foreach (var curInstance in Instances[poolId].ToArray())
            {
                if (!IsActive(curInstance)) Return(curInstance);
            }
        }

        /// <summary>
        /// Releases the instance from the pooling system 
        /// It will no longer be pooled
        /// </summary>
        public void Release(IPoolable instance)
        {            
            // catch null ref
            if (instance == null)
            {
                DebugHelper.Print("Instance to release is null");
                return;
            }

            Instances[instance.PoolId].Remove(instance);
        }
        
        /// <summary>
        /// Releases the instance from the pooling system 
        /// It will no longer be pooled
        /// </summary>
        public void Release(GameObject instance)
        {
            // catch null ref
            if (instance == null)
            {
                DebugHelper.Print("Instance to release is null");
                return;

            }
            var poolable = instance.GetComponent<IPoolable>();
            if (poolable == null)
                throw new System.ArgumentException(string.Format("Instance {0} is missing an IPoolable component", instance.name));

            Release(poolable);
        }

        /// <summary>
        /// Releases all instances from the pooling system
        /// They will no longer be pooled
        /// </summary>
        public void ReleaseAll(string poolId)
        {
            foreach (var curInstance in Instances[poolId].ToArray())
            {
                Release(curInstance);
            }
        }

        /// <summary>
        /// Releases and destroys the instance, no matter if its active or not
        /// </summary>
        public void Destroy(IPoolable instance)
        {
            // catch null ref
            if (instance == null)
            {
                DebugHelper.Print("Instance to destroy is null");
                return;
            }

            Release(instance);
            Object.Destroy(instance.RootObject);
        }

        /// <summary>
        /// Releases and destroys the instance, no matter if its active or not
        /// </summary>
        public void Destroy(GameObject instance)
        {
            // catch null ref
            if (instance == null)
            {
                DebugHelper.Print("Instance to destroy is null");
                return;
            }

            var poolable = instance.GetComponent<IPoolable>();
            if (poolable == null)
                throw new System.ArgumentException(string.Format("Instance {0} is missing an IPoolable component", instance.name));

            Destroy(poolable);
        }

        /// <summary>
        /// Releases and destroys all instances from the pool, no matter if they are active or not
        /// </summary>
        public void DestroyAll(string poolId)
        {
            foreach (var curInstance in Instances[poolId].ToArray())
            {
                Destroy(curInstance);
            }
        }

        /// <summary>
        /// Is the prefab poolable?
        /// -> is implementation of IPoolable?
        /// -> GameObject has component that implements IPoolable?
        /// </summary>
        public bool IsPoolable(object prefab)
        {
            // as IPoolable
            if (prefab is IPoolable)
                return true;

            if (prefab is GameObject asGO)
                return asGO.GetComponent<IPoolable>() != null;

            // not poolable
            return false;
        }
        #endregion

        #region Helper
        private void CreateRoot()
        {
            Root = new GameObject("Pooling Root").transform;
            Object.DontDestroyOnLoad(Root);
        }

        private IPoolable CreateInstance(IPoolable prefab, bool andActivate)
        {
            // instantiate
            var newInstance = Object.Instantiate(prefab.RootObject).GetComponent<IPoolable>();

            // activate?
            if (andActivate)
            {
                Activate(newInstance);
            }
            else
            {
                Deactivate(newInstance);
            }

            Instances[prefab.PoolId].Add(newInstance);
            return newInstance;
        }

        private IPoolable Repool(string poolId, int index)
        {
            var instance = Instances[poolId][index];
            Deactivate(instance);
            Activate(instance);
            Instances[poolId].RemoveAt(index);
            Instances[poolId].Add(instance);
            return instance;
        }

        private void Activate(IPoolable instance)
        {
            instance.RootObject.SetActive(true);
            instance.RootObject.transform.SetParent(null);
            SceneManager.MoveGameObjectToScene(instance.RootObject, SceneManager.GetActiveScene());
        }

        private void Deactivate(IPoolable instance)
        {
            instance.RootObject.SetActive(false);
            instance.RootObject.transform.SetParent(Root);
        }

        private bool IsActive(IPoolable instance)
        {
            return !instance.RootObject.transform.IsChildOf(Root);
        }
        #endregion
    }
}