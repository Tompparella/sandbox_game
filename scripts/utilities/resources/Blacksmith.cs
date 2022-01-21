using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public class Blacksmith : Refinery
{
	public override void _Ready()
	{
		/*
		Blacksmith creates supplies, armour and weapons, that the 'Barracks' requires.
		The demand for supplies relates to the amount of soldiers, and if these needs are not met, the soldiers will start to suffer debuffs (rusting, bad morale, etc).
		Blacksmith also creates a certain amount of weapons and armor, that can be sold to barracks, the open market, or to global trade.
		*/
		workerProfession = Constants.BLACKSMITH_PROFESSION;
		craftableItems = new List<Item>() {
			(Item)GD.Load(Constants.SUPPLIESITEM),
		};

		requiredActions = requiredActions * 2;

		actions = Constants.SMITHACTIONS;
		dialogue = new ResourceDialogue(this, Constants.BLACKSMITH_DESCRIPTION, actions);

		defaultTexture = Constants.WOODCRAFT_TEXTURE;
		defaultPortrait = Constants.WOODCRAFT_PORTRAIT;
		defaultName = Constants.BLACKSMITH_NAME;
		defaultDescription = Constants.BLACKSMITH_DESCRIPTION;

		exhaustedDescription = Constants.OVEN_DESCRIPTION;
		exhaustedTexture = Constants.TREETRUNK_TEXTURE;
		exhaustedName = "Tree Trunk";
		exhaustedPortrait = Constants.TREETRUNK_PORTRAIT;

		base._Ready();
	}
}
