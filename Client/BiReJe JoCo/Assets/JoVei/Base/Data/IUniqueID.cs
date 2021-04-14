namespace JoVei.Base.Data
{
    /// <summary>
    /// Should be used for all sort of data with unique Ids
    /// </summary>
    public interface IUniqueId
    {
        string UniqueId { get; set; }
    }
}