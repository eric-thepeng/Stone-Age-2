using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//The class that is passed on during recursive search to combine all the Tetris together and form a recipe
public class RecipeCombinator
{
    List<Tetris> pastTetris; //The list of Tetris that is already processed
    List<KeyValuePair<Vector2, ItemScriptableObject>> recipeGrid; //The final formed recipe in grid form
    ObjectStackList<ItemScriptableObject> craftingStationCounter;//crafting station in contact
    Dictionary<Tetris, Vector2> tetrisRecipePositionDelta;
    Tetris origionTetris;

    ItemScriptableObject mergeISO;
    GameObject mergeWindow;

    public delegate void OnNewItemCrafted(ItemScriptableObject iso);
    public static event OnNewItemCrafted onNewItemCrafted;
    

    public RecipeCombinator(Tetris oT)
    {
        origionTetris = oT;
        pastTetris = new List<Tetris>();
        recipeGrid = new List<KeyValuePair<Vector2, ItemScriptableObject>>();
        craftingStationCounter = new ObjectStackList<ItemScriptableObject>();
        tetrisRecipePositionDelta = new Dictionary<Tetris, Vector2>();
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

        if (newT.itemSO.isCraftingStation)
        {
            craftingStationCounter.AddAmount(newT.itemSO);
            return;
        }

        pastTetris.Add(newT);
        newT.DestroyRC();
        newT.myRC = this;

        //Is it the first Tetris to be added (the Tetris that create this RecipeCombinator)
        if (baseT == newT)
        {
            //newT.recipeFormingDelta = new Vector2(0, 0);
            tetrisRecipePositionDelta.Add(newT, new Vector2(0, 0));
            List<KeyValuePair<Vector2, ItemScriptableObject>> toSearch = newT.itemSO.FormationRecipeCoord;
            foreach (KeyValuePair<Vector2, ItemScriptableObject> kvp in toSearch)
            {
                recipeGrid.Add(kvp);
            }
            return;
        }
        //It is not the first Tetris to be added.
        else
        {
            //Calculate and record Delta
            Vector2 delta = baseCor + dir - newCor + tetrisRecipePositionDelta[baseT]; //baseT.recipeFormingDelta;
            tetrisRecipePositionDelta.Add(newT, delta);
            //newT.recipeFormingDelta = delta;

            //Load into final receipe 
            foreach (KeyValuePair<Vector2, ItemScriptableObject> kvp in newT.itemSO.FormationRecipeCoord)
            {
                recipeGrid.Add(new KeyValuePair<Vector2, ItemScriptableObject>(kvp.Key + delta, kvp.Value));
            }
        }
    }

