using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public class Blacksmith : Refinery
{
	public override void _Ready()
	{
		/*
		Tää toimii sillesti että blacksmith tuottaa aseita sun muuta jne., mutta ensisijaisesti tarpeeksi 'material supplies' resurssia, jota 'barracks' rakennus vaatii.
		Barracksin tarve supply-resurssille riippuu sotilaiden määrästä, ja jos tarvetta ei tyydytetä, sotilaiden aseet/haarniskat alkavat 'ruostua', ja nämä saavat debuffeja.
		Blacksmith myös tuottaa tietyn määrän aseita ja haarniskoita, joita voidaan myydä joko barracksille (jolloin nämä menevät sotilaille), muille hahmoille (esim. farmer, miner),
		tai muille asutuksille (global trade). Blacksmith- resurssia työstää 'blacksmith' ammatin omaava Npc.
		*/
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
