using BiReJeJoCo.Backend;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace BiReJeJoCo.Map
{
    [CustomEditor(typeof(MapConfig))]
    [CanEditMultipleObjects]
    public class MapConfigEditor : Editor
    {
        MapConfig Target => target as MapConfig;

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            if (GUILayout.Button("Create"))
                CreateConfig();
        }

        private void CreateConfig()
        {
            var allMarker = GameObject.FindObjectsOfType<SpawnPointMarker>();

            var hunterSpawnPoints = allMarker.Where(x =>x.role == PlayerRole.Hunter && !x.isCollectable).ToList().Select(x => x.transform.position).ToArray();
            var huntedSpawnPoints = allMarker.Where(x => x.role == PlayerRole.Hunted && !x.isCollectable).ToList().Select(x => x.transform.position).ToArray();
            var collectableSpawnPoints = allMarker.Where(x => x.isCollectable).ToList().Select(x => x.transform.position).ToArray();

            Target.OverrideHunterSpawnPoints(hunterSpawnPoints);
            Target.OverrideHuntedSpawnPoints(huntedSpawnPoints);
            Target.OverrideCollectableSpawnPoints(collectableSpawnPoints);

            EditorUtility.SetDirty(Target);
        } 
    }
}