using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UniversalUIManager : MonoBehaviour
{
    public class UniversalUIComponent
    {
        public enum Identifier
        {
            LongPress,
            Click
        }

        public Identifier identifier;

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
        myLongPressUI.SetPosition();
    }

    public void DisplaySingleComponent(UniversalUIComponent.Identifier id, float value)
    {
        foreach (var uiComponent in allUIComponent)
        {
            if (uiComponent.identifier == id)
            {
                uiComponent.OpenUI();
                uiComponent.SetValue(value);
            }
            else
            {
                uiComponent.CloseUI();
            }
        }
    }
}
