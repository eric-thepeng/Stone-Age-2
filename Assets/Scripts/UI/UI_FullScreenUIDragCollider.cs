using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;

public class UI_FullScreenUIDragCollider : MonoBehaviour
{
    static UI_FullScreenUIDragCollider instance;
    public static UI_FullScreenUIDragCollider i
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<UI_FullScreenUIDragCollider>();
            }
            return instance;
        }
    }
    
    private MonoBehaviour openedBy = null;
    [SerializeField] private GameObject collider;

    public void Open(MonoBehaviour triggerMonobehaviour)
    {
        openedBy = triggerMonobehaviour;
        collider.SetActive(true);
    }

    public void Close()
    {
        openedBy = null;
        collider.SetActive(false);
    }

    public bool IsOpened()
    {
        return openedBy != null;
    }
    
}
