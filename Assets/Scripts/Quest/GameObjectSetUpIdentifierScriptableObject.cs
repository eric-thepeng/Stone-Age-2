using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameObjectSetUpIdentifierScriptableObject : ScriptableObject
{
    private Dictionary<GameObjectSetUpIdentifier.GameObjectType, GameObject> idToGameObject = new Dictionary<GameObjectSetUpIdentifier.GameObjectType, GameObject>();

    public void RegisterGameObject(GameObjectSetUpIdentifier.GameObjectType id, GameObject gameObject)
    {
        if(idToGameObject.ContainsKey(id)) Debug.LogError("Repeated registration with id: " + id);
        idToGameObject.Add(id,gameObject);
    }

    public bool HasID(GameObjectSetUpIdentifier.GameObjectType id)
    {
        if (idToGameObject.ContainsKey(id)) return true;
        return false;
    }

    public GameObject GetGameObjectWithID(GameObjectSetUpIdentifier.GameObjectType id)
    {
        if (!HasID(id)) return null;
        return idToGameObject[id];
    }
}
