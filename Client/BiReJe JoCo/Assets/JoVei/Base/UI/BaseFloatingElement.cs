using UnityEngine;
using JoVei.Base.PoolingSystem;

namespace JoVei.Base.UI
{
    /// <summary>
    /// Implementation of IFloatingElement as PoolablePrefab
    /// </summary>
    public abstract class BaseFloatingElement : PoolablePrefab, IFloatingElement
    {
        #region Initializtion
        public IFloatingElementConfig Config { get; private set; }
        public Transform FloatyRoot { get { return transform; } }
        public void Initialize(IFloatingElementConfig config)
        {
            Config = config;
            this.enabled = true;
            OnInitialize();
        }

        public void Destroy()
        {
            OnDestroyed();
        }
        #endregion

        #region Behaviour
        /// <summary>
        /// Requests destroying the FloatingElement
        /// Instance will be returned to its pool
        /// </summary>
        public virtual void RequestDestroyFloaty() 
        {
            floatingManager.DestroyElement(this);
        }

        /// <summary>
        /// Requests releasing the FloatingElement
        /// Instance will no longer be controlled by the FloatingElementManager nor the PoolingManager
        /// </summary>
        public void RequestReleaseFloaty()
        {
            floatingManager.ReleaseElement(this);
        }

        /// <summary>
        /// Called right after beeing initialized
        /// </summary>
        protected virtual void OnInitialize() { }

        /// <summary>
        /// Called right before beeing destroyed
        /// </summary>
        protected virtual void OnDestroyed() { }
        #endregion
    }
}
