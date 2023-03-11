using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class RecipeMapBlock : MonoBehaviour
{
    enum State { Unknown , Locked , Unlocked };
    [SerializeField]
    private State state = State.Unknown;

    // ISO
    public ItemScriptableObject myISO;

    // Name
    public string name;

    public string material;
    public string craftDescription;

    //Cost
    [SerializeField ]
    public int baseCost = 0;

    // Components
    private SpriteRenderer spriteRenderer;
    private GameObject border;
    private GameObject background;
    private TextMeshPro levelText;

    // RecipeLevel
    enum RecipeLevel { Name, Materials, Description, Graph };
    [SerializeField] // For testing
    private RecipeLevel recipeLevel = RecipeLevel.Name;

    // Path
    private RecipeMapBlock[] adjacentBlocks = new RecipeMapBlock[4];

    public SpriteRenderer[] blockLines = new SpriteRenderer[4];

    // Color
    private Color32 unlockedColor;
    private Color32 lockedColor;
    private Color32 unknownColor;
    private Color32 unlockedPathColor;
    private Color32 lockedPathColor;
    private Color32 unknownPathColor;
    private Color32 unlockedBackgroundColor;
    private Color32 lockedBackgroundColor;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        border = transform.Find("Border").gameObject;
        background = transform.Find("Background").gameObject;

        levelText = transform.Find("Level").gameObject.GetComponent<TextMeshPro>();

        FindAdjacentBlocks();

        unlockedColor = RecipeMapManager.i.unlockedColor;
        lockedColor = RecipeMapManager.i.lockedColor;
        unknownColor = RecipeMapManager.i.unknownColor;
        unlockedPathColor = RecipeMapManager.i.unlockedPathColor;
        lockedPathColor = RecipeMapManager.i.lockedPathColor;
        unknownPathColor = RecipeMapManager.i.unknownPathColor;
        unlockedBackgroundColor = RecipeMapManager.i.unlockedBackgroundColor;
        lockedBackgroundColor = RecipeMapManager.i.lockedBackgroundColor;

        LevelTextUpdate();
        ColorUpdate();
    }

    void OnMouseOver()
    {
        // Show Tooltip
        if (Input.GetMouseButtonDown(0) && state != State.Unknown)
        {
            RecipeMapManager.i.DisplayRecipe(this);
        }

        
        if (Input.GetMouseButtonDown(1))
        {
            //RecipeUnlock(); // For debug, quick unlock
        }
    }

    public int CurrentCost() {
        return GetLevelInt() * baseCost;
    }

    private void FindAdjacentBlocks() {
        RaycastHit hit;


        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.up), out hit, 1.5f))
        {
            adjacentBlocks[0] = hit.collider.gameObject.GetComponent<RecipeMapBlock>();
        }
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.left), out hit, 1.5f))
        {
            adjacentBlocks[1] = hit.collider.gameObject.GetComponent<RecipeMapBlock>();
        }
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.down), out hit, 1.5f))
        {
            adjacentBlocks[2] = hit.collider.gameObject.GetComponent<RecipeMapBlock>();
        }
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.right), out hit, 1.5f))
        {
            adjacentBlocks[3] = hit.collider.gameObject.GetComponent<RecipeMapBlock>();
        }
    }

    public void RecipeUpgrade()
    {
        if (state == State.Locked)
        {
            if (GetLevelInt() == 1)
            {
                recipeLevel = RecipeLevel.Materials;
            }
            else if (GetLevelInt() == 2)
            {
                recipeLevel = RecipeLevel.Description;
            }
            else if (GetLevelInt() == 3)
            {
                recipeLevel = RecipeLevel.Graph;
            }
        }
     
        ColorUpdate();
        LevelTextUpdate();
    }

    // Discover a recipe, change it from unknown to locked
    private void RecipeDiscover()
    {
        if (state == State.Unknown)
        {
            state = State.Locked;
            ColorUpdate();
            LevelTextUpdate();
        }
    }

    // Unlock a recipe (after it got crafted the first time), change it from locked to unlocked
    public void RecipeUnlock()
    {
        if (GetStateString() == "Unknown" || GetStateString() == "Locked")
        {
            state = State.Unlocked;
            recipeLevel = RecipeLevel.Graph; // turn the level the highest

            for (int count = 0; count < 4; count ++) {
                if (adjacentBlocks[count] != null) {
                    adjacentBlocks[count].RecipeDiscover();
                }
            }

            ColorUpdate();
            LevelTextUpdate();
        }
    }

    private void ColorUpdate()
    {
        // Icon Color
        if (state == State.Unlocked)
        {
            spriteRenderer.color = unlockedColor;
        }
        else if (state == State.Locked)
        {
            spriteRenderer.color = lockedColor;
        }
        else
        {
            spriteRenderer.color = unknownColor;
        }

        // BG & Lines Color
        if (state == State.Unlocked)
        {
            border.GetComponent<SpriteRenderer>().color = unlockedPathColor;

            background.GetComponent<SpriteRenderer>().color = unlockedBackgroundColor;

            for (int count = 0; count < 4; count++)
            {
                if (adjacentBlocks[count] != null)
                {
                    blockLines[count].color = unlockedPathColor;
                }
            }
        }
        else if (state == State.Locked)
        {
            border.GetComponent<SpriteRenderer>().color = lockedPathColor;

            background.GetComponent<SpriteRenderer>().color = lockedBackgroundColor;

            for (int count = 0; count < 4; count++)
            {
                if (adjacentBlocks[count] != null)
                {
                    blockLines[count].color = lockedPathColor;
                }
            }
        }
        else
        {
            border.GetComponent<SpriteRenderer>().color = unknownPathColor;

            background.GetComponent<SpriteRenderer>().color = unknownPathColor;

            for (int count = 0; count < 4; count++)
            {
                if (adjacentBlocks[count] != null)
                {
                    blockLines[count].color = unknownPathColor;
                }
            }
        }

        // Set line color when the adjacent block is null
        for (int count = 0; count < 4; count++)
        {
            if (adjacentBlocks[count] == null)
            {
                blockLines[count].color = unknownPathColor;
            }
        }
    }

    private void LevelTextUpdate()
    {
        if (state == State.Locked)
        {
            levelText.text = GetLevelInt().ToString();
        }
        else
        {
            levelText.text = "";
        }
    }

    public string GetStateString() { return (state.ToString()); }

    public string GetLevelString() { return (recipeLevel.ToString()); }

    // Return recipe level in int
    public int GetLevelInt()
    {
        if (recipeLevel == RecipeLevel.Name)
        {
            return 1;
        }
        else if (recipeLevel == RecipeLevel.Materials)
        {
            return 2;
        }
        else if (recipeLevel == RecipeLevel.Description)
        {
            return 3;
        }
        else if (recipeLevel == RecipeLevel.Graph)
        {
            return 4;
        }

        return 0;
    }

    // Set recipe level based on the level int pass in.
    public void SetLevelInt(int level) {
        if (level == 1)
        {
            recipeLevel = RecipeLevel.Name;
        }
        else if (level == 2)
        {
            recipeLevel = RecipeLevel.Materials;
        }
        else if (level == 3)
        {
            recipeLevel = RecipeLevel.Description;
        }
        else if (level == 4)
        {
            recipeLevel = RecipeLevel.Graph;
        }
    }
}
