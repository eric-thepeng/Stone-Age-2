using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecipeMapBlock : MonoBehaviour
{
    enum State { Unknown , Locked , Unlocked };
    [SerializeField]
    private State state = State.Unknown;

    // Name
    public new string name;

    // Components
    private SpriteRenderer spriteRenderer;
    private GameObject background;

    // RecipeLevel
    enum RecipeLevel { Name, Mats, Description, Graph };
    [SerializeField] // For testing
    private RecipeLevel recipeLevel = RecipeLevel.Name;

    // Path
    public RecipeMapBlock[] adjacentBlocks = new RecipeMapBlock[4];

    public SpriteRenderer[] blockLines = new SpriteRenderer[4];

    // Color
    public Color32 unlockedColor;
    public Color32 lockedColor;
    public Color32 unknownColor;
    public Color32 unlockedPathColor;
    public Color32 lockedPathColor;
    public Color32 unknownPathColor;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        background = transform.Find("Background").gameObject;

        ColorUpdate();
    }

    void OnMouseOver()
    {
        // Level Up / Buy
        if (Input.GetMouseButtonDown(0))
        {
            if (state == State.Locked)
            {
                RecipeLevelUp();
            }
        }

        // Unlock / testing / in the actual game, player unlock recipe by craft the item.
        if (Input.GetMouseButtonDown(1))
        {
            RecipeUnlock();
        }
    }

    private void RecipeLevelUp()
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

        ColorUpdate();
    }

    // Discover a recipe, change it from unknown to locked
    private void RecipeDiscover()
    {
        if (state == State.Unknown)
        {
            state = State.Locked;
            ColorUpdate();
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

        for (int count = 0; count < 4; count++)
        {
            if (adjacentBlocks[count] == null)
            {
                blockLines[count].color = unknownPathColor;
            }
        }
    }
}
