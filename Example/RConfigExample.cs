using System.Text;

using RConfig.Runtime.DefaultSchemes;

using UnityEngine;
using UnityEngine.SceneManagement;

namespace RConfig.Runtime.Examples
{
    public class RConfigExample : MonoBehaviour
    {
        private RCVar<KeyValueScheme> _keyValueData = new("float");
        
        private async void Start()
        {
            Debug.Log("Updating...");

            await RConfig.UpdateDataAsync();
            
            Debug.Log("Data updated");
            
            var sb = new StringBuilder();
            
            var unitOneScheme = RConfig.Get<UnitScheme>("unit_one");
            sb
                .AppendLine("unit_one")
                .AppendLine($"Health = {unitOneScheme.Health.ToInt()}")
                .AppendLine($"Name = {unitOneScheme.Name}")
                .AppendLine($"OtherData = {unitOneScheme.OtherData.ToFloat()}");
            sb.AppendLine("======================");
            
            var unitTwoScheme = RConfig.Get<UnitScheme>("unit_two");
            sb
                .AppendLine("unit_two")
                .AppendLine($"Health = {unitTwoScheme.Health.ToInt()}")
                .AppendLine($"Name = {unitTwoScheme.Name}")
                .AppendLine($"OtherData = {unitTwoScheme.OtherData.ToBool()}");
            sb.AppendLine("======================");

            sb.AppendLine($"Float value = {_keyValueData.Get().Value.ToFloat()}");
            
            Debug.Log(sb);
        }

        [ContextMenu("UpdateData")]
        public void UpdateData()
        {
            RConfig.UpdateDataAsync();
        }
        
        [ContextMenu("ReloadScene")]
        public void ReloadScene()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        private void Update()
        {
            //Debug.Log($"Float value = {_simpleData.Get().Value.ToFloat()}");
        }
    }
}