using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tetris : DragInventoryItem
{
    /*Wait: Tetris sitting still. 
     * Drag: Tetris being clicked and dragged around. 
     * Animation: Tetris moving to snap. 
     * Merge: Tetris is Merging.
     */
    enum state {Wait, Drag, Animation, Merge, CraftPreview}
    enum Zone {Craft, Back}
    Zone zoneNow = Zone.Craft;
    state stateNow = state.Wait;

    //Delta between 
    Vector2 recipeFormingDelta;

    //The Scriptable Object this Tetris contains
    public ItemScriptableObject itemSO;
    public ItemSOListScriptableObject allItemListSO; //List of all Items TODO: delete this shit
    public RecipeListScriptableObject recipeListSO; // NEW

    //All the edges of this Tetris
    public List<Edge> allEdges = new List<Edge>();

    //These are for click and drag
    public Vector2 dragDisplacement = new Vector2(0,0); //Displacement of dragging
    public Vector3 tetrisDownPos = new Vector3(0, 0, 0); //Position of Tetris when mouse clicked

    //Standart scale during play, used to snap during merge animation
    private Vector3 standardScale = new Vector3(0.2f, 0.2f, 1);

    //Merging Progress Bar
    [SerializeField] GameObject mergeProgressBar;

    //The special effect during merge
    [SerializeField] GameObject craftEffect;

    //Shadowing and Animation
    GameObject shadow = null;
    Vector3 shadowOffsetStandard = new Vector3(-0.1f, -0.1f, 0.2f);

    //To trigger and check recipe-related actions

    public RecipeCombiator myRC;

    //----------------------------- START OF RC -------------------------------------//


    //The class that is passed on during recursive search to combine all the Tetris together and form a recipe
    public class RecipeCombiator
    {
        List<Tetris> pastTetris; //The list of Tetris that is already processed
        List<KeyValuePair<Vector2, ScriptableObject>> recipeGrid; //The final formed recipe in grid form
        Tetris origionTetris;
        ItemScriptableObject mergeISO;
        GameObject mergeWindow;


        public RecipeCombiator(Tetris oT)
        {
            origionTetris = oT;
            pastTetris = new List<Tetris>(); 
            recipeGrid = new List<KeyValuePair<Vector2, ScriptableObject>>();
        }

        /// <summary>
        /// Add this Tetris into the recipe and process it.
        /// </summary>
        /// <param name="baseT">The base Tetris that calls this method.</param>
        /// <param name="newT">The new Tetris to be added to the RecipeCombinator.</param>
        /// <param name="baseCor">Coordination on the Tetris of the connected Edge of the base Tetris.</param>
        /// <param name="dir">Direction of connection from base Tetris to new Tetris</param>
        /// <param name="newCor">Coordination on the Tetris of the connected Edge of the new Tetris.</param>
        public void AddTetris(Tetris baseT, Tetris newT, Vector2 baseCor, Vector2 dir, Vector2 newCor)
        {
            //Avoid Repetition (extra prevention
            if (Searched(newT)) return;

            pastTetris.Add(newT);
            newT.DestroyRC();
            newT.myRC = this;

            //Is it the first Tetris to be added (the Tetris that create this RecipeCombinator)
            if (baseT == newT)
            {
                newT.recipeFormingDelta = new Vector2(0, 0);
                List<KeyValuePair<Vector2, ScriptableObject>> toSearch = newT.itemSO.FormationRecipeCoord;
                foreach (KeyValuePair<Vector2, ScriptableObject> kvp in toSearch)
                {
                    recipeGrid.Add(kvp);
                }
                return;
            }
            //It is not the first Tetris to be added.
            else
            {
                //Calculate and record Delta
                Vector2 delta = baseCor + dir - newCor + baseT.recipeFormingDelta;
                newT.recipeFormingDelta = delta;

                //Load into final receipe 
                foreach (KeyValuePair<Vector2, ScriptableObject> kvp in newT.itemSO.FormationRecipeCoord)
                {
                    recipeGrid.Add(new KeyValuePair<Vector2, ScriptableObject>(kvp.Key + delta, kvp.Value));
                }
            }
        }

        public void CheckMerging()
        {
            ItemScriptableObject product = null;

            foreach (ItemCraftScriptableObject icso in origionTetris.recipeListSO.list)
            {
                if (!(icso.CraftingStationRequired != null && !CraftingManager.i.TetrisInPresent(icso.CraftingStationRequired)) && icso.CheckMatch(getRecipeGrid())) //find the scriptableobject with same recipe, if there is one
                {
                    product = icso.ItemCrafted;
                    break;
                }
            }
            mergeISO = product;

            if (mergeISO != null)
            {
                mergeWindow = CraftingManager.i.CreateMergeWindow(this);
            }

        }

        public void DisassembleMerge(Tetris DisassembleFrom)
        {

            DestroyCraftPreview();
            foreach (Tetris t in DisassembleFrom.myRC.pastTetris)
            {
                if (t != DisassembleFrom)
                {
                    t.myRC = null;
                }
            }
            foreach (Tetris t in DisassembleFrom.myRC.pastTetris)
            {
               if(t.myRC == null && t != DisassembleFrom)
                {
                    t.RefreshEdges(DisassembleFrom);
                    t.myRC = new RecipeCombiator(t);
                    t.Search(t.myRC, t, new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0),DisassembleFrom);
                    t.myRC.Organize();
                    t.myRC.CheckMerging();
                }
            }
            DisassembleFrom.myRC.pastTetris.Clear();
            DisassembleFrom.myRC = null;
        }

        /// <summary>
        /// Has this Tetris been searched and added yet?
        /// </summary>
        /// <param name="t">The Tetris to be checked.</param>
        /// <returns></returns>
        public bool Searched(Tetris t)
        {
            return pastTetris.Contains(t);
        }

        /// <summary>
        /// Get the abstract central position of the Tetris's gameobject.
        /// </summary>
        /// <returns></returns>
        public Vector3 CentralPosition()
        {
            Vector3 export = new Vector3(0, 0, 0);
            foreach(Tetris t in pastTetris)
            {
                export += t.transform.position;
            }
            return export/pastTetris.Count;
        }

        /// <summary>
        /// Organize the formed recipe, move all the nodes so that there are no negative coordinations.
        /// </summary>
        public void Organize()
        {
            Vector2 leftNTopBound = new Vector2(0, 0);
            foreach (KeyValuePair<Vector2, ScriptableObject> kvp in recipeGrid)
            {
                if (kvp.Key.x < leftNTopBound.x) leftNTopBound.x = kvp.Key.x;
                if (kvp.Key.y < leftNTopBound.y) leftNTopBound.y = kvp.Key.y;
            }
            List<KeyValuePair<Vector2, ScriptableObject>> editedRecipeGrid = new List<KeyValuePair<Vector2, ScriptableObject>>();
            foreach (KeyValuePair<Vector2, ScriptableObject> kvp in recipeGrid)
            {
                editedRecipeGrid.Add(new KeyValuePair<Vector2, ScriptableObject>(kvp.Key - leftNTopBound, kvp.Value));
            }
            recipeGrid = editedRecipeGrid;
        }

        /// <summary>
        /// Does the base Tetris has any other Tetirs connects to it?
        /// </summary>
        /// <returns></returns>
        public bool hasConnector()
        {
            return (pastTetris.Count != 1);
        }

        /// <summary>
        /// Print out a string representaion of all the nodes in the recipe.
        /// </summary>
        /// <returns></returns>
        public void DebugPrint()
        {
            foreach (KeyValuePair<Vector2, ScriptableObject> kvp in recipeGrid)
            {
                print(kvp.Key + " " + kvp.Value.name);
            }
        }

        public void Merge()
        {
            if (true) //merge from inventory
            {
                
            }
            else //merge the inspected one
            {
               
            }
            origionTetris.StartCoroutine(origionTetris.MergeProgress(this));
            DestroyCraftPreview();
        }

        public void DestroyCraftPreview()
        {
            if(mergeWindow != null)
            {
                Destroy(mergeWindow);
                mergeWindow = null;
            }
        }

        public bool IsOrigionTetris(Tetris thisTetris)
        {
            return thisTetris == origionTetris;
        }

        /// <summary>
        /// Return the grid representation of recipe.
        /// </summary>
        /// <returns></returns>
        public List<KeyValuePair<Vector2, ScriptableObject>> getRecipeGrid() { return recipeGrid; }

        /// <summary>
        /// Get all the Tetris that has been processed before.
        /// </summary>
        /// <returns></returns>
        public List<Tetris> GetPastTetris() { return pastTetris; }

        public ItemScriptableObject GetMergeISO() { return mergeISO; }
    }

    //----------------------------- END OF RC -------------------------------------//



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
            if(zoneNow == Zone.Back) //PUT BACK TO INVENTORY
            {
                CraftingManager.i.PutBackToInventory(this.gameObject);
            }
            else //zoneNow == Zone.Craft //DETECT CRAFTING
            {
                SetState(state.Wait);
                RefreshEdges();
                myRC = new RecipeCombiator(this);
                Search(myRC, this, new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0));
                CheckSnap(myRC);
                myRC.Organize();
                myRC.CheckMerging();
            }
            
        }
    }

    protected override void CustomSetUp()
    {
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
        myRC.DisassembleMerge(this);
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


    public IEnumerator MergeProgress(RecipeCombiator rc)
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
    /// Recursive search a Tetris, add all connected Tetris to 
    /// </summary>
    /// <param name="rc">The recipe combinator that is passed around to do the combination.</param>
    /// <param name="baseTetris">BaseTetris, for the input for RecipeCombinator.</param>
    /// <param name="baseCor">BaseCoordination, for the input for RecipeCombinator.</param>
    /// <param name="dir">Direction of attachment, for the input for RecipeCombinator.</param>
    /// <param name="newCor">Coordination of new Tetris, for the input for RecipeCombinator.</param>
    /// <param name="excludeTetris">Exclude Tetris when searching (used when breaking down origional recipe).</param>
    void Search(RecipeCombiator rc, Tetris baseTetris, Vector2 baseCor, Vector2 dir, Vector2 newCor, Tetris excludeTetris = null)
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

    void CheckSnap(RecipeCombiator rc)//check if the Tetris is close enough to another to snap them together
    {
        if (!rc.hasConnector()) return;
        foreach (Edge e in allEdges)
        {
            if (e.isConnected())
            {
                //AudioManager.i.PlaySoundEffectByName("Tetris Snap", true);
                StartCoroutine(SnapMovement(e.getOppositeEdgeDistance()));
                break;
            }
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
