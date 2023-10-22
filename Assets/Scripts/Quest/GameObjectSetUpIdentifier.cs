using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameObjectSetUpIdentifier : MonoBehaviour
{
    private static Dictionary<string, GameObject> IDList = new Dictionary<string, GameObject>();
    public string id;

    public static GameObject GetGameObjectByID(string idToFind)
    {
        return IDList[idToFind];
    }

    private void Awake()
    {
        if(IDList.ContainsKey(id)) Debug.LogError("Two GameObjectSetUpIdentifier has the same ID");
        IDList.Add(id, this.gameObject);
    }
}
