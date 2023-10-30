using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UniversalUIManager : MonoBehaviour
{
    public class UniversalUIComponents
    {
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
        
    }
    
    [Serializable]
    public class LongPressUI : UniversalUIComponents
    {
        public Slider slider;
        public Image centerImage;
        
        public void AdjustPercentage(float percent)
        {
            slider.value = percent;
        }
    }

    public LongPressUI myLongPressUI;

    private void Start()
    {
        myLongPressUI.Initialize();
    }

    private void Update()
    {
        myLongPressUI.SetPosition();
    }
}
