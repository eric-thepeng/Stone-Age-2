using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using TMPro;

public class CraftingManager : SerializedMonoBehaviour
{
    Transform topLeft, bottomRight;
    [SerializeField] List<ItemScriptableObject> startingTetris = new List<ItemScriptableObject>();
    [SerializeField] ItemScriptableObject axeISO, shovelISO;

    //[SerializeField] Dictionary<ItemScriptableObject, int> testDic = new Dictionary<ItemScriptableObject, int>();

    public List<GameObject> allTetris = new List<GameObject>();
    float unitLength = 0.15f;

    static CraftingManager instance;
    public static CraftingManager i
    {
        get
        {
            if(instance == null)
            {
                instance = FindObjectOfType<CraftingManager>();
            }
            return instance;
        }
    }

    private void Start()
    {
        topLeft = transform.Find("Crafting Background").Find("Top Left");
        bottomRight = transform.Find("Crafting Background").Find("Bottom Right");
        foreach(ItemScriptableObject iso in startingTetris)
        {
            AddToCrafting(iso.myPrefab);
        }
    }

    public void AddToCrafting(GameObject go)
    {
        GameObject newTetris = Instantiate(go,transform);
        newTetris.transform.position = AvaiableTetrisPos();
        newTetris.transform.localScale = new Vector3(0.15f, 0.15f, 1f);
        allTetris.Add(newTetris);
    }

    public void AddToAllTetris(GameObject go)
    {
        allTetris.Add(go);
        ItemScriptableObject thisISO = go.GetComponent<Tetris>().itemSO;
        if (thisISO == axeISO)
        {
            UI_Tools.i.addAxe();
        }
        else if(thisISO == shovelISO)
        {
            UI_Tools.i.addShovel();
        }
        ShopManager.i.newObj(thisISO);
        TaskManager.i.CheckTasksComplete();
    }

    Vector3 AvaiableTetrisPos()
    {
        Vector3 tempPos = RandomTetrisPos();
        bool avaiable = false;
        int tryCount = 0;
        while (avaiable == false && tryCount <= 50) 
        {
            tempPos = RandomTetrisPos();
            avaiable = true;
            foreach (GameObject go in allTetris)    
            {
                if((go.transform.position - tempPos).magnitude < unitLength * 4)
                {
                    avaiable = false;
                    break;
                }
            }
            tryCount += 1;
        }
        return tempPos;
    }

    Vector3 RandomTetrisPos()
    {
        return new Vector3(Random.Range(0.05f, 0.95f) * (topLeft.position.x - bottomRight.position.x) + bottomRight.position.x, Random.Range(0.05f, 0.95f) * (topLeft.position.y - bottomRight.position.y) + bottomRight.position.y, -0.5f);
    }
    public void RemoveFromCrafting(GameObject go)
    {
        allTetris.Remove(go);
    }

    public int CheckAmountISO(ItemScriptableObject toCheck)
    {
        int output = 0;

        foreach(GameObject go in allTetris)
        {
            if (go.GetComponent<Tetris>().itemSO == toCheck) output += 1;
        }

        return output;
    }

    public void mouseEnterTetris(ItemScriptableObject iso)
    {
        Vector3 toSet = new Vector3(Camera.main.ScreenToWorldPoint(Input.mousePosition).x, Camera.main.ScreenToWorldPoint(Input.mousePosition).y, transform.Find("NameUI").transform.position.z);
        transform.Find("NameUI").transform.position = toSet;
        transform.Find("NameUI").gameObject.SetActive(true);
        transform.Find("NameUI").gameObject.GetComponentInChildren<TextMeshPro>().text = iso.tetrisHoverName;

    }
    public void mouseClickTetris()
    {
        transform.Find("NameUI").gameObject.SetActive(false);
    }
    public void mouseExitTetris()
    {
        transform.Find("NameUI").gameObject.SetActive(false);
    }

    public void SetTaskCompleteNotification(bool setTo)
    {
        print("setting to " + setTo);
        if (setTo)
        {
            transform.Find("Task Complete Notification").gameObject.SetActive(true);
        }
        else
        {
            transform.Find("Task Complete Notification").gameObject.SetActive(false);
        }
    }
}
