using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using UnityEngine;
using UnityEngine.Networking;

namespace AB_GoogleSheetImporter.Runtime
{
    public static class GSImporter
    {
        private static readonly Regex _regex;

        static GSImporter()
        {
            _regex = new Regex(@"https://docs\.google\.com/spreadsheets/d/(.+)/");
        }

        public static async Task<string> DownloadCsvAsync(string url)
        {
            var pair = url.Split("gid=");
            if (pair.Length < 2)
            {
                throw new ArgumentException("Url is not valid");
            }
            
            var sheetId = pair[1];
            
            var stringFormat = GetStringFormat(FileFormat.csv);
            var downloadUrl = GetDownloadUrl(url, stringFormat) + $"&gid={sheetId}";
            return await DownloadAsync(downloadUrl);
        }

        public static async Task DownloadAsync(string fileName, string sheetUrl, string savePath, FileFormat format)
        {
            ValidateInputData(fileName, sheetUrl, savePath);

            var stringFormat = GetStringFormat(format);
            var directoryPath = $"{Application.dataPath}/{savePath}";
            var finiteSavePath = $"{directoryPath}/{fileName}.{stringFormat}";

            var downloadUrl = GetDownloadUrl(sheetUrl, stringFormat);

            if (Directory.Exists(directoryPath) == false)
            {
                Directory.CreateDirectory(directoryPath);
            }

            var tableData = await DownloadAsync(downloadUrl);
            await File.WriteAllTextAsync(finiteSavePath, tableData);
        }

        private static string GetDownloadUrl(string publicUrl, string format)
        {
            var match = _regex.Match(publicUrl);
            var key = match.Groups[1];
            return $"https://docs.google.com/spreadsheets/export?id={key}&exportFormat={format}";
        }

        private static async Task<string> DownloadAsync(string downloadUrl)
        {
            try
            {
                using var client = UnityWebRequest.Get(downloadUrl);
                await client.SendWebRequest();
                if (client.error != null)
                {
                    Debug.LogError(client.error);
                    {
                        return null;
                    }
                }

                return client.downloadHandler.text;
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                throw;
            }

            return null;
        }

        private static string GetStringFormat(FileFormat format)
        {
            switch (format)
            {
                case FileFormat.csv:
                    return "csv";
                case FileFormat.tsv:
                    return "tsv";
                case FileFormat.ods:
                    return "ods";
                default:
                    throw new ArgumentOutOfRangeException(nameof(format), format, null);
            }
        }

        private static void ValidateInputData(string fileName, string sheetUrl, string savePath)
        {
            if (string.IsNullOrEmpty(fileName)) throw new Exception("Name can not be null or empty");
            if (string.IsNullOrEmpty(sheetUrl)) throw new Exception("Download url path can not be null or empty");
            if (string.IsNullOrEmpty(savePath)) throw new Exception("Save path can not be null or empty");
        }
    }

    internal static class AsyncExtensions
    {
        internal static TaskAwaiter GetAwaiter(this AsyncOperation operation)
        {
            var tcs = new TaskCompletionSource<object>();
            operation.completed += _ => tcs.SetResult(null);
            return ((Task) tcs.Task).GetAwaiter();
        }
    }
    
    public enum FileFormat
    {
        csv,
        tsv,
        ods
    }
}