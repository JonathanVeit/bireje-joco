using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JoVei.Base.Helper;

namespace JoVei.Base.TickSystem
{
    /// <summary>
    /// Implementation of ITickSystem as system 
    /// Ticks are automatically called 
    /// </summary>
    public class TickSystem : BaseSystemBehaviour, ITickSystem, IInitializable
    {
        #region Initialization
        public IEnumerator Initialize(object[] parameters)
        {
            // add region entries
            AddTickUpdateTypes();

            // setup 
            var setups = Resources.LoadAll<TickSystemConfig>("");
            if (setups.Length > 0)
                Setup(setups[0]);
            else
                SetupDefault();

            DIContainer.RegisterImplementation<ITickSystem>(this);
            yield return null;
        }

        private void AddTickUpdateTypes()
        {
            foreach (TickUpdateType curType in System.Enum.GetValues(typeof(TickUpdateType)))
            {
                RegionsByType.Add(curType, new List<TickRegion>());
            }
        }

        private void Setup(TickSystemConfig config) 
        {
            // set time scale 
            TimeScale = config.GetTimeScale();

            // register default region
            var defaultDrawer = config.GetDefaultRegionDrawer();
            RegisterRegion(defaultDrawer.id, defaultDrawer.type, defaultDrawer.border, defaultDrawer.scale);
            DefaultRegionId = defaultDrawer.id;
             
            // register other regions
            foreach (var curDrawer in config.GetRegionDrawer())
            {
                RegisterRegion(curDrawer.id, curDrawer.type, curDrawer.border, curDrawer.scale);       
            }
        }

        private void SetupDefault() 
        {
            // set timescale
            TimeScale = 1;

            // register default region
            var config = new TickRegionConfig(TickUpdateType.Update, 0, 1);
            RegisterRegion("default", config);
            DefaultRegionId = "default";
        }

        public void CleanUp() { }
        #endregion

        /// <summary>
        /// Region id of the default region
        /// </summary>
        protected string DefaultRegionId { get; private set; }

        /// <summary>
        /// All registered tick regions 
        /// </summary>
        protected Dictionary<string, TickRegion> Regions { get; private set; }
            = new Dictionary<string, TickRegion>();

        /// <summary>
        /// All registered tick regions by their update type 
        /// </summary>
        protected Dictionary<TickUpdateType, List<TickRegion>> RegionsByType { get; private set; }
            = new Dictionary<TickUpdateType, List<TickRegion>>();
        
        /// <summary>
        /// Overall timescale for all regions 
        /// </summary>
        public float TimeScale { get; set; }

        #region Update
        // update 
        public void Update() { UpdateSystem(); }
        public virtual void UpdateSystem()
        {
            if (!IsReady) return;
            foreach (var curRegion in RegionsByType[TickUpdateType.Update])
                UpdateRegion(curRegion, Time.unscaledDeltaTime);
            foreach (var curRegion in RegionsByType[TickUpdateType.UpdateByFrame])
                UpdateRegionByFrame(curRegion, Time.unscaledDeltaTime);
        }

        // fixed update
        public void FixedUpdate() { FixedUpdateSystem(); }
        public virtual void FixedUpdateSystem()
        {
            if (!IsReady) return;
            foreach (var curRegion in RegionsByType[TickUpdateType.FixedUpdate])
                UpdateRegion(curRegion, Time.fixedUnscaledDeltaTime);
            foreach (var curRegion in RegionsByType[TickUpdateType.FixedUpdateByFrame])
                UpdateRegionByFrame(curRegion, Time.fixedUnscaledDeltaTime);
        }

        // late update 
        public void LateUpdate() { LateUpdateSystem(); }
        public virtual void LateUpdateSystem()
        {
            if (!IsReady) return;
            foreach (var curRegion in RegionsByType[TickUpdateType.LateUpdate])
                UpdateRegion(curRegion, Time.unscaledDeltaTime);
            foreach (var curRegion in RegionsByType[TickUpdateType.LateUpdateByFrame])
                UpdateRegionByFrame(curRegion, Time.unscaledDeltaTime);
        }

