using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseLogic : MonoBehaviour
{

    public Texture2D defaultCursor;
    public Texture2D cursorB;
    public Texture2D cursorC;
    public Texture2D cursorD;
    public Texture2D cursorE;
    // Start is called before the first frame update
    void Start()
    {
        SetCursor(defaultCursor);
    }

    // Update is called once per frame
    void Update()
    {
        print(PlayerState.IsResearch());
        if (PlayerState.IsResearch())
        {
            SetCursor(cursorB);
        }
    }

    public void SetCursor(Texture2D cursorTexture)
    {
        Cursor.SetCursor(cursorTexture, Vector2.zero, CursorMode.ForceSoftware);
    }


}
