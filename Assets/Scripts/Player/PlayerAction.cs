using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerAction : MonoBehaviour
{
    UpGroundObj tarObj;
    //Gatherable tarGatherable;
    CapsuleCollider myCollider;
    GameObject tarInfo;
    public enum tool {Hand, Shovel, Axe}
    tool toolNow = tool.Hand;
    private void Awake()
    {
        myCollider = GetComponent<CapsuleCollider>();
    }

    private void Update()
    {

        //switch tools
        if (Input.GetMouseButtonDown(1))
        {
            //Hand -> Shovel -> Axe -> Hand
            if (toolNow == tool.Hand)
            {
                if (UI_Tools.i.hasShovel())
                {
                    toolNow = tool.Shovel;
                    UI_Tools.i.switchTool(2);
                    worldToolUISwitch(2);
                }
                else if (UI_Tools.i.hasAxe())
                {
                    toolNow = tool.Axe;
                    UI_Tools.i.switchTool(3);
                    worldToolUISwitch(3);
                }
            }
            else if (toolNow == tool.Shovel)
            {
                if (UI_Tools.i.hasAxe())
                {
                    toolNow = tool.Axe;
                    UI_Tools.i.switchTool(3);
                    worldToolUISwitch(3);
                }
                else
                {
                    toolNow = tool.Hand;
                    UI_Tools.i.switchTool(1);
                    worldToolUISwitch(1);
                }
            }
            else if (toolNow == tool.Axe)
            {
                toolNow = tool.Hand;
                UI_Tools.i.switchTool(1);
                worldToolUISwitch(1);
            }
        }

        if (!MasterManager.i.inExploration()) { return; }

        if (tarObj == null) return;
        if (tarObj.isGatherable() != null)
        {
            if (Input.GetMouseButton(0))
            {
                tarInfo.SetActive(false);
                tarObj.isGatherable().Gather(toolNow);
            }
            if (tarObj.isGatherable().CheckEmpty())
            {
                tarObj = null;
                refreshCollider();
                tarInfo.SetActive(false);
            }
        }
        else if (tarObj.isPickable() !=null)
        {
            if (Input.GetMouseButtonDown(0))
            {
                tarInfo.SetActive(false);
                tarObj.isPickable().PickUp();
            }
        }
        else if(tarObj.isHome() != null)
        {
            if (Input.GetMouseButtonDown(0))
            {
                tarInfo.SetActive(false);
                tarObj.isHome().GoHome();
            }
        }

        
    }

    /// <summary>
    /// switch world tool ui, 1: hand 2: shovel 3: axe
    /// </summary>
    /// <param name="to"></param>
    void worldToolUISwitch(int to)
    {
        if(to == 1)
        {
            transform.Find("Tool Info").Find("Hand").gameObject.SetActive(true);
            transform.Find("Tool Info").Find("Axe").gameObject.SetActive(false);
            transform.Find("Tool Info").Find("Shovel").gameObject.SetActive(false);
        } else if (to == 2)
        {
            transform.Find("Tool Info").Find("Hand").gameObject.SetActive(false);
            transform.Find("Tool Info").Find("Axe").gameObject.SetActive(false);
            transform.Find("Tool Info").Find("Shovel").gameObject.SetActive(true);

        } else if (to == 3)
        {
            transform.Find("Tool Info").Find("Hand").gameObject.SetActive(false);
            transform.Find("Tool Info").Find("Axe").gameObject.SetActive(true);
            transform.Find("Tool Info").Find("Shovel").gameObject.SetActive(false);

        }

    }
    void refreshCollider()
    {
        myCollider.enabled = false;
        myCollider.enabled = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (tarObj == null && other.gameObject.GetComponent<UpGroundObj>() != null)
        {
            tarInfo = transform.Find("Target Info").gameObject;
            tarInfo.SetActive(true);
            tarInfo.GetComponent<TextMeshPro>().text = other.GetComponent<UpGroundObj>().displayName;//other.gameObject.name;

            tarObj = other.gameObject.GetComponent<UpGroundObj>();
        }
    }

    private void OnTriggerExit(Collider other) //exit a area
    {
        if (other.gameObject.GetComponent<UpGroundObj>() == null) return; //ignore if not UpGroundObj

        if(tarObj != null && tarObj.isGatherable()) //isGatherable and is tarGatherable, abandonGather and dislink
        {
            tarObj.isGatherable().AbandonGather();
        }

        tarInfo.SetActive(false);
        //CanvasManager.i.CloseTargetInfo();
        tarObj = null;
    }
}
