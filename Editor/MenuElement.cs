#if UNITY_EDITOR

using System.Threading.Tasks;

using UnityEditor;

using UnityEngine;

namespace RConfig.Runtime
{
    public static class MenuElement
    {
        [MenuItem("Tools/RConfig/UpdateData")]
        public static void UpdateData()
        {
            RConfig.DownloadDataAsync().ContinueWith(task =>
            {
                if (task.Status != TaskStatus.RanToCompletion)
                {
                    foreach (var exception in task.Exception!.InnerExceptions)
                    {
                        Debug.LogError(exception);
                    }
                }
            });
        }
    }
}
#endif