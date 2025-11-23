using System.Collections.Generic;
using UnityEngine;

public class CraftingRecipe : MonoBehaviour
{
    public string recipeName;
    public string recipeId;
    public List<CraftableItem> ItemIngredients = new List<CraftableItem>();
    public CraftableItem ResultItem;
    public int ResultQuantity;
    public float CraftingTime; // Time in seconds to craft the item
    public bool IsUnlocked; // Whether the recipe is unlocked for the player

}
