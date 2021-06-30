using UnityEngine;
using JoVei.Base;

namespace BiReJeJoCo
{
    [CreateAssetMenu(fileName = "SoundMapping", menuName = "Mapping/SoundMapping")]
    public class SoundMapping : AssetMapper<SoundMapping, string, AudioClipSetup>
    {
    }

    [System.Serializable]
    public struct AudioClipSetup 
    {
        public string name;
        public AudioClip clip;
        [Range(0, 2)] public float volume;
    }
}