#if UNITY_EDITOR

using UnityEditor;

namespace RConfig.Runtime
{
    public static class MenuElement
    {
        [MenuItem("Tools/RConfig/UpdateData")]
        public static void UpdateData()
        {
            RConfig.DownloadDataAsync();
        }
    }
}
#endif