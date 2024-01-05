using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ExploreMapBillboards))]
public class ExploreMapBillboardsEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        ExploreMapBillboards script = (ExploreMapBillboards)target;

        if (GUILayout.Button("Arrange Billboards Sorting Order"))
        {
            ArrangeBillboards(script);
        }
    }

    private void ArrangeBillboards(ExploreMapBillboards script)
    {
        ArrangeBillboardsRecursive(script.transform);
    }

    private void ArrangeBillboardsRecursive(Transform parentTransform)
    {
        foreach (Transform child in parentTransform)
        {
            SpriteRenderer spriteRenderer = child.GetComponent<SpriteRenderer>();
            if (spriteRenderer != null)
            {
                // The higher the y-position, the lower the sorting order. Times 100 to Ensure Precision
                spriteRenderer.sortingOrder = Mathf.FloorToInt(-child.position.y * 100);

                // Just to make sure and reset Z-Axis to 0.
                child.transform.localPosition = new Vector3(child.transform.localPosition.x, child.transform.localPosition.y, 0);
            }

            // Recursively arrange children of this child
            ArrangeBillboardsRecursive(child);
        }
    }
}