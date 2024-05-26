using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Xml.Serialization;

using AB_GoogleSheetImporter.Runtime;

using Leopotam.Serialization.Csv;

using UnityEngine;

namespace RConfig.Runtime
{
    public static class RConfig
    {
        private static Dictionary<Type, Dictionary<string, RCScheme>> _dataCache;
        private static List<SchemeData> _schemeDataCache;
        private static string _cachePath;

        static RConfig()
        {
            _cachePath = Path.Combine(Application.persistentDataPath, "RCCache.xml");
        }

        public static T Get<T>(string key) where T : RCScheme
        {
            if (_dataCache.TryGetValue(typeof(T), out var mappedData))
            {
                return mappedData[key] as T;
            }
            else
            {
                throw new Exception($"Can not get value of type ${typeof(T).Name}");
            }
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        public static void Init()
        {
            _dataCache ??= new Dictionary<Type, Dictionary<string, RCScheme>>();
            _dataCache.Clear();

            CreateSchemeDataCache();

            var schemeConfigs = ParseSchemeConfigs();
            for (int i = 0; i < schemeConfigs.Count; i++)
            {
                var schemeConfig = schemeConfigs[i];
                var schemeName = schemeConfig.SchemeName;
                var csv = GetCsvBySchemeName(schemeName);
                if (string.IsNullOrEmpty(csv))
                {
                    Debug.LogError($"Can not find data for {schemeName}");
                    continue;
                }

                (var data, bool ok) = CsvReader.ParseKeyedLists(csv, true);
                if (!ok)
                {
                    Debug.LogError($"Can not parse data for {schemeName}");
                    continue;
                }

                MapScheme(TypeUtils.GetTypeByName(schemeName), data);
            }

            Debug.Log("RConfig Initialized");
        }

        public static async Task UpdateDataAsync()
        {
            if (!Application.isPlaying)
                return;

            await DownloadDataAsync();
            Init();
        }

        private static void MapScheme(Type type, Dictionary<string, List<string>> _schemeData)
        {
            var mappedData = new Dictionary<string, RCScheme>();

            foreach (var (key, dataList) in _schemeData)
            {
                var instance = Activator.CreateInstance(type);

                if (instance is not RCScheme rcScheme)
                {
                    throw new Exception($"Type {type.Name} is not an RCScheme inheritor");
                }

                rcScheme.Map(dataList);
                mappedData.Add(key, rcScheme);
            }

            _dataCache.Add(type, mappedData);
        }

        public static async Task DownloadDataAsync()
        {
            var configs = ParseSchemeConfigs();
            var dataCache = new SchemeData[configs.Count];

            for (int i = 0; i < configs.Count; i++)
            {
                var schemeName = configs[i].SchemeName;
                var pageUrl = configs[i].PageUrl;
                var csv = await GSImporter.DownloadCsvAsync(pageUrl);
                dataCache[i] = new SchemeData
                {
                    SchemeName = schemeName,
                    UpdatedAt = DateTime.Now,
                    Csv = csv,
                };

                Debug.Log($"Downloaded {schemeName} \n {csv}");
            }

            XmlManager.Serialize(dataCache, _cachePath);
        }

        private static List<SchemeConfig> ParseSchemeConfigs()
        {
            var data = Resources.Load<TextAsset>("RCData");
            if (data == null)
            {
                throw new Exception("Can not find RCData.txt");
            }

            var fileText = data.text;
            var lines = fileText.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);
            var output = new List<SchemeConfig>();

            for (int i = 0; i < lines.Length; i++)
            {
                var line = lines[i];
                if (line.StartsWith("#"))
                    continue;

                var pair = line.Split(' ');
#if DEBUG
                if (pair.Length != 2)
                {
                    throw new Exception($"Data in RCData.txt is not valid in line {i + 1}");
                }
#endif

                output.Add(new SchemeConfig {SchemeName = pair[0], PageUrl = pair[1]});
            }

            return output;
        }

        private static void CreateSchemeDataCache()
        {
            _schemeDataCache = XmlManager.Deserialize<List<SchemeData>>(_cachePath);
        }

        private static string GetCsvBySchemeName(string schemeName)
        {
            for (int i = 0; i < _schemeDataCache.Count; i++)
            {
                var data = _schemeDataCache[i];
                if (data.SchemeName == schemeName)
                {
                    return data.Csv;
                }
            }

            throw new Exception($"Can not find data by scheme name {schemeName}");
        }
    }
}