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
            recTransform.anchoredPosition = Input.mousePosition;
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
            if (v == 1) OpenUI();
            else CloseUI();
        }
    }

    private WorldInteractable.InteractionType displayingWIIT;
    private UniversalUIComponent displayingUIComponent;
    
    public LongPressUI myLongPressUI;
    public ClickUI myClickUI;

    private UniversalUIComponent[] allUIComponent;

    private void Start()
    {
        allUIComponent = new UniversalUIComponent[] {myLongPressUI, myClickUI };
        foreach (var uiComponent in allUIComponent)
        {
            uiComponent.Initialize();
        }
        myLongPressUI.Initialize();
    }

    private void Update()
    {
        if (displayingUIComponent == null)
        {
            print("is null");
        }
        else
        {
            print("not");
            displayingUIComponent.SetPosition();
        }
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

    public void CancelDisplayComponent(WorldInteractable.InteractionType wiit)
    {
        if(displayingWIIT != wiit) return;
        displayingWIIT = null;
        displayingUIComponent = null;
        foreach (var uiComponent in allUIComponent)
        {
            uiComponent.CloseUI();
        }
    }
}
