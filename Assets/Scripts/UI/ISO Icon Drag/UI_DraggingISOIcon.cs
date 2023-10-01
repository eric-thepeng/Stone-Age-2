using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer),typeof(Collider))]
public class UI_DraggingISOIcon : MonoBehaviour
{
    private void Update()
    {
        transform.position = WorldUtility.GetMouseHitPoint(WorldUtility.LAYER.UI_BACKGROUND, true);
        if (Input.GetMouseButtonUp(0))
        {
            DraggingISOIconManager.i.DeleteDraggingISOIcon();
        }
    }
}
