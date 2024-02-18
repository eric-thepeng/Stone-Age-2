using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UniversalUIManager : MonoBehaviour
{
    static UniversalUIManager instance=null;
    public static UniversalUIManager i
    {
        get
        {
            if(instance == null)
            {
                instance = FindObjectOfType<UniversalUIManager>();
            }
            return instance;
        }
    }

    public class UniversalUIComponent
    {
        public WorldInteractable.InteractionType.TypeName identifier;

        public GameObject uiGameObject;
        private RectTransform recTransform = null;
        public void OpenUI()
        {
            uiGameObject.SetActive(true);
        }
        public void CloseUI()
        {
            uiGameObject.SetActive(false);
        }
        public void Initialize()
        {
            recTransform = uiGameObject.GetComponent<RectTransform>();
        }
        public void SetPosition()
        {
            recTransform.anchoredPosition = Input.mousePosition + new Vector3(0,15,0);
        }

        public virtual void SetValue(float v)
        {
            throw new NotImplementedException("not implemented");
        }
    }
    
    [Serializable]
    public class LongPressUI : UniversalUIComponent
    {
        public Slider slider;
        public Image centerImage;
        
        public void AdjustPercentage(float percent)
        {
            slider.value = percent;
        }

        public override void SetValue(float v)
        {
            AdjustPercentage(v);
        }
    }

    [Serializable]
    public class ClickUI : UniversalUIComponent
    {
        public override void SetValue(float v)
        {
            
        }
    }

    [Serializable]
    public class DefaultUI : UniversalUIComponent
    {

    }

    private WorldInteractable.InteractionType displayingWIIT;
    private UniversalUIComponent displayingUIComponent;
    
    public LongPressUI myLongPressUI;
    public ClickUI myClickUI;
    public DefaultUI myDefaultUI;

    public Texture2D cursorDefault;
    public Texture2D cursorB;
    public Texture2D cursorC;
    public Texture2D cursorD;
    public Texture2D cursorE;
    private Texture2D[] cursors;
    public enum CursorType
    {
        A,
        B,
        C,
    }

    public Dictionary<CursorType, Texture2D> mouseSprites = new Dictionary<CursorType, Texture2D>();
    private UniversalUIComponent[] allUIComponent;

    private void Start()
    {
        //cursor
        cursors = new Texture2D[] { cursorDefault, cursorB, cursorC};
        int c = 0;
        foreach (CursorType cursorType in Enum.GetValues(typeof(CursorType)))
        {
            mouseSprites.Add(cursorType, cursors[c]);
            c++;
        }
        SetCursor(cursorDefault);

        //sprite
        allUIComponent = new UniversalUIComponent[] {myLongPressUI, myClickUI};
        foreach (var uiComponent in allUIComponent)
        {
            uiComponent.Initialize();
            uiComponent.CloseUI();
        }
    }

    private void Update()
    {
        if (displayingUIComponent == null)
        {
        }
        else
        {
            displayingUIComponent.SetPosition();
        }
    }



    public void SetCursor(Texture2D cursorTexture)
    {
        Cursor.SetCursor(cursorTexture, Vector2.zero, CursorMode.ForceSoftware);
    }


    public void DisplayComponent(WorldInteractable.InteractionType wiit)
    {
        displayingWIIT = wiit;
        
        foreach (var uiComponent in allUIComponent)
        {
            if (uiComponent.identifier == wiit.typeName)
            {
                displayingUIComponent = uiComponent;
                displayingUIComponent.OpenUI();
                displayingUIComponent.SetPosition();
                displayingUIComponent.SetValue(wiit.GetProgressPercent());
            }
            else
            {
                uiComponent.CloseUI();
            }
        }
    }

    public void DisplayCursor(CursorType cursorType)
    {
        SetCursor(mouseSprites[cursorType]);
    }


    public void CancelDisplayComponent(WorldInteractable.InteractionType wiit)
    {
        if(wiit!=null && displayingWIIT != wiit) return;
        displayingWIIT = null;
        displayingUIComponent = null;
        foreach (var uiComponent in allUIComponent)
        {
            uiComponent.CloseUI();
        }
    }

    public void CancelDisplayCursor()
    {
        SetCursor(mouseSprites[CursorType.A]);
    }
}
