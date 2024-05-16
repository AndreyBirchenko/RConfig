using System;

namespace RConfig.Runtime
{
    [Serializable]
    public class SchemeConfig
    {
        public string SchemeTypeFullName;
        public Type SchemeType() => Type.GetType(SchemeTypeFullName);
        public string PageUrl;
    }
}