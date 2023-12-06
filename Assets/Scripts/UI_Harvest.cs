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
        float disappearTime = 2f;
        float disappearTimeLeft = 2f;

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
                float opacityPercent = (float)(percent * 0.25);
                if (child.GetComponent<TextMeshProUGUI>() != null)
                {
                    TextMeshProUGUI textMeshPro = child.GetComponent<TextMeshProUGUI>();
                    Color newColor = textMeshPro.color;
                    newColor.a = opacityPercent;
                    textMeshPro.color = newColor;
                }
                else
                {
                    SpriteRenderer spriteRenderer = child.GetComponent<SpriteRenderer>();
                    Color newColor = spriteRenderer.color;
                    newColor.a = opacityPercent;
                    spriteRenderer.color = newColor;
                }

            }
        }
    }


    //Queue<string> myQueue = new Queue<string>();
    List<HarvestInfo> harvestInfoList = new List<HarvestInfo>();
    [SerializeField] GameObject uiTemplate;
    [SerializeField] private int harvestSetSize = 3;
    [SerializeField] private float moveAmount = 1f;
   

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
        newDisplayGO.transform.localPosition += new Vector3(0, 0.5f, 0) * (harvestInfoList.Count-1);
        if (harvestInfoList.Count >= harvestSetSize)
        {
            RemoveFromFront();
        }
    }

    private void RemoveFromFront()
    {
        harvestInfoList[0].DestroyDisplay();
        harvestInfoList.RemoveAt(0);
    }

    private void MoveUpPosition(HarvestInfo go, int positionIndex)
    {
        float moveSpeed = 1f;
        float moveAmount = 20f;
        Vector3 newPosition = go.displayGO.transform.position;
        newPosition.y = (harvestInfoList[harvestSetSize - 1].displayGO.transform.position.y + (moveAmount * (harvestSetSize - positionIndex))) ;
        go.displayGO.transform.position = Vector3.Lerp(transform.position, newPosition, moveSpeed);
    }

    private void Update()
    {
        if (harvestInfoList.Count != 0)
        {
            if (harvestInfoList[0].SpendTime(Time.deltaTime) == false)
            {
                RemoveFromFront();
            }
            while(harvestInfoList.Count > harvestSetSize)
            {
                RemoveFromFront();
            }

            for (int i = harvestSetSize - 1; i >= 0; i--)
            {
                print("number: " + i);
                harvestInfoList[i].ChangeOpacity(i);
                MoveUpPosition(harvestInfoList[i], i);
            }
        }


    }

}
