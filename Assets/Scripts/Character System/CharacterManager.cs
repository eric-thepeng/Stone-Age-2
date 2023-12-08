using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterManager : MonoBehaviour
{
    public GameObject characterIconPrefab;
    public float iconInterval;

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
    }


    // hang out area is the area for character to move around
    // character will be spawned in the center of the hangoutarea
    public void AddCharacter(CharacterBasicStats characterSettings, BoxCollider hangOutArea)
    {
        InstantiateCharacter(new CharacterConfig(characterSettings, hangOutArea));
    }

    private CharacterConfig InstantiateCharacter(CharacterConfig characterConfig)
    {
        CharacterConfig newCharacterConfig = new CharacterConfig(characterConfig.characterSettings, characterConfig.hangOutArea);


        GameObject characterObject = new GameObject(newCharacterConfig.characterSettings.name);
        newCharacterConfig.character = characterObject.AddComponent<Character>();
        newCharacterConfig.character.Initialize(newCharacterConfig.characterSettings);
        CharacterHomeStatus homeStatus = characterObject.AddComponent<CharacterHomeStatus>();

        homeStatus.hangOutArea = newCharacterConfig.hangOutArea;
        characterObject.transform.position = homeStatus.hangOutArea.center;
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


}
