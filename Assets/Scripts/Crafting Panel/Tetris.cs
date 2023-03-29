using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEditor.PlayerSettings;

public class Tetris : DragInventoryItem
{

    /// <summary>
    /// Wait: Tetris sitting still. 
    /// Drag: Tetris being clicked and dragged around. 
    /// Animation: Tetris moving to snap. 
    /// Merge: Tetris is Merging.
    /// </summary>
    enum state {Wait, Drag, Animation, Merge, CraftPreview}
    enum Zone {Craft, Back}
    Zone zoneNow = Zone.Craft;
    state stateNow = state.Wait;

    //Delta between 
    //public Vector2 recipeFormingDelta;

    //The Scriptable Object this Tetris contains
    public ItemScriptableObject itemSO;
    public RecipeListScriptableObject recipeListSO; // NEW

    //All the edges of this Tetris
    public List<Edge> allEdges = new List<Edge>();

    //These are for click and drag
    Vector2 dragDisplacement = new Vector2(0,0); //Displacement of dragging
    Vector3 tetrisDownPos = new Vector3(0, 0, 0); //Position of Tetris when mouse clicked

    //Standart scale during play, used to snap during merge animation
     Vector3 standardScale = new Vector3(0.2f, 0.2f, 1);

    //Merging Progress Bar
    [SerializeField] GameObject mergeProgressBar;

    //The special effect during merge
    [SerializeField] GameObject craftEffect;

    //Shadowing and Animation
    GameObject shadow = null;
    Vector3 shadowOffsetStandard = new Vector3(-0.1f, -0.1f, 0.2f);

    //To trigger and check recipe-related actions
    public RecipeCombinator myRC;


    private void Start()
    {
        CreateShadow();
    }

