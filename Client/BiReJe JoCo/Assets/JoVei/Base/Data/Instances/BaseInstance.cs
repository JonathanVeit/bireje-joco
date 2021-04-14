using Newtonsoft.Json;

namespace JoVei.Base.Data
{
    /// <summary>
    /// Serializable data representing an instance of a Progressive Base Data
    /// This object should be serialized and saved as progress e.g. of a player
    /// </summary>
    public abstract class BaseInstance<TBaseData> : BaseSystemAccessor, IInstance, ILevelable
        where TBaseData : ProgressiveBaseData<TBaseData>, new()
    {
        [JsonProperty]
        protected string BaseDataId { get; private set; }
        [JsonProperty]
        public string InstanceId { get; protected set; }
        [JsonProperty]
        public int Level { get; protected set; }

        /// <summary>
        /// Base Data depending on the level
        /// </summary>
        [JsonIgnore] public TBaseData Data 
        { 
            get
            {
                if (_data == null)
                    gameDataManager.GetDataForElement(BaseDataId, out _data);
                return _data[Level];
            } 
        }
        [JsonIgnore] private TBaseData _data;

        #region Initialization
        public void Create(string InstanceId)
        {
            this.InstanceId = InstanceId;
            OnCreate();
        }

        public void SetLevel(int level)
        {
            this.Level = level;
        }

        public void SetBaseDataId(string baseDataId) 
        {
            this.BaseDataId = baseDataId;
        }

        /// <summary>
        /// Called when the instance is created 
        /// Wont be called when the instances is beeing loaded (deserialized)
        /// </summary>
        protected virtual void OnCreate() { }
        #endregion
    }
}