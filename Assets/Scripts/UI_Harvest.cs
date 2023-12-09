using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UI_Harvest : MonoBehaviour
{
    static UI_Harvest instance;
    public static UI_Harvest i
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<UI_Harvest>();
            }
            return instance;
        }
    }

    class HarvestInfo
    {
        ItemScriptableObject iso;
        int amount;
        public GameObject displayGO;
        float disappearTime = 4f;
        float disappearTimeLeft = 4f;

        public HarvestInfo(ItemScriptableObject newIso, int initialAmount, GameObject go)
        {
            iso = newIso;
            amount = initialAmount;
            displayGO = go;
            go.transform.Find("Icon").GetComponent<SpriteRenderer>().sprite = iso.iconSprite;
            go.transform.Find("Text").GetComponent<TextMeshPro>().text = iso.tetrisHoverName + " + " + amount;
        }

        public void AddAmount(int am)
        {
            ResetTime();
            amount += am;
            displayGO.transform.Find("Text").GetComponent<TextMeshPro>().text = DisplayText();
        }

        public void ResetTime()
        {
            disappearTimeLeft = disappearTime;
        }

        public bool SpendTime(float delta)
        {
            disappearTimeLeft -= delta;
            if (disappearTimeLeft < 0) return false;
            return true;
        }

        public bool IsISO(ItemScriptableObject checkIso)
        {
            return iso == checkIso;
        }

        public string DisplayText()
        {
            return iso.tetrisHoverName + " + " + amount;
        }

        public void DestroyDisplay()
        {
            Destroy(displayGO);
        }

        //change opacity based on the position in the queue
        public void ChangeOpacity(float percent)
        {
            foreach (Transform child in displayGO.transform)
            {
                float opacityPercent = (float)(0.1 + 1 - percent * 0.3);
                if (child.GetComponent<TextMeshProUGUI>() != null)
                {
                    TextMeshProUGUI textMeshPro = child.GetComponent<TextMeshProUGUI>();
                    Color newColor = textMeshPro.color;
                    newColor.a = opacityPercent;
                    textMeshPro.color = newColor;
                }
                else if (child.GetComponent<SpriteRenderer>() != null)
                {
                    SpriteRenderer spriteRenderer = child.GetComponent<SpriteRenderer>();
                    Color newColor = spriteRenderer.color;
                    newColor.a = opacityPercent;
                    spriteRenderer.color = newColor;
                }

            }
        }



    }


    List<HarvestInfo> harvestInfoList = new List<HarvestInfo>();
    [SerializeField] GameObject uiTemplate;
    [SerializeField] private int harvestSetSize = 3;
    [Header("Mode 0 is w Coroutine(bug), Mode 1 is without")]
    [SerializeField] private int mode = 1;
    //[SerializeField] private float moveAmount = 1f;
   

    public void AddItem(ItemScriptableObject iso, int amount)
    {
        foreach(HarvestInfo hi in harvestInfoList) //add amount
        {
            if (hi.IsISO(iso))
            {
                hi.AddAmount(amount);
                return;
            }
        }
        //create new
        GameObject newDisplayGO = Instantiate(uiTemplate,this.transform);
        harvestInfoList.Add(new HarvestInfo(iso, amount, newDisplayGO));
        newDisplayGO.SetActive(true);
        //newDisplayGO.transform.localPosition += new Vector3(0, 0.5f, 0) * (harvestInfoList.Count-1);
        if (harvestInfoList.Count > harvestSetSize)
        {
            RemoveFromFront();
        }
        switch (mode)
        {
            case 0:
                StartCoroutine(UpdatePosition2());
                break;
            case 1:
                UpdatePosition();
                break;
        }
        for (int i = 0; i < harvestInfoList.Count; i++)
        {
            harvestInfoList[i].ChangeOpacity(harvestInfoList.Count - 1 - i);
        }
    }

    private void RemoveFromFront()
    {
        harvestInfoList[0].DestroyDisplay();
        harvestInfoList.RemoveAt(0);
    }

    private void UpdatePosition()
    {
        float moveAmount = 0.8f;

        for (int i = 0; i < harvestInfoList.Count - 1; i++)
         {
            Vector3 _pos = harvestInfoList[i].displayGO.transform.position;
            _pos.y = (harvestInfoList[harvestInfoList.Count - 1].displayGO.transform.position.y + ((harvestInfoList.Count - 1 - i) * moveAmount));
            harvestInfoList[i].displayGO.transform.position = _pos;
         }
    }

    private IEnumerator UpdatePosition2()
    {
        float moveSpeed = 2f;
        float moveAmount = 0.8f;

        Vector3 startPosition = harvestInfoList[harvestInfoList.Count - 1].displayGO.transform.position;

        for (int i = 0; i < harvestInfoList.Count - 1; i++)
        {
            Vector3 _pos = startPosition;
            _pos.y = startPosition.y + (harvestInfoList.Count - 1 - i) * moveAmount;
            while (Vector3.Distance(harvestInfoList[i].displayGO.transform.position, _pos) > 0.01f)
            {
                harvestInfoList[i].displayGO.transform.position = Vector3.Lerp(harvestInfoList[i].displayGO.transform.position, _pos, Time.deltaTime * moveSpeed);
                yield return null;
            }
            harvestInfoList[i].displayGO.transform.position = _pos;
        }
    }

    private void Update()
    {
        //print("number: " + harvestInfoList.Count);
        if (harvestInfoList.Count != 0)
        {
            if (harvestInfoList[0].SpendTime(Time.deltaTime) == false)
            {
                RemoveFromFront();
                if(harvestInfoList.Count != 0)
                {
                    switch (mode)
                    {
                        case 0:
                            StartCoroutine(UpdatePosition2());
                            break;
                        case 1:
                            UpdatePosition();
                            break;
                    }
                    for (int i = 0; i < harvestInfoList.Count; i++)
                    {
                        harvestInfoList[i].ChangeOpacity(harvestInfoList.Count - 1 - i);
                    }
                }
            }

            while(harvestInfoList.Count > harvestSetSize)
            {
                RemoveFromFront();
            }


        }


    }

}
