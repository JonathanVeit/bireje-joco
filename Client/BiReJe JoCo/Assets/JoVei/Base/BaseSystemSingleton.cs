using UnityEngine;

namespace JoVei.Base
{
    /// <summary>
    /// Singleton that instantiates itself if neccessary 
    /// </summary>
    public abstract class BaseSystemSingleton<T> : BaseSystemBehaviour where T : MonoBehaviour
    {
        protected static T _instance;

        public static T Instance
        {
            get
            {
                if (_instance)
                {
                    return _instance;
                }
                else
                {
                    _instance = FindObjectOfType<T>();
                    if (!_instance)
                        _instance = new GameObject("(Singleton) " + typeof(T).Name).AddComponent<T>();
                    return _instance;
                }
            }
        }

        protected virtual void Awake()
        {
            if (_instance == null)
            {
                _instance = this as T;
            }
            else
            {
                Destroy(this.gameObject);
            }
        }
    }
}