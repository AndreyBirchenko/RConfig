using System;

using UnityEngine;

namespace RConfig.Runtime
{
    [Serializable]
    public class SchemeConfig
    {
        [SerializeReference] public RCScheme Scheme;
        public int SheetId;
    }
}