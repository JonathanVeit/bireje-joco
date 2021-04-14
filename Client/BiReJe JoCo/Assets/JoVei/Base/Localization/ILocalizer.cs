using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JoVei.Base
{
    /// <summary>
    /// Localize strings to different languages
    /// </summary>
    public interface ILocalizer 
    {
        string GetLocalizedString(string key);
    }
}