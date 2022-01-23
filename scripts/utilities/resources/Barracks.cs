using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public class Barracks : Refinery
{
    [Signal]
    public delegate void Replenished(int foodAmount, int commodityAmount, Character logisticsOfficer);

    private Supply supply;

    private List<Guardpost> guardPosts = new List<Guardpost>();
    public void AddGuardPost(Guardpost guardpost) {
        guardPosts.Add(guardpost);
    }

    private int logisticsPayment = 100;     // Amount of currency given to logistics officers when sent out to purchase goods for the barracks.
    private int storedFoodAmount = 3;       // Barracks always attempts to keep at least this number of food in store.
    private int storedCommodityAmount = 1;  // Barracks always attempts to keep at least this number of commodities in store.
    private float funding = 0.75f;          // The amount of funding the state is giving to this barracks. Affects the amount of subsidized supplies.

    private Timer tradeTimer = new Timer();
    private int tradeTime = 60;             // Time stayed trading in seconds.
    private bool trading = false;

    public override void _Ready()
    {
        workerProfession = Constants.LOGISTICSOFFICER_PROFESSION;
        craftableItems = new List<Item>() {
            (Item)GD.Load(Constants.LOGISTICSITEM),
            (Item)GD.Load(Constants.RATIONITEM),
            (Item)GD.Load(Constants.RATIONEDGOODSITEM),
        };

        supply = supply == null ? new Supply() : supply;

        maxWorkers = 1;

        requiredActions = (int)Math.Round(requiredActions * 0.5); // The logistics officer only repurposes existing items for military use, which doesn't take much effort.
        actions = Constants.DEF_WORKACTIONS;
        dialogue = new ResourceDialogue(this, Constants.BARRACKS_DESCRIPTION, actions);

        defaultTexture = Constants.WOODCRAFT_TEXTURE;
        defaultPortrait = Constants.WOODCRAFT_PORTRAIT;
        defaultName = Constants.WOODCRAFT_NAME;
        defaultDescription = Constants.BARRACKS_DESCRIPTION;

        exhaustedDescription = Constants.OVEN_DESCRIPTION;
        exhaustedTexture = Constants.TREETRUNK_TEXTURE;
        exhaustedName = "Tree Trunk";
        exhaustedPortrait = Constants.TREETRUNK_PORTRAIT;

        createDebugInstance();  // Comment out for production.

        tradeTimer.OneShot = true;   // Instancing the tradeTimer
        tradeTimer.WaitTime = tradeTime;
        tradeTimer.Connect("timeout", this, nameof(TradingOff));
        AddChild(tradeTimer);

        base._Ready();
    }

    public void Initialize() {
        guardPosts?.ForEach(x => {
            Connect(nameof(Replenished), x, nameof(x.SupplySoldiers));
            x.Connect("SupplyUsed", this, nameof(ReduceSupply));
        });
        //GD.Print(string.Format("Barracks GuardPosts: {0}", guardPosts));
    }

    /*
    Logistics Officer gets assigned in the extended AddWorker function. The barracks adds items on its
    neededItems list depending on the amount of stored goods at the moment. For example, the barracks tries
    to always keep at least four food items and two consumer goods stored in its inventory. 
    */

    /*
    Supply represents the materials that are consumed while maintaining an armed force. Anything from repairing
    chipped blades and bent armor, to providing training materials, as well as decent living quarters. If supplies
    aren't met, the combat efficiency of soldiers degrades depending on how large the deficiency becomes. 
    */

    /*
    The logistics officer will create special 'ration' food items for the soldiers. These are free for the soldiers to use.
    Ration's nutrition value depends on what food item it was made from.
    */

    public override bool AddWorker(Character worker) {
        if (worker is Npc && ((Npc)worker).hasTraded && !setWorkItemQueue(worker))
        {
            SetToTrade(worker);
        }
        workers.Add(worker);
        return true;
    }
    public override void RemoveWorker(Character worker) {
        if (trading) {
            worker.RemoveFromGroup(Constants.LOGISTICS_GROUP);
            worker.tradeInventory = worker.inventory;
            worker.EmitSignal("Refresh", worker);
        }
        trading = false;
        base.RemoveWorker(worker);
    }

    private void SetToTrade(Character worker) {
        /*
        If the Logistics Officer hasn't got any better things to do, it will start to trade,
        providing soldiers with free munitions and goods.
        */
        worker.AddToGroup(Constants.LOGISTICS_GROUP);
        worker.tradeInventory = inventory;
        worker.EmitSignal("Refresh", worker);
        trading = true;

        int edibleItems = inventory.GetEdibleItemCount();
        int commodityItems = inventory.GetCommodityItemCount();
        if (edibleItems > 0 || commodityItems > 0) {    // This could be done so that soldiers get called to resupply based on the number of supplies/goods available (Not all soldiers should be called at the same time).
            EmitSignal(nameof(Replenished), edibleItems, commodityItems, worker);
        }
        tradeTimer.Start();
    }

    private void TradingOff() {
        if (workers.Any()) {
            Character worker = workers.First();
            worker.RemoveFromGroup(Constants.LOGISTICS_GROUP);
            worker.tradeInventory = worker.inventory;
            worker.EmitSignal("Refresh", worker);
        }
        trading = false;
    }

    public override void workAction(Character worker)
    {
        if (!trading) {
            if (!workItemQueue.Any())
            {
                if (!setWorkItemQueue(worker))
                {
                    //GD.Print(String.Format("Nothing to work on refinery {0}", this.Name));
                    worker.SetInteractive(); // Leave work state if no longer resources to continue crafting.
                    return;
                };
                //GD.Print(String.Format("Refinery {0} now working on item {1}", this.Name, workItemQueue[0].itemName));
            }
            currentActions++;
            //GD.Print(worker.Name, currentActions);
            if (currentActions >= requiredActions)
            {
                GiveResource(worker);
            }
        }
    }

    public override bool setWorkItemQueue(Character worker)
    {
        foreach (Item i in craftableItems)
        {
            /*
            Food- and commodity items require separate handling.
            */

            if (i is ConsumableItem) {                                                          
                ConsumableItem item = (ConsumableItem)i;
                if (item.nutritionValue > 0) {
                    if (inventory.HasItem(item, storedFoodAmount)) { // If inventory has stored amount of food/commodities, don't add it to workqueue and clear food/commodities from logistics officer's neededItems.
                        ((Npc)worker).ClearFoodFromBuyQueue();
                        continue;
                    }
                    ConsumableItem originItem = worker.inventory.GetEdibleItems()?.Last();  
                    if (originItem != null) {
                        item.nutritionValue = originItem.nutritionValue;
                        item.healValue = originItem.healValue;
                        addItemToWorkQueue(i, worker);
                        return true;
                    }
                }

                else if (item.commodityValue > 0) {
                    if (inventory.HasItem(item, storedCommodityAmount)) {
                        ((Npc)worker).ClearCommoditiesFromBuyQueue();
                        continue;
                    }
                    ConsumableItem originItem = worker.inventory.GetCommodityItems()?.Last();
                    if (originItem != null) {
                        item.commodityValue = originItem.commodityValue;
                        addItemToWorkQueue(i, worker);
                        return true;
                    }
                }
            }
            else if (worker.inventory.HasItems(i.recipe.GetRecipeDictionary()))
            {
                //GD.Print(String.Format("Craftable item: {0}\n{1}", i.itemName, i.recipe.GetRecipe()));
                addItemToWorkQueue(i, worker);
                return true;
            }
            if (i is LogisticsItem && supply.supply > (100 * funding)) {   // If there's enough supplies, don't buy supplies to make them.
                continue;
            }
            worker.checkBuyQueue(i.recipe.GetRecipeDictionary());
        }

        if (worker.inventory.currency < logisticsPayment) {     // Transfer funds from inventory to the logistics officer to purchase necessary goods.
            Trade tradeInstance = new Trade(inventory, worker.inventory);
            tradeInstance.TransferCurrency(logisticsPayment);
        }
        return false;
    }

    public override void GiveResource(Character worker)
    /*
    Barracks doesn't create items. It just consumes supply to provide logistics for soldiers.
    Logistics Officers provide barracks with food and goods that the soldiers will use.
    Soldiers are notified if the stocks of the barracks are updated with something they need.
    */
    {
        Item workItem = workItemQueue[0];
        //GD.Print("Currently given item:", workItem.itemName);
        if (inventory.HasItems(workItem?.recipe.GetRecipeDictionary()))
        {
            foreach (KeyValuePair<Item, int> kvp in workItem.recipe.GetRecipeDictionary())
            {
                inventory.RemoveItem(kvp.Key, kvp.Value);
            }
            inventory.AddItem(workItem);
            workItemQueue.RemoveAt(0);
            ItemAdded(workItem);
        }
        else
        {
            GD.Print(this.Name, ": Refinery : GiveResource: Inventory lacks required raw materials.");
        }
        currentActions = 0;
        workAction(worker);
    }

    private void ItemAdded(Item item)
    {
        if (item is LogisticsItem && supply.supply <= 100 * funding)
        {
            RefillSupply((LogisticsItem)item);
        }
    }

    private void ReduceSupply() {
        supply.DecreaseSupply();
    }
    private void RefillSupply(LogisticsItem supplyItem)
    {
        supply.AddSupply(supplyItem.supplyValue);
        inventory.RemoveItem(supplyItem);
    }

    // Notify soldiers of attacker
    public void AlertSoldiers(Character target) {
        GetSoldiers()?.ForEach(x => x.AttackTarget(target));
    }
    private List<Character> GetSoldiers() {
        List<Character> soldiers = new List<Character>();
        guardPosts?.ForEach(x => soldiers.AddRange(x.GetWorkers()));
        return soldiers;
    }

    // Debugging
    private float GetSupply() {
        return supply.supply;
    }
    private string GetGuardPostsString() {
        return string.Join(", " ,guardPosts.Select(x => x.entityName));
    }
    private string GetSoldiersString() {
        return string.Join(", " ,GetSoldiers().Select(x => x.entityName));
    }

    private void createDebugInstance() // Keep track of supply etc. during development.
	{
		PackedScene packedDebug = (PackedScene)ResourceLoader.Load("res://assets/debug/DebugInstance.tscn");
		DebugInstance debugInstance = (DebugInstance)packedDebug.Instance();
		AddChild(debugInstance);
		debugInstance.AddStat("Soldiers", this, "GetSoldiersString", true);
		debugInstance.AddStat("Supply", this, "GetSupply", true);
        debugInstance.AddStat("Guardposts", this, "GetGuardPostsString", true);
		debugInstance.AddStat("Logistics Payment", this, "logisticsPayment", false);
	}
}