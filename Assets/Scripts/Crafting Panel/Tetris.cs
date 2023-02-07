using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tetris : MonoBehaviour
{
    //Wait: Tetris sitting still. Drag: Tetris being clicked and dragged around. Animation: Tetris moving to snap. Merge: Tetris is Merging.
    enum state {Wait, Drag, Animation, Merge, Lift, Drop}
    state stateNow = state.Wait;

    //Delta between 
    Vector2 recipeFormingDelta;

    //The Scriptable Object this Tetris contains
    public ItemScriptableObject itemSO;
    public ItemSOListScriptableObject allItemListSO; //List of all Items
    public ItemSOListScriptableObject craftRecipeSO;
    public GroundRecipeScriptableObject groundRecipeSO; //List of all Grounding Recipe
    public GroundRecipeScriptableObject boilRecipeSO; //List of all Boiling Recipe

    //All the edges of this Tetris
    public List<Edge> allEdges = new List<Edge>();

    //These are for click and drag
    public Vector2 dragDisplacement = new Vector2(0,0); //Displacement of dragging
    public Vector3 mouseDownPos = new Vector2(0, 0); //Position that mouse clicked
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

    //The class that is passed on during recursive search to combine all the Tetris together and form a recipe
    public class RecipeCombiator
    {
         enum mode {Combine, Grind, Cook }
         mode Mode;
        GameObject mergeProgressBar;
        List<Tetris> pastTetris; //The list of Tetris that is already processed
        List<KeyValuePair<Vector2, ScriptableObject>> recipeGrid; //The final formed recipe in grid form
         Tetris origionTetris;

        //Create and initialize the two variables.
        public RecipeCombiator(Tetris oT, GameObject mpb)
        {
            origionTetris = oT;
            pastTetris = new List<Tetris>(); 
            recipeGrid = new List<KeyValuePair<Vector2, ScriptableObject>>();
            mergeProgressBar = mpb;
            Mode = mode.Combine;
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
            if(Mode == mode.Grind || Mode == mode.Cook)
            {
                return;
            }

            if (newT.itemSO.isGround)
            {
                Mode = mode.Grind;
                return;
            }
           else if (newT.itemSO.isCook)
            {
                Mode = mode.Cook;
                return;
            }

            //Avoid Repetition (extra prevention
            if (Searched(newT)) return;
            pastTetris.Add(newT);

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

       

        /// <summary>
        /// Has this Tetris been searched and added yet?
        /// </summary>
        /// <param name="t">The Tetris to be checked.</param>
        /// <returns></returns>
        public bool Searched(Tetris t)
        {
            return (pastTetris.Contains(t));
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
            BindRCForAll();
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

        public void StartMerge(ItemScriptableObject iso)
        {
            origionTetris.StartCoroutine(origionTetris.MergeProgress(this,iso));
        }

        public void StopMerge()
        {
            foreach (Tetris t in getPastTetris())
            {
                t.terminateMergeProcess();
            }
            origionTetris.StopAllCoroutines();
        }

        public void BindRCForAll()
        {
            foreach(Tetris t in getPastTetris())
            {
                t.myRC = this;
            }
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
        public List<Tetris> getPastTetris() { return pastTetris; }

        public bool isGrind() { return Mode == mode.Grind; }
        public bool isCook() { return Mode == mode.Cook; }
    }


    private void Start()
    {
        CreateShadow();
        //BornSelf();
    }

    private void CreateShadow()
    {
        shadow = new GameObject("shadow of " + gameObject.name);
        //shadow.transform.position = gameObject.transform.position + shadowOffsetStandard;
        shadow.transform.parent = this.transform;
        shadow.transform.localPosition = new Vector3(0,0,0) + shadowOffsetStandard;
        shadow.transform.localScale = new Vector3(1, 1, 1); //gameObject.transform.localScale;
        shadow.AddComponent<SpriteRenderer>();
        shadow.GetComponent<SpriteRenderer>().sprite = GetComponent<SpriteRenderer>().sprite;
        shadow.GetComponent<SpriteRenderer>().color = new Color(0, 0, 0);

    }

    

    private Vector3 GetMouseWorldPos()
    {
        //return mouse position through main camera
        return Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.transform.position.z * -1));
        //return CameraManager.i.getPanelCamera().ScreenToViewportPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, CameraManager.i.getPanelCamera().transform.position.z * -1));
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

    private void OnMouseDown()
    {
        CraftingManager.i.mouseClickTetris();
        if(stateNow == state.Merge)
        {
            //myRC.StopMerge();
        }
        if (stateNow != state.Wait) return;
        SetState(state.Drag);
        ResetEdges(); //so that rest of the recipe refreshes
        mouseDownPos = GetMouseWorldPos();
        tetrisDownPos = transform.position;
    }

    private void OnMouseDrag()
    {
        if (stateNow != state.Drag) return;
        transform.position = tetrisDownPos + (GetMouseWorldPos() - mouseDownPos);
    }

    private void OnMouseUp() //Real job of combining/calculating crafting and recipe
    {
        if (stateNow != state.Drag) return;
        SetState(state.Wait);
        RefreshEdges();
        RecipeCombiator rc = new RecipeCombiator(this,mergeProgressBar);
        Search(rc, this, new Vector2(0,0), new Vector2(0, 0), new Vector2(0, 0));
        CheckSnap(rc);
        rc.Organize();
        CheckRecipe(rc);
    }

    public void RefreshEdges()
    {
        foreach (Edge e in allEdges)
        {
            e.RefreshState();
        }
    }

    public void ResetEdges()
    {
        foreach (Edge e in allEdges)
        {
            e.ResetState();
        }
    }

    void CheckRecipe(RecipeCombiator rc)
    {
        //TODO: restore check recipe
        CheckMerging(rc);
    }

    void CheckMerging(RecipeCombiator rc)
    {
        ItemScriptableObject product = null;
        rc.DebugPrint();

        if (rc.isGrind())
        {
            product = groundRecipeSO.Ground(itemSO);
        }
        else if (rc.isCook())
        {
            product = boilRecipeSO.Ground(itemSO);
        }
        else
        {
            foreach (ItemScriptableObject iso in craftRecipeSO.list)
            {
                if (iso == itemSO) { continue; } //skip if the recipe is itself
                if (iso.CheckMatch(rc.getRecipeGrid())) //find the scriptableobject with same recipe, if there is one
                {
                    product = iso;
                    break;
                }
            }
        }

        if (product != null) //if there is a recipe match, destroyself and emerge new game object and add special effect
        {
            rc.StartMerge(product);
        }
    }

    public IEnumerator MergeProgress(RecipeCombiator rc, ItemScriptableObject product)
    {
        foreach (Tetris t in rc.getPastTetris())
        {
            t.startMergeProcess();
        }
        float tCount = 0;
        float tRequire = 1;
        ProgressBar pb = Instantiate(mergeProgressBar, rc.CentralPosition(), Quaternion.identity).GetComponent<ProgressBar>();
        pb.transform.position += new Vector3(0, 0, -0.5f);


        while (tCount < tRequire)
        {
            tCount += Time.deltaTime;
            pb.setTo(tCount / tRequire);
            yield return new WaitForSeconds(0);
        }

        GameObject newTetris = Instantiate(product.myPrefab, rc.CentralPosition(), Quaternion.identity);
        CraftingManager.i.AddToAllTetris(newTetris);

        foreach (Tetris t in rc.getPastTetris())
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

    /// <summary>
    /// Recursive search a Tetris
    /// </summary>
    /// <param name="rc">The recipe combinator that is passed around to do the combination.</param>
    /// <param name="baseTetris">BaseTetris, for the input for RecipeCombinator.</param>
    /// <param name="baseCor">BaseCoordination, for the input for RecipeCombinator.</param>
    /// <param name="dir">Direction of attachment, for the input for RecipeCombinator.</param>
    /// <param name="newCor">Coordination of new Tetris, for the input for RecipeCombinator.</param>
    void Search(RecipeCombiator rc, Tetris baseTetris, Vector2 baseCor, Vector2 dir, Vector2 newCor)
    {
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
    }

    void CheckSnap(RecipeCombiator rc)//check if the Tetris is close enough to another to snap them together
    {
        if (!rc.hasConnector()) return;
        foreach (Edge e in allEdges)
        {
            if (e.isConnected())
            {
                AudioManager.i.PlaySoundEffectByName("Tetris Snap", true);
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
        CraftingManager.i.RemoveFromCrafting(gameObject);
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
}
