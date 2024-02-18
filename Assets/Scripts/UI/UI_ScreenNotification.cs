using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UI_ScreenNotification : MonoBehaviour
{
    [SerializeField] private GameObject board;
    [SerializeField] private TextMeshPro targetTMP;
    
    static UI_ScreenNotification instance;
    public static UI_ScreenNotification i
    {
        get
        {
            if(instance == null)
            {
                instance = FindObjectOfType<UI_ScreenNotification>();
            }
            return instance;
        }
    }

    public void EndNotification()
    {
        StopAllCoroutines();
        TurnOffDisplay();
    }
    
    /// <summary>
    /// Start a screen space notification that can either disappear after certain time or not.
    /// </summary>
    /// <param name="text">Text to display. Make it short.</param>
    /// <param name="autoEnd">Does the notification disappear after a duration of time. True: assign next parameter for duration. False: ignore next parameter.</param>
    /// <param name="duration">How long does the display last.</param>
    public void StartNotification(string text, bool autoEnd = true, float duration = 10)
    {
        targetTMP.text = text;
        board.SetActive(true);
        StopAllCoroutines();
        StartCoroutine(EndNotificationAfterDuration(duration));
    }

    IEnumerator EndNotificationAfterDuration(float duration)
    {
        yield return new WaitForSeconds(duration);
        TurnOffDisplay();
    }

    void TurnOffDisplay()
    {
        board.SetActive(false);
    }
}
