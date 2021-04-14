using UnityEngine;
using UnityEditor;

namespace JoVei.Base.TickSystem
{
    // IngredientDrawerUIE
    [CustomPropertyDrawer(typeof(TickSystemConfig.TickRegionDrawer))]
    public class TickSystemConfigDrawer : PropertyDrawer
    {
        private float fieldHeight = 18;
        private float Spacing = 2;

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return property.isExpanded ? fieldHeight * 5 + Spacing * 3 : fieldHeight;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            // Using BeginProperty / EndProperty on the parent property means that
            // prefab override logic works on the entire property.
            position.height = fieldHeight;
            EditorGUI.BeginProperty(position, label, property);

            // Don't make child fields be indented
            var indent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;

            property.isExpanded = EditorGUI.Foldout(position, property.isExpanded, label);

            if (property.isExpanded)
            {
                // Calculate rects
                var idRect = new Rect(position.x, position.y + fieldHeight, position.width, position.height);
                var typeRect = new Rect(position.x, position.y + fieldHeight * 2 + Spacing, position.width, position.height);
                var borderRect = new Rect(position.x, position.y + fieldHeight * 3 + Spacing * 2, position.width, position.height);
                var scaleRect = new Rect(position.x, position.y + fieldHeight * 4 + Spacing * 3, position.width, position.height);

                // Draw fields 
                EditorGUI.PropertyField(idRect, property.FindPropertyRelative("id"), new GUIContent("Id"));
                EditorGUI.PropertyField(typeRect, property.FindPropertyRelative("type"), new GUIContent("Type"));
                DrawBorderField(property, borderRect);
                EditorGUI.PropertyField(scaleRect, property.FindPropertyRelative("scale"), new GUIContent("Scale"));
            }

            // Set indent back to what it was
            EditorGUI.indentLevel = indent;
            EditorGUI.EndProperty();
        }

        private static void DrawBorderField(SerializedProperty property, Rect borderRect)
        {
            var typeProp = property.FindPropertyRelative("type");
            var updateType = (TickUpdateType)typeProp.enumValueIndex;
            var borderProp = property.FindPropertyRelative("border");

            switch (updateType)
            {
                case TickUpdateType.Update:
                case TickUpdateType.FixedUpdate:
                case TickUpdateType.LateUpdate:
                    borderProp.floatValue = EditorGUI.Slider(borderRect, new GUIContent("Border"), borderProp.floatValue, 0, 100);
                    break;

                case TickUpdateType.UpdateByFrame:
                case TickUpdateType.FixedUpdateByFrame:
                case TickUpdateType.LateUpdateByFrame:
                    borderProp.floatValue = EditorGUI.IntSlider(borderRect, new GUIContent("Border"), (int)borderProp.floatValue, 0, 100);
                    break;
            }
        }
    }
}