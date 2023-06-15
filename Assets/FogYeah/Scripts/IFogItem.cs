using UnityEngine;
using static FogSystems.FogSystemEnums;

internal interface IFogItem
{
    /// <summary>
    /// Get the current state of the Fog Item
    /// </summary>
    /// <returns>The current state of the item</returns>
    FogState GetState();

    /// <summary>
    /// Get the cached position of the Fog Item
    /// </summary>
    /// <returns>The cached position of this item</returns>
    Vector3 GetPosition();

    /// <summary>
    /// Gets the unique identifier for the Fog Item
    /// </summary>
    /// <returns>The unique identifier for this item</returns>
    int GetId();

    /// <summary>
    /// Sets the gameobject associated to this Fog Item to inactive
    /// </summary>
    void SetItemInactive();

    /// <summary>
    /// Sets the gameobject associated to this Fog Item to active
    /// </summary>
    void SetItemActive();

    /// <summary>
    /// Sets the state of the fog item to the state specified
    /// </summary>
    /// <param name="state">The desired state to apply to the item</param>
    void SetState(FogState state);

    /// <summary>
    /// Check for if a desired opacity has been reached yet
    /// </summary>
    /// <returns>True if the desired opacity does not match the last known opacity</returns>
    bool RequiresChange();

    /// <summary>
    /// Steps the opacity of a FogItem towards a desired opacity.
    /// </summary>
    /// <param name="val">Increment of value to apply to the alpha of the FogItems image</param>
    void StepColor(float val);
}