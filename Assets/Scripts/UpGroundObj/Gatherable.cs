using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.U2D.Animation;

public class Gatherable : UpGroundObj
{
    [SerializeField] SpriteLibrary spriteLibrary;
    [SerializeField] SpriteResolver targetResolver;
    SpriteLibraryAsset LibraryAsset => spriteLibrary.spriteLibraryAsset;

    [SerializeField] string targetCategroy;
    [SerializeField] string fullSpriteLabel;
    [SerializeField] string emptySpriteLabel;
    public enum state {Gathering, Full, Empty}
    public enum toolMatchness {Normal, Stuck}
    public enum type {Berry, Grass, Tree, SmallTree}
    public type myType = type.Tree;
    state stateNow;
    public state bornState;

    [SerializeField] GameObject objProduce;
    [SerializeField]List<ItemScriptableObject> produceList = new List<ItemScriptableObject>();

    Animator animator;

    [SerializeField] float gatherTimeRequire = 1;
    float gatherTimeCount = 0;
    float lastGatherTimeCount = 0;

    private void Awake()
    {
        spriteLibrary = GetComponent<SpriteLibrary>();
        targetResolver = GetComponent<SpriteResolver>();
        animator = GetComponent<Animator>();
    }
    void Start()
    {
       BornAs(bornState);
    }
    void BornAs(state bornState)
    {
        if (bornState == state.Gathering)
        {
            print("error: born as gathering");
            return;
        }
        stateNow = bornState;
        SetSpriteToCurrentState();
    }
    void SetSpriteToCurrentState()
    {
        
        if(stateNow == state.Full)
        {
            targetResolver.SetCategoryAndLabel(targetCategroy, fullSpriteLabel);
        }
        else if(stateNow == state.Empty)
        {
            print("switch to empty");
            targetResolver.SetCategoryAndLabel(targetCategroy, emptySpriteLabel);
        }
    }
    void ChangeStateTo(state newState)
    {
        if (stateNow == newState) return;
        else
        {
            stateNow = newState;
            SetSpriteToCurrentState();
            if(stateNow == state.Gathering) animator.SetBool("Gathering", true);
            else animator.SetBool("Gathering", false);
        } 
    }
    public void Gather(PlayerAction.tool inTool) 
    {
        if(stateNow == state.Empty)
        {
            EmptyGather();
            return;
        }
        ChangeStateTo(state.Gathering);

        
        if(GetToolMatchness(inTool) == toolMatchness.Stuck)
        {
            if (gatherTimeCount >=  gatherTimeRequire * 0.2f)
            {
                //do nothing -> add stuck effect
            }
            else
            {
                gatherTimeCount += Time.deltaTime;
            }
        }
        else
        {
            gatherTimeCount += Time.deltaTime;
        }
        DigUI.i.SetRatio(gatherTimeCount/gatherTimeRequire);
        if(gatherTimeCount >= gatherTimeRequire)
        {
            Produce();
        }
    }
    private void Update()
    {
        if(stateNow == state.Gathering && gatherTimeCount == lastGatherTimeCount)
        {
            AbandonGather();
        }
        lastGatherTimeCount = gatherTimeCount;
    }
    public void AbandonGather() 
    {
        if (stateNow == state.Empty) return;
        ChangeStateTo(state.Full);
        gatherTimeCount = 0;
        DigUI.i.Clear();
    }
    public void EmptyGather() { }

    public void Grow()
    {
        ChangeStateTo(state.Full);
    }
    public void Produce()
    {
        DigUI.i.Clear();
        ChangeStateTo(state.Empty);
        if(objProduce != null)
        {
            StartCoroutine("ProduceIE");
        }
    }

    IEnumerator ProduceIE()
    {
        AudioManager.i.PlaySoundEffectByName("Get Resource", true);
        foreach(ItemScriptableObject iso in produceList)
        {
            GameObject ngo = Instantiate(objProduce, transform.parent);
            ngo.transform.position = transform.position;
            ngo.GetComponent<mushroom>().SetUp(iso.objectSprite,iso, 0.7f);
            yield return new WaitForSeconds(0.5f);
        }
    }

    public bool CheckEmpty() { return stateNow == state.Empty; }

    public toolMatchness GetToolMatchness(PlayerAction.tool inTool)
    {
        if(myType == type.Berry) //berry：只能用手
        {
            if (inTool == PlayerAction.tool.Hand) return toolMatchness.Normal;
        }
        else if(myType == type.Grass) //grass:：只能shovel
        {
            if (inTool == PlayerAction.tool.Shovel) return toolMatchness.Normal;
        }
        else if(myType == type.Tree) //tree：只能axe
        {
            if (inTool == PlayerAction.tool.Axe) return toolMatchness.Normal;
        }
        else if (myType == type.SmallTree) //small tree: 斧子或者手
        {
            if (inTool == PlayerAction.tool.Axe) return toolMatchness.Normal;
            if (inTool == PlayerAction.tool.Hand) return toolMatchness.Normal;
        }
        return toolMatchness.Stuck;
    }

}
