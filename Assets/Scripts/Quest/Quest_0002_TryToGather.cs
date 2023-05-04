using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Quest_0002_TryToGather : Quest
{
    [SerializeField] private GameObject characterIconArrow;

    protected override void StartQuest()
    {
        base.StartQuest();
        ShowCharacterIconArrow();
        CharacterIcon.onCharacterPickedUp += CloseCharacterIconArrow;
        CharacterIcon.onCharacterQuitPickUp += ShowCharacterIconArrow;
        CharacterIcon.onCharacterStartGathering += CloseCharacterIconArrow;
        CharacterIcon.onCharacterStartGathering += SetComplete;//AddListener(SetComplete);
    }

    protected override void CompleteQuest()
    {
        CloseCharacterIconArrow();
        CharacterIcon.onCharacterStartGathering -= SetComplete;//RemoveListener(SetComplete);
        CharacterIcon.onCharacterPickedUp -= CloseCharacterIconArrow;
        CharacterIcon.onCharacterQuitPickUp -= ShowCharacterIconArrow;
        CharacterIcon.onCharacterStartGathering -= CloseCharacterIconArrow;
        base.CompleteQuest();
    }

    public void SetComplete()
    {
        CompleteQuest();
    }
    
    void ShowCharacterIconArrow()
    {
        characterIconArrow.gameObject.SetActive(true);
    }

    void CloseCharacterIconArrow()
    {
        characterIconArrow.gameObject.SetActive(false);
    }


}
