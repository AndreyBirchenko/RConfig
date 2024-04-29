using System.Collections.Generic;

namespace RConfig.Runtime
{
    public abstract class RCScheme
    {
        internal void Map(List<string> data)
        {
            var fieldInfos = this.GetType().GetFields();
            
            for (int i = 0; i < fieldInfos.Length; i++)
            {
                var fieldInfo = fieldInfos[i];
                var fieldValue = new RCType(data[i]);
                
                fieldInfo.SetValue(this, fieldValue);
            }
        }
    }
}