using UnityEngine;
using JoVei.Base;

namespace BiReJeJoCo
{
    [CreateAssetMenu(fileName = "MapConfigMapping", menuName = "Mapping/MapConfigMapping")]
    public class MapConfigMapping : AssetMapper<MapConfigMapping, string, MapConfig>
    {
    }
}