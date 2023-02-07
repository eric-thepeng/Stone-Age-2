using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PictureSequence : MonoBehaviour
{
    [SerializeField] Frame[] AllFrames;
    [SerializeField] Transform CameraTransform;
    int cursor = 0;
    Frame frameNow = null;
    private void Start()
    {
        PlayFrame(AllFrames[cursor]);
    }
    void PlayFrame(Frame f)
    {
        //remove last frame
        if(frameNow != null)
        {
            frameNow.BackToOrigionalPosition();
        }

        //move in new frame
        frameNow = f;
        f.MoveToPosition(CameraTransform.position);
        StartCoroutine(CountDown(f.playTime));
    }
    void NextFrame()
    {
        StopAllCoroutines();
        cursor += 1;
        if(cursor == AllFrames.Length)
        {
            End();
        }
        else
        {
            PlayFrame(AllFrames[cursor]);
        }
    }

    void End()
    {
        SceneManager.LoadScene(sceneName: "SampleScene");
    }

    private void OnMouseUpAsButton()
    {
        NextFrame();
    }

    IEnumerator CountDown(float frameTime)
    {
        float timeCount = 0;
        while (timeCount < frameTime)
        {
            timeCount += Time.deltaTime;
            yield return new WaitForSeconds(0);
        }
        NextFrame();
    }
}
