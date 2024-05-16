using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Leopotam.Serialization.Csv;

using UnityEngine;

namespace RConfig.Runtime
{
    public static class RConfig
    {
        public static event Action DataUpdated = delegate { };
        private static Dictionary<Type, Dictionary<string, RCScheme>> _dataCache;
        private static RCData _data;
        private static TaskCompletionSource<bool> _taskCompletionSource;

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
            if (!_data)
            {
                _data = Resources.Load<RCData>("RCData");
            }

            _dataCache = new Dictionary<Type, Dictionary<string, RCScheme>>();

            foreach (var schemeConfig in _data.SchemeConfigs)
            {
                var schemeName = schemeConfig.SchemeType().Name;
                var csv = _data.GetCsvBySchemeName(schemeName);
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

                MapScheme(schemeConfig.SchemeType(), data);
            }

            DataUpdated.Invoke();
            _taskCompletionSource?.SetResult(true);
            Debug.Log("RConfig Initialized");
        }

        public static void UpdateData()
        {
            if (!Application.isPlaying)
                return;

            _data.UpdateData();
        }

        public static async Task UpdateDataAsync()
        {
            if (!Application.isPlaying)
                return;

            _taskCompletionSource = new TaskCompletionSource<bool>();
            _data.UpdateData();

            await _taskCompletionSource.Task;
            _taskCompletionSource = null;
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
    }
}