using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using TMPro;

/* Tutorial Texts
 * 1. Drag the character icon of your spirit-animal friend onto a exploration spot to gather resource.
 * 2. Click on the different panels to craft
 * 
 */

public class ProgressControl : SerializedMonoBehaviour
{
    static ProgressControl instance;

    public static ProgressControl i
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<ProgressControl>();
            }
            return instance;
        }
    }

    public class TutorialInfo
    {
        public string text;
        public Sprite image;
    }

    [SerializeField] List<TutorialInfo> allTutorials;

    int displayingIndex = 0;

    private void Start()
    {
        DisplayTutorial(displayingIndex);
    }

    void DisplayTutorial(int displayWhich)
    {
        displayingIndex = displayWhich;
        transform.Find("Image").GetComponent<SpriteRenderer>().sprite = allTutorials[displayingIndex].image;
        transform.Find("Text").GetComponent<TextMeshPro>().text = allTutorials[displayingIndex].text;
    }

    public void CloseTutorial()
    {
        Destroy(gameObject);
    }

    public void PageBefore()
    {
        if (displayingIndex == 0) return;
        DisplayTutorial(displayingIndex - 1);
    }

    public void PageAfter()
    {
        if (displayingIndex == allTutorials.Count - 1) return;
        DisplayTutorial(displayingIndex + 1);

    }

}