    public void CheckMerging()
    {
        DebugPrint();
        mergeISO = null;

        foreach (ItemCraftScriptableObject icso in origionTetris.recipeListSO.list)
        {
            //print("Check icso");
            //if (!(icso.CraftingStationRequired != null && !CraftingManager.i.TetrisInPresent(icso.CraftingStationRequired)) && icso.CheckMatch(getRecipeGrid())) //find the scriptableobject with same recipe, if there is one
            //{
            if (icso.CheckMatch(getRecipeGrid(), craftingStationCounter))
            {
                //print("FOUND IT!!");
                mergeISO = icso.ItemCrafted;
                break;
            }
        }

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
            if (t.myRC == null && t != DisassembleFrom)
            {
                t.RefreshEdges(DisassembleFrom);
                t.myRC = new RecipeCombinator(t);
                t.Search(t.myRC, t, new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), DisassembleFrom);
                t.myRC.Organize();
                t.myRC.CheckMerging();
            }
        }
        DisassembleFrom.myRC.pastTetris.Clear();
        DisassembleFrom.myRC = null;
    }

    /// <summary>
    /// </summary>
    /// <param name="t">The Tetris to be checked.</param>
    /// <returns>True if the Tetris is checked before.</returns>
    public bool Searched(Tetris t)
    {
        return pastTetris.Contains(t);
    }

    /// <summary>
    /// </summary>
    /// <returns>The abstract central position of the Tetris's gameobject.</returns>
    public Vector3 CentralPosition()
    {

        Vector3 topLeft = new Vector3(0, 0, 0);
        Vector3 botRight = new Vector3(0, 0, 0);

        bool first = true;

        foreach (Tetris t in pastTetris)
        {
            Vector3 tPosition = t.transform.position;
            if (first)
            {
                topLeft = tPosition;
                botRight = tPosition;
                first = false;
            }

            Vector3 thisBotRightDelta = t.itemSO.GetBottomRightDelta();
            Vector3 deltaWeight = thisBotRightDelta * t.gameObject.transform.localScale.x; //get bottom-right delta with scale
            deltaWeight = new Vector3(deltaWeight.x, deltaWeight.y / Mathf.Sqrt(2f), deltaWeight.y / Mathf.Sqrt(2f)); //readjust it to incline level
            
            //get the top-left and bottom-right position
            Vector3 thisBotRight = tPosition + deltaWeight;
            Vector3 thisTopLeft = tPosition - deltaWeight;
            
            //compare and get the top-left and bottom-right boundary of the recipe
            if (thisBotRight.x > botRight.x) botRight.x = thisBotRight.x;
            if (thisBotRight.y < botRight.y) { botRight.y = thisBotRight.y; botRight.z = thisBotRight.z; }
            if (thisTopLeft.x < topLeft.x) topLeft.x = thisTopLeft.x;
            if (thisTopLeft.y > topLeft.y) { topLeft.y = thisTopLeft.y; topLeft.z = thisTopLeft.z; }
        }

        return (topLeft + botRight) / 2;
    }

    /// <summary>
    /// Organize the formed recipe, move all the nodes so that there are no negative coordinations.
    /// </summary>
    public void Organize()
    {
        Vector2 leftNTopBound = new Vector2(0, 0);
        foreach (KeyValuePair<Vector2, ItemScriptableObject> kvp in recipeGrid)
        {
            if (kvp.Key.x < leftNTopBound.x) leftNTopBound.x = kvp.Key.x;
            if (kvp.Key.y < leftNTopBound.y) leftNTopBound.y = kvp.Key.y;
        }
        List<KeyValuePair<Vector2, ItemScriptableObject>> editedRecipeGrid = new List<KeyValuePair<Vector2, ItemScriptableObject>>();
        foreach (KeyValuePair<Vector2, ItemScriptableObject> kvp in recipeGrid)
        {
            editedRecipeGrid.Add(new KeyValuePair<Vector2, ItemScriptableObject>(kvp.Key - leftNTopBound, kvp.Value));
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
        Debug.Log("Debug Print Recipe Combinator");
        foreach (KeyValuePair<Vector2, ItemScriptableObject> kvp in recipeGrid)
        {
            Debug.Log(kvp.Key + " " + kvp.Value.name);
        }
        Debug.Log(craftingStationCounter);
        Debug.Log("End Debug Print Recipe Combinator");
    }

    public void MergeOne()
    {
        origionTetris.StartCoroutine(MergeProcess());
        DestroyCraftPreview();
        RecipeMapManager.i.CheckUnlock(GetMergeISO());
        if (onNewItemCrafted != null) onNewItemCrafted(GetMergeISO());
    }

    public void MergeAuto()
    {
        bool mergeFromInventory = true;
        if (onNewItemCrafted != null) onNewItemCrafted(GetMergeISO());
        foreach (KeyValuePair<ItemScriptableObject, int> kvp in GetIngredients())
        {
            if (Inventory.i.ItemInStockAmount(kvp.Key) < kvp.Value) mergeFromInventory = false;
        }
        if (mergeFromInventory) //merge from inventory
        {
            foreach (KeyValuePair<ItemScriptableObject, int> kvp in GetIngredients())
            {
                for (int i = kvp.Value; i > 0; i--)
                {
                    Inventory.i.UseItemFromStock(kvp.Key);
                }
            }
        }
        else //merge the inspected one
        {
            DeleteTetrisAndMergeWindow();
            /*
            origionTetris.StartCoroutine(MergeProcess());
            DestroyCraftPreview();*/
        }

        CraftingManager.i.TetrisFlyToInventoryEffect(GetMergeISO(), CentralPosition(), 0.4f, true);

        //2023 02 27 Recipe System to check if there is a unlock // Added by Will
        RecipeMapManager.i.CheckUnlock(GetMergeISO());

    }

    IEnumerator MergeProcess()
    {
        if(GetMergeISO() == null)
        {
            Debug.LogError("Trying to merge empty Recipe Combinator");
        }

        foreach(Tetris t in GetPastTetris())
        {
            t.startMergeProcess();
        }
        float tCount = 0;
        float tRequire = 1;
        ProgressBar pb = MonoBehaviour.Instantiate(CraftingManager.i.mergeProgressBar, CentralPosition(), Quaternion.identity).GetComponent<ProgressBar>();
        pb.transform.position += new Vector3(0, 0, -0.5f);

        while (tCount < tRequire)
        {
            tCount += Time.deltaTime * 5;
            pb.setTo(tCount / tRequire);
            yield return new WaitForSeconds(0);
        }

        GameObject newTetrisGO = CraftingManager.i.CreateTetris(GetMergeISO(), CentralPosition(), CraftingManager.CreateFrom.MERGE);

        //2023 02 27 Recipe System to check if there is a unlock // Added by Will
        RecipeMapManager.i.CheckUnlock(GetMergeISO());

        foreach (Tetris t in GetPastTetris())
        {
            t.DestroySelf();
        }

        if (GetMergeISO().isCraftingStation)
        {
            yield return new WaitForSeconds(0.2f);
            newTetrisGO.transform.DOMove(CraftingManager.i.testtesttest.position + new Vector3(0, 0.3f, 0.3f), 1);
        }

    }

    public void DeleteTetrisAndMergeWindow()
    {
        Debug.Log("delete merge window");
        foreach(Tetris t in pastTetris)
        {
             t.DestroySelf();
        }
        DestroyCraftPreview();
    }

    public void PutBackTetrisAndMergeWindow()
    {
        Debug.Log("delete merge window");
        foreach (Tetris t in pastTetris)
        {
            CraftingManager.i.PutBackTetrisToInventory(t.gameObject);
        }
        DestroyCraftPreview();
    }

    public void DestroyCraftPreview()
    {
        if (mergeWindow != null)
        {
            MonoBehaviour.Destroy(mergeWindow);
            mergeWindow = null;
        }
    }

    public bool IsOrigionTetris(Tetris thisTetris)
    {
        return thisTetris == origionTetris;
    }

    /// <summary>
    /// Return the grid representation (coordination + so) of recipe.
    /// </summary>
    public List<KeyValuePair<Vector2, ItemScriptableObject>> getRecipeGrid() { return recipeGrid; }

    /// <summary>
    /// Count and return the amount of each iso in the RC
    /// </summary>
    public Dictionary<ItemScriptableObject, int> GetIngredients()
    {
        Dictionary<ItemScriptableObject, int> result = new Dictionary<ItemScriptableObject, int>();
        foreach (KeyValuePair<Vector2, ItemScriptableObject> kvp in getRecipeGrid())
        {
            ItemScriptableObject afterCast = (ItemScriptableObject)kvp.Value;
            if (result.ContainsKey(afterCast))
            {
                result[afterCast] += 1;
            }
            else
            {
                result.Add(afterCast, 1);
            }
        }
        return result;
    }

    /// <summary>
    /// Get all the Tetris that has been processed before.
    /// </summary>
    public List<Tetris> GetPastTetris() { return pastTetris; }

    public ItemScriptableObject GetMergeISO() { return mergeISO; }
}


