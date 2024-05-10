using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

using AB_GoogleSheetImporter.Runtime;

using UnityEngine;

namespace RConfig.Runtime
{
    public class RCData : ScriptableObject
    {
        [HideInInspector] public string LastUpdateTime;
        [HideInInspector] public List<SchemeConfig> SchemeConfigs;

        private List<SchemeData> _schemeDataCache;

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
                var schemeName = config.SchemeType().Name;
                var csv = await GSImporter.DownloadCsvAsync(config.PageUrl);
                var schemeData = new SchemeData {SchemeName = schemeName, Csv = csv};
                _schemeDataCache.Add(schemeData);
                Debug.Log($"Saved {schemeName} \n {csv}");
            }


            LastUpdateTime = DateTime.Now.ToString(CultureInfo.InvariantCulture);

            if (Application.isPlaying)
            {
                RConfig.Init();
            }
        }

        public string GetCsvBySchemeName(string schemeName)
        {
            return _schemeDataCache.First(x => x.SchemeName == schemeName).Csv;
        }
    }
}