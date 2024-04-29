using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

using AB_GoogleSheetImporter.Runtime;

using UnityEngine;

namespace RConfig.Runtime
{
    [CreateAssetMenu(fileName = nameof(RCData), menuName = "RConfig/" + nameof(RCData), order = 0)]
    public class RCData : ScriptableObject
    {
        [SerializeField] private string _googleSheetUrl;
        [SerializeField] private string _lastUpdateTime;
        public SchemeConfig[] SchemeConfigs;

        private List<SchemeData> _schemeDataCache;

        [ContextMenu("UpdateData")]
        public void UpdateData()
        {
            UpdateDataAsync();
        }

        private async void UpdateDataAsync()
        {
            _schemeDataCache ??= new List<SchemeData>();
            _schemeDataCache.Clear();
            
            foreach (var config in SchemeConfigs)
            {
                var schemeName = config.Scheme.GetType().Name;
                var csv = await GSImporter.DownloadCsvAsync(_googleSheetUrl, config.SheetId);
                var schemeData = new SchemeData {SchemeName = schemeName, Csv = csv};
                _schemeDataCache.Add(schemeData);
                Debug.Log($"Saved {schemeName} \n {csv}");
            }

            
            _lastUpdateTime = DateTime.Now.ToString(CultureInfo.InvariantCulture);
        }

        public string GetCsvBySchemeName(string schemeName)
        {
            return _schemeDataCache.First(x => x.SchemeName == schemeName).Csv;
        }
    }
}