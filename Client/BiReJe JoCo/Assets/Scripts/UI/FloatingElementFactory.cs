using UnityEngine;
using JoVei.Base.UI;
using JoVei.Base.PoolingSystem;

namespace BiReJeJoCo
{
    /// <summary>
    /// Our implementation for the FloatingElementFactory
    /// </summary>
    public class FloatingElementFactory : SystemAccessor, IFloatingElementFactory
    {
        public IFloatingElement CreateElementForConfig(IFloatingElementConfig config)
        {
            // prefab from mapping
            var prefab = FloatingElementMapping.GetMapping().GetElementForKey(config.PrefabId);

            // instance as poolable
            if (poolingManager.IsPoolable(prefab))
                return poolingManager.PoolInstance(prefab).GetComponent<IFloatingElement>();

            // instantiate
            return Object.Instantiate(prefab).GetComponent<IFloatingElement>(); ;
        }

        public void DestroyElement(IFloatingElement element)
        {
            // return to pool
            if (poolingManager.IsPoolable(element))
            {
                poolingManager.Return(element as IPoolable);
                return;
            }
            
            // destroy
            Object.Destroy(element as Object);
        }
    }
}
