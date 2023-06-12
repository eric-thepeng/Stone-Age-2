using Hypertonic.GridPlacement.GridInput;
using UnityEngine;

namespace Hypertonic.GridPlacement.Models
{
    [System.Serializable]
    public class PlatformGridInputsDefinitionMapping 
    {
        public string Name = "";
        
        public RuntimePlatform RuntimePlatform;
        public GridInputDefinition GridInputDefinition;

        public PlatformGridInputsDefinitionMapping(RuntimePlatform runtimePlatform, GridInputDefinition gridInputDefinition)
        {
            RuntimePlatform = runtimePlatform;
            GridInputDefinition = gridInputDefinition;
            Name = System.Enum.GetName(typeof(RuntimePlatform), runtimePlatform);
        }
    }
}