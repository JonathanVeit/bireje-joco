using System.Collections;
using System.Collections.Generic;
using JoVei.Base.Helper;
using UnityEngine;

namespace JoVei.Base.Data
{
    /// <summary>
    /// Implementation of IProfileHandler as system
    /// </summary>
    public class ProfileHandler : IProfileHandler, IInitializable
    {
        #region Initialization
        public virtual IEnumerator Initialize(object[] parameters)
        {
            profiles = new Dictionary<string, IPlayerProfile>();
            DIContainer.RegisterImplementation<IProfileHandler>(this);
            yield return null;
        }

        public virtual void CleanUp() { }
        #endregion

        // registered profiles 
        protected Dictionary<string, IPlayerProfile> profiles;
        protected string localPlayerId = null;

        /// <summary>
        /// Register new profile 
        /// </summary>
        public virtual void RegisterProfile(IPlayerProfile profile)
        {
            if (profiles.ContainsKey(profile.Id))
            {
                DebugHelper.PrintFormatted(LogType.Warning, "Profile with Id {0} is already registered", profile.Id);
                return;
            }

            profiles.Add(profile.Id, profile);
        }

        /// <summary>
        /// Register new profile as local player profile 
        /// </summary>
        public void RegisterLocalProfile(IPlayerProfile profile)
        {
            RegisterProfile(profile);
            localPlayerId = profile.Id;
        }

        /// <summary>
        /// Unregister profile
        /// </summary>
        public virtual void UnregisterProfile(IPlayerProfile profile)
        {
            if (!profiles.ContainsKey(profile.Id))
            {
                DebugHelper.PrintFormatted(LogType.Warning, "There is no profile with Id {0} registered", profile.Id);
                return;
            }

            if (profile.Id == localPlayerId) localPlayerId = null;
            profiles.Remove(profile.Id);
        }

        /// <summary>
        /// Returns registered profile for Id 
        /// </summary>
        public virtual IPlayerProfile GetProfile(string Id)
        {
            if (!profiles.ContainsKey(Id))
            {
                DebugHelper.PrintFormatted(LogType.Error, "There is no profile registered for Id {0}", Id);
                return null;
            }

            return profiles[Id];
        }

        /// <summary>
        /// Returns the registered local player profile
        /// </summary>
        /// <returns></returns>
        public IPlayerProfile GetLocalPlayerProfile()
        {
            return GetProfile(localPlayerId);
        }

        /// <summary>
        /// Returns registered profile for Id 
        /// </summary>
        public virtual TProfile GetProfileAs<TProfile>(string Id)
            where TProfile : class, IPlayerProfile
        {
            return (TProfile) GetProfile(Id);
        }
    }
}