        /// <summary>
        /// Update a tick region with a given delta time 
        /// </summary>
        protected virtual void UpdateRegion(TickRegion region, float deltaTime) 
        {
            region.timeSinceTick += deltaTime;
            if (region.timeSinceTick < region.Config.Border) return;

            float scaledDeltaTime = region.timeSinceTick * region.Config.Scale;
            UpdateTickables(region.Tickables, scaledDeltaTime);
            region.timeSinceTick = 0;
        }

        /// <summary>
        /// Update a tick region with a given delta time framewise
        /// </summary>
        protected virtual void UpdateRegionByFrame(TickRegion region, float deltaTime)
        {
            region.timeSinceTick += deltaTime;
            region.framesSinceTick++;
            if (region.framesSinceTick < region.Config.Border) return;

            float scaledDeltaTime = region.timeSinceTick * region.Config.Scale;
            UpdateTickables(region.Tickables, scaledDeltaTime);
            region.timeSinceTick = 0;
            region.framesSinceTick = 0;
        }

        /// <summary>
        /// Tick list of tickables with delta time 
        /// </summary>
        protected virtual void UpdateTickables(List<ITickable> tickables, float deltaTime)
        {
            for (int i = 0; i < tickables.Count; i++) 
            {
                if (tickables[i] != null)
                    tickables[i].Tick(deltaTime);
            }
        }
        #endregion

        #region Register / Unregister
        /// <summary>
        /// Register a tickable in the default region (Unity Update)
        /// </summary>
        public virtual void Register(ITickable tickable)
        {
            Register(tickable, DefaultRegionId);
        }

        /// <summary>
        /// Register a tickable at a specific region
        /// </summary>
        public virtual void Register(ITickable tickable, string tickRegion)
        {
            // find region
            if (!Regions.TryGetValue(tickRegion, out TickRegion region))
            {
                DebugHelper.PrintFormatted(LogType.Error, "There is no region registered for id {0}", tickRegion);
                return;
            }

            region.Tickables.Add(tickable);
        }

        /// <summary>
        /// Remove a tickable from its region
        /// </summary>
        public virtual void Unregister(ITickable tickable)
        {
            foreach (var curRegion in Regions.Values)
            {
                if (curRegion.Tickables.Contains(tickable))
                {
                    curRegion.Tickables.Remove(tickable);
                    return;
                }
            }
        }

        /// <summary>
        /// Register a new tick region
        /// </summary>
        public virtual void RegisterRegion(string id, TickUpdateType updateType, float border, float scale)
        {
            var config = new TickRegionConfig(updateType, border, scale);
            RegisterRegion(id, config);
        }

        /// <summary>
        /// Register a new tick region
        /// </summary>
        public virtual void RegisterRegion(string id, ITickRegionConfig regionConfig)
        {
            if (Regions.ContainsKey(id))
            {
                DebugHelper.PrintFormatted(LogType.Error, "Ticksystem already contains region with id {0}", id);
                return;
            }

            // create and add region
            var region = new TickRegion(regionConfig as TickRegionConfig);

            Regions.Add(id, region);

            // add to RegionsByType 
            RegionsByType[region.Config.UpdateType].Add(region);
        }

        /// <summary>
        /// Unregister an existing tick region
        /// </summary>
        public virtual void UnregisterRegion(string id)
        {
            // find region
            if (!Regions.TryGetValue(id, out TickRegion region))
            {
                DebugHelper.PrintFormatted(LogType.Log, "There is no region registered for id {0}", id);
                return;
            }

            // remove from RegionsByType 
            RegionsByType[region.Config.UpdateType].Remove(region);

            // remove from Regions
            Regions.Remove(id);
        }
        #endregion

        protected class TickRegion 
        {
            public TickRegionConfig Config { get; private set; }
            public List<ITickable> Tickables { get; private set; }
            
            public float timeSinceTick;
            public float framesSinceTick;

            public TickRegion (TickRegionConfig config)
            {
                this.Config = config;
                Tickables = new List<ITickable>();
            }
        }
    }
}
