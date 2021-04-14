namespace JoVei.Base.Data
{
    /// <summary>
    /// Interface for playerprofiles
    /// </summary>
    public interface IPlayerProfile
    {
        public string Id { get; } // unique
        public string Name { get; } // not unique
    }

    /// <summary>
    /// Base implementation of IPlayerProfile
    /// Contains a generic data center and registers itself the profile handler 
    /// </summary>
    public abstract class BasePlayerProfile<TPlayerData, TDataCenter> : BaseSystemAccessor, IPlayerProfile
        where TPlayerData : System.Enum
        where TDataCenter : DataCenter<TPlayerData>
    {
        public abstract string Name { get; }
        public abstract string Id { get; }

        protected TDataCenter dataCenter;

        /// <summary>
        /// Setup with existing data center
        /// </summary>
        public virtual void Setup (TDataCenter dataCenter) 
        {
            this.dataCenter = dataCenter;
            RegisterSelf();
        }

        /// <summary>
        /// Auto register at implemented profile handler 
        /// </summary>
        protected virtual void RegisterSelf() 
        {
            profileHandler.RegisterProfile(this);
        }
    }
}
