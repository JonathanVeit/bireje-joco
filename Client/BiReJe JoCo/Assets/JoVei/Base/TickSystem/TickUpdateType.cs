namespace JoVei.Base.TickSystem
{
    /// <summary>
    /// Update, FixedUpdate or LateUpdate 
    /// </summary>
    public enum TickUpdateType 
    {
        Update      = 0,
        FixedUpdate = 1,
        LateUpdate  = 2,
        UpdateByFrame       = 3,
        FixedUpdateByFrame  = 4,
        LateUpdateByFrame   = 5
    }
}
