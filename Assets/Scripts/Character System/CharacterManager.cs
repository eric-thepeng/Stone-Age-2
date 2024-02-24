using System;
using System.Collections;
using System.Collections.Generic;
using Uniland.Characters;
using UnityEngine;
using UnityEngine.AI;

public class CharacterManager : MonoBehaviour
{

    static CharacterManager instance;
    public static CharacterManager i
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<CharacterManager>();
            }
            return instance;
        }
    }
    
    public GameObject characterIconPrefab;
    public float iconInterval;
    public BoxCollider defaultHangoutArea;

    [Serializable]
    protected class CharacterConfig
    {
        [Header("Editor Settings")]
        public CharacterBasicStats characterSettings;
        public BoxCollider hangOutArea;

        [Header("Runtime Info (Do not change in editor)")]
        public Character character;
        public CharacterIcon characterIcon;

        public CharacterConfig(CharacterBasicStats characterSettings, BoxCollider hangOutArea)
        {
            this.characterSettings = characterSettings;
            this.hangOutArea = hangOutArea;
        }

    }

    [SerializeField]
    List<CharacterConfig> initialCharacters;

    [SerializeField]
    List<CharacterConfig> inGameExistingCharacters = new List<CharacterConfig>();

    // Start is called before the first frame update
    void Start()
    {
        foreach (CharacterConfig characterConfig in initialCharacters)
        {

            inGameExistingCharacters.Add(InstantiateCharacter(characterConfig));

        }
        initialCharacters.Clear();
        
        NavMeshSurface _navMeshSurface = FindObjectOfType<NavMeshSurface>();
        _navMeshSurface.UpdateNavMesh(_navMeshSurface.navMeshData);
    }


    // hang out area is the area for character to move around
    // character will be spawned in the center of the hangoutarea
    public void AddCharacter(CharacterBasicStats characterSettings, BoxCollider hangOutArea = null)
    {
        if (hangOutArea == null) hangOutArea = defaultHangoutArea;
        inGameExistingCharacters.Add(InstantiateCharacter(new CharacterConfig(characterSettings, hangOutArea)));
    }

    private CharacterConfig InstantiateCharacter(CharacterConfig characterConfig)
    {
        CharacterConfig newCharacterConfig = new CharacterConfig(characterConfig.characterSettings, characterConfig.hangOutArea);


        GameObject characterObject = new GameObject(newCharacterConfig.characterSettings.name);
        newCharacterConfig.character = characterObject.AddComponent<Character>();
        newCharacterConfig.character.Initialize(newCharacterConfig.characterSettings);
        CharacterBehaviors behaviors = characterObject.AddComponent<CharacterBehaviors>();
        
        newCharacterConfig.character.l2dCharacter = Instantiate(characterConfig.characterSettings.l2dGameObject, newCharacterConfig.character.transform);
        newCharacterConfig.character.CharacterStats = new CharacterStats(newCharacterConfig.characterSettings);

        behaviors.hangOutArea = newCharacterConfig.hangOutArea;
        characterObject.transform.position = behaviors.hangOutArea.center;
        characterObject.transform.SetParent(transform.GetChild(0));

        GameObject characterIcon = Instantiate(characterIconPrefab, GameObject.Find("Character Icon").transform);
        newCharacterConfig.characterIcon = characterIcon.GetComponent<CharacterIcon>();

        characterIcon.GetComponent<SpriteRenderer>().sprite = newCharacterConfig.characterSettings.iconSprite;

        newCharacterConfig.characterIcon.character = newCharacterConfig.character;
        //characterIcon.transform.SetParent(GameObject.Find("Character Icon").transform);

        Vector3 _pos = characterIcon.transform.localPosition;
        _pos.y = inGameExistingCharacters.Count * -iconInterval;
        characterIcon.transform.localPosition = _pos;

        newCharacterConfig.character.SetUp(newCharacterConfig.characterIcon);

        return newCharacterConfig;

        //inGameExistingCharacters.Add(newCharacterConfig);
    }
    
    // usage: Character tarCharacter = getCharacter(characterSettings);
    // tarCharacter.charInteractions.EnableRuaCountdown();
    // tarCharacter.charInteractions.DisableRuaCountdown();
    public Character getCharacter(CharacterBasicStats characterSettings) {
        // check if character exists in inGameExistingCharacters
        foreach (CharacterConfig characterConfig in inGameExistingCharacters)
        {
            if (characterConfig.characterSettings.name == characterSettings.name)
            {
                return characterConfig.character;
            }
        }
        return null;

    }


}
