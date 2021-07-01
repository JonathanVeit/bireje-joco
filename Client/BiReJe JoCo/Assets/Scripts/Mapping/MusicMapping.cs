using UnityEngine;
using JoVei.Base;

namespace BiReJeJoCo
{
    [CreateAssetMenu(fileName = "MusicMapping", menuName = "Mapping/MusicMapping")]
    public class MusicMapping : AssetMapper<MusicMapping, string, AudioClip>
    {
    }
}