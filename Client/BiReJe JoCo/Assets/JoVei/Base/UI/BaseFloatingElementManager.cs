using System.Collections.Generic;
using UnityEngine;
using JoVei.Base.Helper;
using System.Collections;

namespace JoVei.Base.UI
{
    /// <summary>
    /// Base implementation of IFloaingElementManager as System
    /// </summary>
    public abstract class BaseFloatingElementManager : BaseSystemBehaviour, IFloatingElementManager, IInitializable
    {
        #region Initialization
        public IEnumerator Initialize(object[] parameters)
        {
            DIContainer.RegisterImplementation<IFloatingElementManager>(this);
            factory = CreateFactory();
            yield return null;
        }

        public void CleanUp() { }
        #endregion

        /// <summary>
        /// All IFloatingElements currently controlled by the manager
        /// </summary>
        public List<IFloatingElement> Elements { get; protected set; }
            = new List<IFloatingElement>();
        protected IFloatingElementFactory factory;
        
        #region Element Management
        /// <summary>
        /// Pool a new instance and initialize it 
        /// </summary>
        public virtual TElement GetElementAs<TElement>(IFloatingElementConfig config)
            where TElement : Object, IFloatingElement
        {
            return (TElement)GetElement(config);
        }

        /// <summary>
        /// Pool a new instance and initialize it 
        /// </summary>
        public virtual IFloatingElement GetElement(IFloatingElementConfig config)
        {
            // create 
            var newFloaty = factory.CreateElementForConfig(config);

            // set parent
            newFloaty.FloatyRoot.transform.SetParent(config.Parent);

            // initialize 
            newFloaty.Initialize(config);

            // register instance 
            Elements.Add(newFloaty);

            return newFloaty;
        }

        /// <summary>
        /// Releases the element from the FloatingElementsManager and the PoolingSystem
        /// </summary>
        public virtual void ReleaseElement(IFloatingElement element)
        {
            Elements.Remove(element);
        }

        /// <summary>
        /// Destroy element 
        /// </summary>
        public virtual void DestroyElement(IFloatingElement element)
        {
            Elements.Remove(element);
            element.Destroy();
            factory.DestroyElement(element);
        }

        /// <summary>
        /// Destroy element for target
        /// </summary>
        public virtual void DestroyElement(Transform target)
        {
            DestroyElement(Elements.Find(x => x.Config.Target == target));
        }

        /// <summary>
        /// Release all elements 
        /// </summary>
        public virtual void ReleaseAll()
        {
            foreach (var curElement in Elements.ToArray())
            {
                ReleaseElement(curElement);
            }
        }

        /// <summary>
        /// Destroy all elements
        /// </summary>
        public virtual void DestroyAll()
        {
            foreach (var curElement in Elements.ToArray())
            {
                DestroyElement(curElement);
            }
        }
        #endregion

        #region Behaviour
        protected abstract IFloatingElementFactory CreateFactory();

        protected virtual void Update()
        {
            for (int i = 0; i < Elements.Count; i++)
            {
                UpdateElement(Elements[i]);
            }
        }

        protected void UpdateElement(IFloatingElement element)
        {
            // check if valid 
            if (!ElementIsValid(element))
            {
                DebugHelper.Print(LogType.Warning, "Floating Element and/or its target have been destroyed. Please consider using the DestroyElement() method instead.");
                Elements.Remove(element);
                return;
            }

            if (Camera.main == null) return;

            // update position
            element.FloatyRoot.transform.position = Camera.main.WorldToScreenPoint(element.Config.Target.position) + (Vector3)element.Config.Offset;
        }
        #endregion

        #region Helper
        protected bool ElementIsValid(IFloatingElement element)
        {
            if (element == null || element.Config.Target == null)
                return false;

            return true;
        }
        #endregion
    }
}