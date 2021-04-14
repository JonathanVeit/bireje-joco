namespace JoVei.Base.Data
{
    /// <summary>
    /// Interface for a container class to store IPlayerProfile
    /// </summary>
    public interface IProfileHandler
    {
        void RegisterProfile(IPlayerProfile profile);
        void RegisterLocalProfile(IPlayerProfile profile);
        void UnregisterProfile(IPlayerProfile profile);
        IPlayerProfile GetProfile(string Id);
        TProfile GetProfileAs<TProfile>(string Id) where TProfile : class, IPlayerProfile;
        IPlayerProfile GetLocalPlayerProfile();
    }
}
