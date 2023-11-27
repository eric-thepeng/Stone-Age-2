using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameObjectSetUpIdentifier : MonoBehaviour
{
    private static bool analyzedScene = false;
    private static Dictionary<string, GameObject> IDDictionary;
    public string id;

    // This only covers game object that is part of the scene when the scene is loaded.
    public static GameObject GetGameObjectByID(string idToFind)
    {
        if (!analyzedScene)
        {
            IDDictionary = new Dictionary<string, GameObject>();
            Scene currentScene = SceneManager.GetActiveScene();
            GameObjectSetUpIdentifier[] allGOSUI = Resources.FindObjectsOfTypeAll<GameObjectSetUpIdentifier>();

            foreach (GameObjectSetUpIdentifier gosui in allGOSUI)
            {
                if (gosui.gameObject.scene == currentScene)
                {
                    IDDictionary.Add(gosui.id, gosui.gameObject);
                }
            }

            analyzedScene = true;
        }
 
        if (IDDictionary.ContainsKey(idToFind)) return IDDictionary[idToFind];
        else
        {        
            Debug.LogError("Cannot find gameobject with id: " + idToFind);
            return null;
        }
    }
}
