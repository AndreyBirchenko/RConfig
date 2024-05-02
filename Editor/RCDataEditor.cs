#if UNITY_EDITOR

using System.Collections.Generic;

using RConfig.Runtime;

using UnityEditor;

using UnityEngine;

namespace RConfig.Editor
{
    [CustomEditor(typeof(RCData))]
    public class RCDataEditor : UnityEditor.Editor
    {
        private RCData _rcData;

        private void OnEnable()
        {
            _rcData = (RCData) target;
        }

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            EditorGUILayout.LabelField("Last update time", _rcData.LastUpdateTime);
            EditorGUILayout.Space();
            if (_rcData.SchemeConfigs != null && _rcData.SchemeConfigs.Count > 0)
            {
                if (GUILayout.Button("Update data", GUILayout.MaxHeight(EditorGUIUtility.singleLineHeight * 1.5f)))
                {
                    _rcData.UpdateData();
                }
                
                EditorGUILayout.Space();

                for (int i = 0; i < _rcData.SchemeConfigs.Count; i++)
                {
                    var config = _rcData.SchemeConfigs[i];
                    GUILayout.BeginHorizontal();
                    if (GUILayout.Button("X", GUILayout.ExpandWidth(false)))
                    {
                        _rcData.SchemeConfigs.RemoveAt(i);
                        EditorUtility.SetDirty(_rcData);
                        continue;
                    }

                    if (config == null || config.SchemeType() == null)
                    {
                        EditorGUILayout.HelpBox("Scheme is broken, delete it", MessageType.Warning, true);
                    }
                    else
                    {
                        GUILayout.BeginVertical(GUI.skin.box, GUILayout.MaxWidth(float.MaxValue));
                        EditorGUILayout.LabelField(config.SchemeType().Name, EditorStyles.boldLabel);
                        var indent = EditorGUI.indentLevel;
                        EditorGUI.indentLevel++;
                        var newSheetId = EditorGUILayout.IntField("Sheet Id", config.SheetId);
                        if (newSheetId != config.SheetId)
                        {
                            Undo.RecordObject(_rcData, "Change Sheet Id");
                            config.SheetId = newSheetId;
                            EditorUtility.SetDirty(_rcData);
                        }

                        EditorGUI.indentLevel = indent;
                        GUILayout.EndVertical();
                        EditorGUILayout.Space();
                    }

                    GUILayout.EndHorizontal();
                    EditorGUILayout.Space();
                }
            }
            else
            {
                EditorGUILayout.HelpBox("You need to add at least one scheme", MessageType.Warning, true);
            }

            EditorGUILayout.Space();
            if (GUILayout.Button("Add scheme", GUILayout.MaxHeight(EditorGUIUtility.singleLineHeight * 1.5f)))
            {
                ShowDropdown();
            }
        }
        
        private void ShowDropdown()
        {
            var menu = new GenericMenu();
            var types = TypeUtils.GetSchemeTypes();
            for (int i = 0; i < types.Count; i++)
            {
                var type = types[i];
                menu.AddItem(
                    new GUIContent(type.Name),
                    false,
                    (x) =>
                    {
                        _rcData.SchemeConfigs ??= new List<SchemeConfig>();
                        var schemeConfig = new SchemeConfig() {SchemeTypeFullName = type.AssemblyQualifiedName};
                        foreach (var config in _rcData.SchemeConfigs)
                        {
                            if (config.SchemeType() == type)
                            {
                                EditorUtility.DisplayDialog("RConfig", "This scheme is already exist",
                                    "Close");
                                return;
                            }
                        }

                        _rcData.SchemeConfigs.Add(schemeConfig);
                        EditorUtility.SetDirty(_rcData);
                    },
                    type);
            }

            menu.ShowAsContext();
        }
    }
}
#endif