using System.IO;
using System.Xml.Serialization;

namespace RConfig.Runtime
{
    public static class XmlManager
    {
        public static void Serialize<T>(T data, string path)
        {
            var serializer = new XmlSerializer(typeof(T));
            using var writer = new StreamWriter(path);
            serializer.Serialize(writer, data);
        }

        public static T Deserialize<T>(string path)
        {
            var serializer = new XmlSerializer(typeof(T));
            using var reader = new StreamReader(path);
            return (T) serializer.Deserialize(reader);
        }
    }
}