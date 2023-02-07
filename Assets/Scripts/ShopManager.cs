using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopManager : MonoBehaviour
{
    bool skirt = false, lChair = false, rChair = false, table = false, lemonTea = false, taroTea = false, honeyTea = false, redBeanTea = false;
    [SerializeField] GameObject Skirt, LChair, RChair, Table, LemonTea1, LemonTea2, TaroTea1, TaroTea2, HoneyTea1, HoneyTea2, RedBeanTea1, RedBeanTea2;
    [SerializeField] ItemScriptableObject isoSkirt, isoChair, isoTable, isoLemonTea, isoTaroTea, isoHoneyTea, isoRedBeanTea;

    static ShopManager instance;
    public static ShopManager i
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<ShopManager>();
            }
            return instance;
        }
    }

    public void newObj(ItemScriptableObject iso)
    {
        if (iso == isoSkirt && !Skirt.activeSelf) bornGO(Skirt);
        if (iso == isoTable && !Table.activeSelf) bornGO(Table);

        if(iso == isoChair)
        {
            if (!RChair.activeSelf) bornGO(RChair);
            else if (!LChair.activeSelf) bornGO(LChair);
        }

        if(iso == isoLemonTea && !LemonTea1.activeSelf)
        {
            bornGO(LemonTea1);
            bornGO(LemonTea2);
        }
        if (iso == isoHoneyTea && !HoneyTea1.activeSelf)
        {
            bornGO(HoneyTea1);
            bornGO(HoneyTea2);
        }
        if (iso == isoTaroTea && !TaroTea1.activeSelf)
        {
            bornGO(TaroTea1);
            bornGO(TaroTea2);
        }
        if (iso == isoRedBeanTea && !RedBeanTea1.activeSelf)
        {
            bornGO(RedBeanTea1);
            bornGO(RedBeanTea2);
        }
    }

    void bornGO(GameObject go)
    {
        go.SetActive(true);
        MasterManager.i.GoToShop();
    }


}
