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
        GameObject displayGO;
        float disappearTime = 5;
        float disappearTimeLeft = 5f;

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

    }

    List<HarvestInfo> harvestInfoList = new List<HarvestInfo>();
    [SerializeField] GameObject uiTemplate;

    public void AddItem(ItemScriptableObject iso, int amount)
    {
        print(harvestInfoList);
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
        newDisplayGO.transform.position += new Vector3(0, 0.5f, 0) * harvestInfoList.Count;

    }

    private void Update()
    {
        for(int i = harvestInfoList.Count - 1; i >=0 ; i--)
        {
            if(harvestInfoList[i].SpendTime(Time.deltaTime) == false)
            {
                harvestInfoList[i].DestroyDisplay();
                harvestInfoList.Remove(harvestInfoList[i]);
            }
        }
    }

}
