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
    public enum StarterCardType { staircase, crate, bowl, lamp}

	public static CardData Rand()
	{
        return new CardData(RandStringValue<CardType>());

        
	}

    public static T RandType<T>() where T : Enum
    {
        if (!typeof(T).IsEnum)
        {
            throw new ArgumentException("T must be an enumerated type");
        }


        var values = Enum.GetValues(typeof(T));
        int counter = values.Length;
        
        int rand = (int)(GD.Randi() % counter);

        return (T)Enum.Parse(typeof(T), values.GetValue(rand).ToString());

    }

    static string RandStringValue<T>() where T : Enum
    {
        return MakeCardPath(RandType<T>());
    }

    static string GetRandStarter()
    {
        int rand = (int)(GD.Randi() % 3);
        return GetStarterObjectPath((StarterCardType)rand);
     }

    public static CardData Create(CardType type)
    {
        return new CardData(MakeCardPath(type));
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
    public static string MakeCardPath(Enum type)
    {


        string originPath = "res://Scenes/PhysicsCardObjects/";
        originPath += type.ToString();

        return originPath + ".tscn";
    }

    public static List<CardData> ShopTestDeck(int deckSize)
    {
        List<CardData> starterDeck = new List<CardData>
        {
            new CardData(MakeCardPath(CardType.obsidian)),
            new CardData(MakeCardPath(CardType.shop)),
            new CardData(MakeCardPath(CardType.shop)),
            new CardData(MakeCardPath(CardType.shop)),
            new CardData(MakeCardPath(CardType.shop)),
            new CardData(MakeCardPath(CardType.shop)),
            new CardData(MakeCardPath(CardType.shop))
        };
        while (starterDeck.Count < deckSize)
        {
            starterDeck.Add(Rand());
        }
        return starterDeck;
    }



    public static List<CardData> FullRandDeck(int deckSize)
    {
        List<CardData> starterDeck = new List<CardData>
        {
            new CardData(MakeCardPath(CardType.obsidian)),
            new CardData(MakeCardPath(CardType.shop))
        };

        while (starterDeck.Count < deckSize)
        {
            starterDeck.Add(Rand());
        }

        foreach (CardData card in starterDeck)
        {
            card.inShop = false;
        }
        

        return starterDeck;
    }

    public static List<CardData> BeachBallStarter(int deckSize)
    {
        List<CardData> starterDeck = new List<CardData>
        {
            new CardData(MakeCardPath(CardType.obsidian)),
            new CardData(MakeCardPath(CardType.shop))
        };

        while (starterDeck.Count < deckSize/2)
        {
            starterDeck.Add(new CardData(MakeCardPath(RandType<StarterCardType>())));
            
        }
        while (starterDeck.Count < deckSize)
        {
            starterDeck.Add(new CardData(MakeCardPath(CardType.beachball)));
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
            new CardData(MakeCardPath(CardType.obsidian)),
            new CardData(MakeCardPath(CardType.shop))
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
            oneEach.Add(new CardData(MakeCardPath(type)));
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
