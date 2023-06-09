using UnityEngine;

namespace Hypertonic.GridPlacement.CustomSizing
{
    public class CustomBoundsController : MonoBehaviour
    {
        public Bounds Bounds 
        { 
            get { return _bounds; } 
            set { _bounds = value; } 
        }

        [SerializeField]
        private Bounds _bounds = new Bounds(Vector3.zero, Vector3.one);
    }
}