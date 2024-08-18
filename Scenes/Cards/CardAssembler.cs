using Godot;
using Medallion;
using System;
using System.Collections.Generic;

public static class CardAssembler 
{

    public enum CardType { Crate,Toilet, TrafficCone, Scaffleting, Stoneball, Obsidian, Shop}

	public static CardData Rand()
	{
        return new CardData(RandStringValue());
	}

    public static CardType RandType()
    {
        int rand = (int)(GD.Randi() % 7);
        return (CardType)rand;

    }

    static string RandStringValue()
    {
        int rand = (int)(GD.Randi() % 7);
        return GetObjectPath((CardType)rand);
     }

    static string GetRandStarter()
    {
        int rand = (int)(GD.Randi() % 3);
        return GetObjectPath((CardType)rand);
     }

    public static CardData Create(CardType type)
    {
        return new CardData(GetObjectPath(type));
    }

    static string GetObjectPath(CardType type)
    {
        string originPath = "res://Scenes/PhysicsCardObjects/";
        switch (type)
        {
            case CardType.Crate:
                originPath += "crate";
                break;
            case CardType.Toilet:
                originPath += "toilet";
                break;
            case CardType.TrafficCone:
                originPath += "trafficCone";
                break;

            
            case CardType.Scaffleting:
                originPath += "scaffleting";
                break;
            case CardType.Stoneball:
                originPath += "stoneball";
                break;
            case CardType.Obsidian:
                originPath += "obsidian";
                break;
            case CardType.Shop:
                originPath += "shop";
                break;
            

            default:
                originPath += "crate.tscn";
                GD.Print("GOT BAD NUMBER");
                break;

        }

        return originPath + ".tscn";
    }

    public static List<CardData> TestDeck()
    {
        List<CardData> starterDeck = new List<CardData>
        {
            new CardData(GetObjectPath(CardType.Obsidian)),
            new CardData(GetObjectPath(CardType.Shop))
        };

        while (starterDeck.Count < 4)
        {
            starterDeck.Add(new CardData(GetRandStarter()));
        }

        foreach (CardData card in starterDeck)
        {
            card.inShop = false;
        }


        return starterDeck;
    }

    public static List<CardData> Starter()
    {
        List<CardData> starterDeck = new List<CardData>
        {
            new CardData(GetObjectPath(CardType.Obsidian)),
            new CardData(GetObjectPath(CardType.Shop))
        };

        while (starterDeck.Count < 10)
        {
            starterDeck.Add(new CardData(GetRandStarter()));
        }

        foreach (CardData card in starterDeck)
        {
            card.inShop = false;
        }


        return starterDeck;
    }
}
