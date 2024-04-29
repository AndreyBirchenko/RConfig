using System.Text;

using UnityEngine;

namespace RConfig.Runtime.Examples
{
    public class RConfigExample : MonoBehaviour
    {
        private RCVar<SimpleScheme> _simpleData = new("float");
        
        private void Start()
        {
            var sb = new StringBuilder();
            
            var unitOneKey = "unit-one";
            var unitOneScheme = RConfig.Get<UnitScheme>(unitOneKey);
            sb
                .AppendLine(unitOneKey)
                .AppendLine($"Health = {unitOneScheme.Health.ToInt()}")
                .AppendLine($"Name = {unitOneScheme.Name}")
                .AppendLine($"OtherData = {unitOneScheme.OtherData.ToFloat()}");
            sb.AppendLine("======================");
            
            var unitTwoKey = "unit-two";
            var unitTwoScheme = RConfig.Get<UnitScheme>(unitTwoKey);
            sb
                .AppendLine(unitTwoKey)
                .AppendLine($"Health = {unitTwoScheme.Health.ToInt()}")
                .AppendLine($"Name = {unitTwoScheme.Name}")
                .AppendLine($"OtherData = {unitTwoScheme.OtherData.ToBool()}");
            sb.AppendLine("======================");

            sb
                .AppendLine($"Float value = {_simpleData.Get().Value.ToFloat()}");
            
            Debug.Log(sb);
        }
    }
}