    private void Update()
    {
        if(stateNow == state.Drag && Input.GetMouseButton(0)) //DRAG
        {
            transform.position = WorldUtility.GetMouseHitPoint(WorldUtility.LAYER.UI_BACKGROUND, true);
        }

        if(stateNow == state.Drag && Input.GetMouseButtonUp(0))  //RELEASE ON DRAG
        {
            //PlaceDrag();
            if(zoneNow == Zone.Back) //PUT BACK TO INVENTORY
            {
                CraftingManager.i.PutBackToInventory(this.gameObject);
            }
            else //zoneNow == Zone.Craft //DETECT CRAFTING
            {
                SetState(state.Wait);
                RefreshEdges();
                myRC = new RecipeCombinator(this);
                Search(myRC, this, new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0));
                CheckSnap(myRC);
            }
            
        }
    }
    public override void SetUp(UI_InventoryBlock uiib)
    {
        base.SetUp(uiib);
        SetState(state.Drag);
    }

    public void DestroyRC()
    {
        if (myRC == null) return;
        myRC.DestroyCraftPreview();
        myRC = null;
    }


    private void CreateShadow()
    {
        //shadow = new GameObject("shadow of " + gameObject.name);
        shadow = Instantiate(new GameObject("shadow of " + gameObject.name), this.transform);
        //shadow.transform.parent = this.transform;
        shadow.transform.localPosition = new Vector3(0,0,0) + shadowOffsetStandard;
        shadow.transform.localScale = new Vector3(1, 1, 1); //gameObject.transform.localScale;
        //transform.rotation = Quaternion.Euler(0, 0, 0);
        shadow.AddComponent<SpriteRenderer>();
        shadow.GetComponent<SpriteRenderer>().sprite = GetComponent<SpriteRenderer>().sprite;
        shadow.GetComponent<SpriteRenderer>().color = new Color(0, 0, 0);
        shadow.GetComponent<SpriteRenderer>().sortingLayerName = "Crafting System";
        shadow.GetComponent<SpriteRenderer>().sortingOrder = 9;

    }

    private void SetState(state newState)
    {
        //set state of the Tetris
        stateNow = newState;
    }

    private void OnMouseEnter()
    {
        CraftingManager.i.mouseEnterTetris(itemSO);
    }

    private void OnMouseExit()
    {
        CraftingManager.i.mouseExitTetris();
    }

    private void OnMouseDown() // 
    {
        CraftingManager.i.mouseClickTetris();
        if (stateNow != state.Wait) return;
        SetState(state.Drag);
        if(myRC!=null) myRC.DisassembleMerge(this);
        ResetEdges(); //so that rest of the recipe refreshes
        tetrisDownPos = transform.position;
    }

    /*
    public void RefreshEdges()
    {
        foreach (Edge e in allEdges)
        {
            e.RefreshState();
        }
    }*/

    public void RefreshEdges(Tetris excludeTetris = null)
    {
        foreach (Edge e in allEdges)
        {
            e.RefreshState(excludeTetris);
        }
    }

    public void ResetEdges()
    {
        foreach (Edge e in allEdges)
        {
            e.ResetState();
        }
    }


    public IEnumerator MergeProgress(RecipeCombinator rc)
    {
        ItemScriptableObject product = rc.GetMergeISO();
        if (product == null) {
            Debug.LogError("Trying to merge empty Recipe Combinator");
        }

        foreach (Tetris t in rc.GetPastTetris())
        {
            t.startMergeProcess();
        }
        float tCount = 0;
        float tRequire = 1;
        ProgressBar pb = Instantiate(mergeProgressBar, this.transform.position, Quaternion.identity).GetComponent<ProgressBar>();
        pb.transform.position += new Vector3(0, 0, -0.5f);

        while (tCount < tRequire)
        {
            tCount += Time.deltaTime * 5;
            pb.setTo(tCount / tRequire);
            yield return new WaitForSeconds(0);
        }

        //GameObject newTetris = Instantiate(product.myPrefab, rc.CentralPosition(), Quaternion.identity);
        //CraftingManager.i.AddToAllTetris(newTetris);

        CraftingManager.i.CreateTetris(product, rc.CentralPosition(), CraftingManager.CreateFrom.MERGE);

        //2023 02 27 Recipe System to check if there is a unlock // Added by Will
        RecipeMapManager.i.CheckUnlock(product);

        foreach (Tetris t in rc.GetPastTetris())
        {
            t.DestroySelf();
        }
    }

    public void startMergeProcess()
    {
        stateNow = state.Merge;
    }

    public void terminateMergeProcess()
    {
        StartCoroutine(terminateMergeProcessIenumerator());
    }

    IEnumerator terminateMergeProcessIenumerator()
    {
        yield return new WaitForEndOfFrame();
        stateNow = state.Wait;
    }

    /*
    /// <summary>
    /// Recursive search a Tetris, add all connected Tetris to 
    /// </summary>
    /// <param name="rc">The recipe combinator that is passed around to do the combination.</param>
    /// <param name="baseTetris">BaseTetris, for the input for RecipeCombinator.</param>
    /// <param name="baseCor">BaseCoordination, for the input for RecipeCombinator.</param>
    /// <param name="dir">Direction of attachment, for the input for RecipeCombinator.</param>
    /// <param name="newCor">Coordination of new Tetris, for the input for RecipeCombinator.</param>
    void Search(RecipeCombiator rc, Tetris baseTetris, Vector2 baseCor, Vector2 dir, Vector2 newCor)
    {
        print("non exclusive add tetris " + gameObject.name);
        //add Tetris to recipe (embedded repitition check
        rc.AddTetris(baseTetris, this, baseCor, dir, newCor);

        //do this for every edge it has
        List<Edge> toProcess = new List<Edge>(allEdges);
        foreach (Edge e in toProcess)
        {
            if (!e.isConnected()) continue; //if the edge is not connected to anything, skip it.
            if (rc.Searched(e.getOppositeTetris())) continue; //if the connected Tetris of this Edge is already searched, skip it.
            e.getOppositeTetris().Search(rc, this,e.getAttachedCoord(),e.getAttachToDirection(),e.getOppositeEdge().getAttachedCoord()); //Recursively search this edge.
        }
    }*/

    /// <summary>
    /// Recursive search a Tetris, add all connected Tetris to rc
    /// </summary>
    /// <param name="rc">The recipe combinator that is passed around to do the combination.</param>
    /// <param name="baseTetris">BaseTetris, for the input for RecipeCombinator.</param>
    /// <param name="baseCor">BaseCoordination, for the input for RecipeCombinator.</param>
    /// <param name="dir">Direction of attachment, for the input for RecipeCombinator.</param>
    /// <param name="newCor">Coordination of new Tetris, for the input for RecipeCombinator.</param>
    /// <param name="excludeTetris">Exclude Tetris when searching (used when breaking down origional recipe).</param>
    public void Search(RecipeCombinator rc, Tetris baseTetris, Vector2 baseCor, Vector2 dir, Vector2 newCor, Tetris excludeTetris = null)
    {
        //add Tetris to recipe (embedded repitition check
        if (excludeTetris!=null && this == excludeTetris) return;
        rc.AddTetris(baseTetris, this, baseCor, dir, newCor);

        //do this for every edge it has
        List<Edge> toProcess = new List<Edge>(allEdges);
        foreach (Edge e in toProcess)
        {
            if (!e.isConnected()) continue; //if the edge is not connected to anything, skip it.
            if (rc.Searched(e.getOppositeTetris())) continue; //if the connected Tetris of this Edge is already searched, skip it.
            e.getOppositeTetris().Search(rc, this, e.getAttachedCoord(), e.getAttachToDirection(), e.getOppositeEdge().getAttachedCoord(), excludeTetris); //Recursively search this edge.
        }
    }

    void CheckSnap(RecipeCombinator rc)//check if the Tetris is close enough to another to snap them together
    {
        /*
        if (!rc.hasConnector())
        {
            rc.CheckMerging();
            return;
        }*/

        bool moved = false;

        foreach (Edge e in allEdges)
        {
            if (e.isConnected())
            {
                //AudioManager.i.PlaySoundEffectByName("Tetris Snap", true);
                StartCoroutine(SnapMovement(e.getOppositeEdgeDistance()));
                moved = true;
                break;
            }
        }

        if (!moved)
        {
            rc.CheckMerging();
        }

    }

    //born animation
    public void BornSelf()
    {
        transform.localScale = standardScale / 10;
        StartCoroutine(BornSelfProgress());
    }

    IEnumerator BornSelfProgress()
    {
        stateNow = state.Animation;
        while (transform.localScale.x < standardScale.x)
        {
            transform.localScale += standardScale * Time.deltaTime * 3;
            yield return new WaitForSeconds(0);
        }
        transform.localScale = standardScale;
        stateNow = state.Wait;
    }

    //destroy animation
    public void DestroySelf(){StartCoroutine(DestroySelfProcess());}

    IEnumerator DestroySelfProcess()
    {

        CraftingManager.i.RemoveFromTetrisList(gameObject);
        stateNow = state.Animation;
        bool animStart = false;
        float t = 0;
        while (t < 0.5)
        {
            if(animStart == false && t > 0.15)
            {
                Instantiate(craftEffect, this.transform.position, Quaternion.identity);
                
                animStart = true;
            }
            t += Time.deltaTime;
            transform.localScale -= standardScale * Time.deltaTime * 2;
            yield return new WaitForSeconds(0);
        }
        Destroy(gameObject);
    }

    //snapping animation
    IEnumerator SnapMovement(Vector3 delta)
    {
        stateNow = state.Animation;
        Vector3 orgPos = transform.position, tarPos = orgPos+delta;
        float timeCount = 0, timeRequire = 0.2f;

        while (timeCount < timeRequire)
        {
            timeCount += Time.deltaTime;
            transform.position = Vector3.Lerp(orgPos,tarPos,timeCount/timeRequire);
            yield return new WaitForSeconds(0);
        }
        stateNow = state.Wait;
        myRC.Organize();
        myRC.CheckMerging();
    }


    //set zone
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.name != "Background") return;
        CraftingManager.i.mouseExitTetris();
        zoneNow = Zone.Back;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.name != "Background") return;
        zoneNow = Zone.Craft;
    }

    /* 3D
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name != "Background") return;
        CraftingManager.i.mouseExitTetris();
        zoneNow = Zone.Back;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.name != "Background") return;
        zoneNow = Zone.Craft;
    }*/
}
