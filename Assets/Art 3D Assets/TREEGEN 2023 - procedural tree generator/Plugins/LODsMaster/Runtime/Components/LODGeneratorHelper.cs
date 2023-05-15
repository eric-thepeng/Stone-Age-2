using UnityEngine;

namespace UnityMeshSimplifier
{
    /// <summary>
    /// A LOD (level of detail) generator helper.
    /// </summary>
    [AddComponentMenu("TREEGEN/LOD Generator Helper")]
    public sealed class LODGeneratorHelper : MonoBehaviour
    {
        #region Fields
        [SerializeField, Tooltip("The fade mode used by the created LOD group.")]
        private LODFadeMode fadeMode = LODFadeMode.None;
        [SerializeField, Tooltip("If the cross-fading should be animated by time.")]
        private bool animateCrossFading = false;

        [SerializeField, Tooltip("If the renderers under this game object and any children should be automatically collected.")]
        private bool autoCollectRenderers = true;

        [SerializeField, Tooltip("The simplification options.")]
        private SimplificationOptions simplificationOptions = SimplificationOptions.Default;

        [SerializeField, Tooltip("The path within the project to save the generated assets. Leave this empty to use the default path.")]
        private string saveAssetsPath = string.Empty;

        [SerializeField, Tooltip("The LOD levels.")]
        private LODLevel[] levels = null;

        [SerializeField]
        private bool isGenerated = false;
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the fade mode used by the created LOD group.
        /// </summary>
        public LODFadeMode FadeMode
        {
            get { return fadeMode; }
            set { fadeMode = value; }
        }

        /// <summary>
        /// Gets or sets if the cross-fading should be animated by time. The animation duration
        /// is specified globally as crossFadeAnimationDuration.
        /// </summary>
        public bool AnimateCrossFading
        {
            get { return animateCrossFading; }
            set { animateCrossFading = value; }
        }

        /// <summary>
        /// Gets or sets if the renderers under this game object and any children should be automatically collected.
        /// </summary>
        public bool AutoCollectRenderers
        {
            get { return autoCollectRenderers; }
            set { autoCollectRenderers = value; }
        }

        /// <summary>
        /// Gets or sets the simplification options.
        /// </summary>
        public SimplificationOptions SimplificationOptions
        {
            get { return simplificationOptions; }
            set { simplificationOptions = value; }
        }

        /// <summary>
        /// Gets or sets the path within the project to save the generated assets.
        /// Leave this empty to use the default path.
        /// </summary>
        public string SaveAssetsPath
        {
            get { return saveAssetsPath; }
            set { saveAssetsPath = value; }
        }

        /// <summary>
        /// Gets or sets the LOD levels for this generator.
        /// </summary>
        public LODLevel[] Levels
        {
            get { return levels; }
            set { levels = value; }
        }

        /// <summary>
        /// Gets if the LODs have been generated.
        /// </summary>
        public bool IsGenerated
        {
            get { return isGenerated; }
        }
        #endregion

        #region Unity Events
        private void Reset()
        {
            fadeMode = LODFadeMode.None;
            animateCrossFading = false;
            autoCollectRenderers = true;
            simplificationOptions = SimplificationOptions.Default;

            levels = new LODLevel[]
            {
                new LODLevel(0.5f, 1f)
                {
                    CombineMeshes = false,
                    CombineSubMeshes = false,
                    SkinQuality = SkinQuality.Auto,
                    ShadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On,
                    ReceiveShadows = true,
                    SkinnedMotionVectors = true,
                    LightProbeUsage = UnityEngine.Rendering.LightProbeUsage.BlendProbes,
                    ReflectionProbeUsage = UnityEngine.Rendering.ReflectionProbeUsage.BlendProbes,
                },
                new LODLevel(0.17f, 0.65f)
                {
                    CombineMeshes = true,
                    CombineSubMeshes = false,
                    SkinQuality = SkinQuality.Auto,
                    ShadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On,
                    ReceiveShadows = true,
                    SkinnedMotionVectors = true,
                    LightProbeUsage = UnityEngine.Rendering.LightProbeUsage.BlendProbes,
                    ReflectionProbeUsage = UnityEngine.Rendering.ReflectionProbeUsage.Simple
                },
                new LODLevel(0.02f, 0.4225f)
                {
                    CombineMeshes = true,
                    CombineSubMeshes = true,
                    SkinQuality = SkinQuality.Bone2,
                    ShadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off,
                    ReceiveShadows = false,
                    SkinnedMotionVectors = false,
                    LightProbeUsage = UnityEngine.Rendering.LightProbeUsage.Off,
                    ReflectionProbeUsage = UnityEngine.Rendering.ReflectionProbeUsage.Off
                }
            };
        }
        #endregion
    }
}