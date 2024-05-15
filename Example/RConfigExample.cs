using System;
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
            
            var unitOneScheme = RConfig.Get<UnitScheme>("unit-one");
            sb
                .AppendLine("unit-one")
                .AppendLine($"Health = {unitOneScheme.Health.ToInt()}")
                .AppendLine($"Name = {unitOneScheme.Name}")
                .AppendLine($"OtherData = {unitOneScheme.OtherData.ToFloat()}");
            sb.AppendLine("======================");
            
            var unitTwoScheme = RConfig.Get<UnitScheme>("unit-two");
            sb
                .AppendLine("unit-two")
                .AppendLine($"Health = {unitTwoScheme.Health.ToInt()}")
                .AppendLine($"Name = {unitTwoScheme.Name}")
                .AppendLine($"OtherData = {unitTwoScheme.OtherData.ToBool()}");
            sb.AppendLine("======================");

            sb.AppendLine($"Float value = {_simpleData.Get().Value.ToFloat()}");
            
            Debug.Log(sb);
        }

        [ContextMenu("UpdateData")]
        public void UpdateData()
        {
            RConfig.UpdateData();
        }

        private void Update()
        {
            Debug.Log($"Float value = {_simpleData.Get().Value.ToFloat()}");
        }
    }
}