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
        Barracks myös tuottaa tietyn määrän aseita ja haarniskoita, joita voidaan myydä joko barracksille (jolloin nämä menevät sotilaille), muille hahmoille (esim. farmer, miner),
        tai muille asutuksille (global trade). Blacksmith- resurssia työstää 'blacksmith' ammatin omaava Npc.
        */
        craftableItems = new List<Item>() {
            (Item)GD.Load(Constants.CONSUMERITEM),
        };

        requiredActions = requiredActions * 2; 

        actions = Constants.CRAFTACTIONS;
        dialogue = new ResourceDialogue(this, Constants.WOODCRAFT_DESCRIPTION, actions);

        defaultTexture = Constants.WOODCRAFT_TEXTURE;
        defaultPortrait = Constants.WOODCRAFT_PORTRAIT;
        defaultName = Constants.WOODCRAFT_NAME;
        defaultDescription = Constants.WOODCRAFT_DESCRIPTION;
        
        exhaustedDescription = Constants.OVEN_DESCRIPTION;
        exhaustedTexture = Constants.TREETRUNK_TEXTURE;
        exhaustedName = "Tree Trunk";
        exhaustedPortrait = Constants.TREETRUNK_PORTRAIT;

        base._Ready();
    }
}