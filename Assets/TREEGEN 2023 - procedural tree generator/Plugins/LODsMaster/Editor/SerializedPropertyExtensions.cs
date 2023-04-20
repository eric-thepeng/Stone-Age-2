

using System.Collections.Generic;
using UnityEditor;

namespace UnityMeshSimplifier.Editor
{
    internal static class SerializedPropertyExtensions
    {
        public static IEnumerable<SerializedProperty> GetChildProperties(this SerializedProperty property)
        {
            int originalDepth = property.depth;
            var childProperty = property.Copy();
            if (!childProperty.NextVisible(true))
                yield break; // There was no more properties

            while (childProperty.depth > originalDepth)
            {
                yield return childProperty;

                if (!childProperty.NextVisible(false))
                    break;
            }
        }
    }
}