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

    public enum GameObjectType
    {
        // Basic Relationships
        Self,
        Parent,
        ZeroLocation,
        
        // Components
        WorldSpaceButton,
        LevelUp,
        ExploreSpot,
        GatherSpot,
        TextMeshPro,
        SpriteRenderer,
        
        // Highlight
        Highlight_Arrow,
        Highlight_Circle,
        Highlight_Other,
        
        // Other
        Background,
        Other_1,
        Other_2,
        Other_3
    }
    
    public GameObjectSetUpIdentifierScriptableObject gosuiScriptableObject;
    public GameObjectType gameObjectType;


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
                if (!gosui.id.Equals("none") && gosui.gameObject.scene == currentScene)
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
