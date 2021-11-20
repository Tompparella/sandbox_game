using Godot;
using System.Collections.Generic;
using System.Linq;

public class Recipe : Resource
{
    [Export]
    public List<Item> recipeItems = new List<Item>();

    public string GetRecipe() {
        string returnString = "Recipe:\n";
        if (recipeItems.Any()) {
            Item[] processedItems = {};
            foreach (Item i in recipeItems) {
                if (!processedItems.Contains(i)) {
                    int itemCount = recipeItems.Where(x => x.Equals(i)).Count();
                    returnString += string.Format("{0}x {1}\n", itemCount, i.itemName);
                    processedItems.Append(i);
                }
            }
            return returnString;
        } else {
            return "Error in recipe. recipeItems count != itemCounts count.";
        }
    }

    public Dictionary<Item, int> GetRecipeDictionary() {
        Dictionary<Item, int> processedItems = new Dictionary<Item, int>(); // We turn the list of recipe items into a dictionary for easier use later.
        foreach (Item i in recipeItems) {
            if (!processedItems.Keys.Contains(i)) {
                int itemCount = recipeItems.Where(x => x.Equals(i)).Count();
                processedItems.Add(i,itemCount);
            }
        }
        return processedItems;
    }
}
