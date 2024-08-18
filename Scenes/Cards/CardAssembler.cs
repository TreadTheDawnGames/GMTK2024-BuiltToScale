using Godot;
using Medallion;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection.Emit;

public static class CardAssembler 
{

    public enum CardType { beachball, bowl, crate, lamp, mattress, obsidian, scaffleting, ship, shop, staircase, steelcrate, stoneball, table, toilet,trafficCone, tree,truck}
    public enum GuaranteedCardType {trafficCone, crate, scaffleting }
    public enum StarterCardType { staircase, beachball, crate}

	public static CardData Rand()
	{
        return new CardData(RandStringValue());

        
	}

    public static CardType RandType()
    {
        int rand = (int)(GD.Randi() % 17);
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
        return GetStarterObjectPath((StarterCardType)rand);
     }

    public static CardData Create(CardType type)
    {
        return new CardData(GetObjectPath(type));
    }
    public static CardData Create(StarterCardType type)
    {
        return new CardData(GetStarterObjectPath(type));
    }
    public static CardData Create(GuaranteedCardType type)
    {
        return new CardData(GetGuaranteedObjectPath(type));
    }

    static string GetStarterObjectPath(StarterCardType type)
    {
        string originPath = "res://Scenes/PhysicsCardObjects/";
        originPath += type.ToString();
        return originPath + ".tscn";
    }
    static string GetGuaranteedObjectPath(GuaranteedCardType type)
    {
        string originPath = "res://Scenes/PhysicsCardObjects/";
        originPath += type.ToString();
        return originPath + ".tscn";
    }
    static string GetObjectPath(CardType type)
    {
        string originPath = "res://Scenes/PhysicsCardObjects/";
        originPath += type.ToString();

        return originPath + ".tscn";
    }

    public static List<CardData> TestDeck()
    {
        List<CardData> starterDeck = new List<CardData>
        {
            new CardData(GetObjectPath(CardType.obsidian)),
            new CardData(GetObjectPath(CardType.shop))
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
            new CardData(GetObjectPath(CardType.obsidian)),
            new CardData(GetObjectPath(CardType.shop))
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


    public static List <CardData> OneEach() 
    {
        List<CardData> oneEach = new List<CardData>();

        foreach (CardType type in Enum.GetValues(typeof(CardType)))
        {
            oneEach.Add(new CardData(GetObjectPath(type)));
        }

        return oneEach;
    }

    public static CardType GetWeightedRand()
    {
        int rand = (int)(GD.Randi() % 101);

        //50% chance
        if (rand >= 0 && rand < 50)
        {
            CardType[] cardTypes = new CardType[]
            {
                CardType.beachball,
                CardType.bowl,
                CardType.crate,
                CardType.lamp,
                CardType.scaffleting,
                CardType.tree
            };
            int innerRand = (int)(GD.Randi() % cardTypes.Length);

            return cardTypes[innerRand];
        }
        //25% chance
        else if (rand >= 50 && rand <= 75)
        {
            CardType[] cardTypes = new CardType[]
            {
                CardType.toilet,
                CardType.trafficCone,
                CardType.staircase,
                CardType.table
            };
            int innerRand = (int)(GD.Randi() % cardTypes.Length);

            return cardTypes[innerRand];
        }
        //15% chance
        else if (rand >= 75 && rand <= 90)
        {
            CardType[] cardTypes = new CardType[]
            {
                CardType.steelcrate,
                CardType.stoneball,
                CardType.mattress
            };
            int innerRand = (int)(GD.Randi() % cardTypes.Length);
            return cardTypes[innerRand];
        }
        //9% chance
        else if (rand >= 91 && rand <= 99)
        {
            CardType[] cardTypes = new CardType[]
            {
                CardType.truck,
                CardType.shop,
                CardType.obsidian
            };
            int innerRand = (int)(GD.Randi() % cardTypes.Length);


            return cardTypes[innerRand];
        }
        //1% chance;
        else if (rand == 100)
        {
            CardType[] cardTypes = new CardType[]
             {
                CardType.ship,
             };
            int innerRand = (int)(GD.Randi() % cardTypes.Length);


            return cardTypes[innerRand];
        }



        return CardType.crate;

    }
}
