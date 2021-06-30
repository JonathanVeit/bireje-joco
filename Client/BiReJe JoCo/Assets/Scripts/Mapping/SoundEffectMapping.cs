using UnityEngine;
using JoVei.Base;

namespace BiReJeJoCo
{
    [CreateAssetMenu(fileName = "SoundEffectMapping", menuName = "Mapping/SoundEffectMapping")]
    public class SoundEffectMapping : AssetMapper<SoundEffectMapping, string, AudioSourceHandler>
    {
    }
}