#if UNITY_EDITOR
using System.IO;

using UnityEditor;

using UnityEngine;

namespace RConfig.Runtime
{
    [InitializeOnLoad]
    public static class RCDataCreator
    {
        static RCDataCreator()
        {
            var folderPath = Application.dataPath + "/Resources";
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }


            var dataPath = "Assets/Resources/RCData.asset";
            if (!File.Exists(dataPath))
            {
                Debug.Log("Create RCData");
                var dataInstance = ScriptableObject.CreateInstance<RCData>();
                AssetDatabase.CreateAsset(dataInstance, dataPath);
                AssetDatabase.SaveAssets();   
            }
        }
    }
}
#endif