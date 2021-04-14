using System;

namespace JoVei.Base.Helper
{
    /// <summary>
    /// Helper for identification
    /// </summary>
    public static class IdentificationHelper
    {
        /// <summary>
        /// Returns a new unique Identifier 
        /// </summary>
        public static string GetUniqueIdentifier() 
        {
            return Guid.NewGuid().ToString();
        }
    }
}