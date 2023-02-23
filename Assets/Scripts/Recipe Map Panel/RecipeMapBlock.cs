using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class RecipeMapBlock : MonoBehaviour
{
    enum State { Unknown , Locked , Unlocked };
    [SerializeField]
    private State state = State.Unknown;

    // Name
    public string name;

    //Cost
    [SerializeField ]
    public int baseCost = 0;

    // Components
    private SpriteRenderer spriteRenderer;
    private GameObject background;
    private TextMeshPro levelText;

    // RecipeLevel
    enum RecipeLevel { Name, Mats, Description, Graph };
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

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        background = transform.Find("Background").gameObject;

        levelText = transform.Find("Level").gameObject.GetComponent<TextMeshPro>();

        FindAdjacentBlocks();

        unlockedColor = RecipeMapManager.instance.unlockedColor;
        lockedColor = RecipeMapManager.instance.lockedColor;
        unknownColor = RecipeMapManager.instance.unknownColor;
        unlockedPathColor = RecipeMapManager.instance.unlockedPathColor;
        lockedPathColor = RecipeMapManager.instance.lockedPathColor;
        unknownPathColor = RecipeMapManager.instance.unknownPathColor;

        LevelTextUpdate();
        ColorUpdate();
    }

    void OnMouseOver()
    {
        // Level Up / Buy
        if (Input.GetMouseButtonDown(0))
        {
            RecipeMapManager.instance.DisplayRecipe(this);
        }

        // Unlock / testing / in the actual game, player unlock recipe by craft the item.
        if (Input.GetMouseButtonDown(1))
        {
            RecipeUnlock();
        }
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
            if (recipeLevel == RecipeLevel.Name)
            {
                recipeLevel = RecipeLevel.Mats;
            }
            else if (recipeLevel == RecipeLevel.Mats)
            {
                recipeLevel = RecipeLevel.Description;
            }
            else if (recipeLevel == RecipeLevel.Description)
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
    private void RecipeUnlock()
    {
        if (state == State.Locked)
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
            background.GetComponent<SpriteRenderer>().color = unlockedPathColor;

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
            background.GetComponent<SpriteRenderer>().color = lockedPathColor;

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
            if (recipeLevel == RecipeLevel.Name)
            {
                levelText.text = "1";
            }
            else if (recipeLevel == RecipeLevel.Mats)
            {
                levelText.text = "2";
            }
            else if (recipeLevel == RecipeLevel.Description)
            {
                levelText.text = "3";
            }
            else if (recipeLevel == RecipeLevel.Graph)
            {
                levelText.text = "4";
            }
        }
        else
        {
            levelText.text = "";
        }
    }
}
