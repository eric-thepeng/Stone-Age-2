using UnityEngine;

namespace FogSystems
{
    public class ExampleFogTerrainHeightStuckEffector : MonoBehaviour
    {
        private void Update()
        {
            Vector3 pos = transform.position;
            pos.y = Terrain.activeTerrain.SampleHeight(transform.position);
            transform.position = pos;
        }
    }
}