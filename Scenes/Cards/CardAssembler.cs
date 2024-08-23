using Godot;
using Medallion;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection.Emit;

public static class CardAssembler
{

    public enum CardType { beachball, bowl, crate, gascan, lamp, mattress, obsidian, piano, scaffleting, ship, shop, staircase, starterbowl, steelcrate, stoneball, table, toilet, trafficCone, tree, truck, zeekplushy}
    public enum GuaranteedCardType { obsidian /*crate*/, scaffleting, staircase }
    public enum StarterCardType { staircase, crate, toilet }
    public enum SpecialCardType { obsidian, shop, ship, zeekplushy }


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

    static string GetRandStarterCard()
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
            new CardData(MakeCardPath(CardType.shop)),
            new CardData(MakeCardPath(CardType.shop))
        };
        while (starterDeck.Count < deckSize)
        {
            starterDeck.Add(Rand());
        }
        return starterDeck;
    }

    public static CardType GetRandCardTypeOfCost(int cost)
    {
        List<CardType> cards = new List<CardType>();
        foreach (CardType type in Enum.GetValues(typeof(CardType)))
        {
            if (Create(type).cost == cost)
            {
                cards.Add(type);
            }
        }

        int rand;
        if (cards.Count > 0)
        {
            rand = (int)(GD.Randi() % cards.Count);
            return cards[rand];
        }
        else
        {
            GD.PrintErr("No card of cost " + cost + "exists!");
            return CardType.crate;
        }

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

    public static List<CardData> BalancedStarterDeck(int deckSize)
    {
        DeckManager.Instance.correctDeck = true;
        List<CardData> starterDeck = new List<CardData>
        {
            new CardData(MakeCardPath(CardType.obsidian)),
            new CardData(MakeCardPath(CardType.shop)),
            new CardData(MakeCardPath(CardType.shop)),
            new CardData(MakeCardPath(CardType.crate)),
            new CardData(MakeCardPath(CardType.crate)),
            new CardData(MakeCardPath(CardType.staircase)),
            new CardData(MakeCardPath(CardType.steelcrate))
        };

        while (starterDeck.Count < Mathf.FloorToInt(deckSize * 0.75))
        {
            starterDeck.Add(new CardData(MakeCardPath(RandType<StarterCardType>())));

        }
        while (starterDeck.Count < deckSize)
        {
            starterDeck.Add(Create(GetRandCardTypeOfCost(5)));
        }


        foreach (CardData card in starterDeck)
        {
            card.inShop = false;
        }

        return starterDeck;
    }

    public static List<CardData> StarterDeck()
    {
        List<CardData> starterDeck = new List<CardData>
        {
            new CardData(MakeCardPath(CardType.obsidian)),
            new CardData(MakeCardPath(CardType.shop))
        };

        while (starterDeck.Count < 10)
        {
            starterDeck.Add(new CardData(GetRandStarterCard()));
        }

        foreach (CardData card in starterDeck)
        {
            card.inShop = false;
        }


        return starterDeck;
    }


    public static List<CardData> OneEachDeck()
    {
        List<CardData> oneEach = new List<CardData>();

        foreach (CardType type in Enum.GetValues(typeof(CardType)))
        {
            oneEach.Add(new CardData(MakeCardPath(type)));
        }

        return oneEach;
    }
    public static List<CardData> DevShenaniganDeck(int? deckSize = null)
    {
        if (deckSize == null)
            deckSize = DeckManager.Instance.deckSize;

        List<CardData> deck = new List<CardData>()
        {
            new CardData(MakeCardPath(CardType.obsidian)),
            new CardData(MakeCardPath(CardType.obsidian)),
            new CardData(MakeCardPath(CardType.obsidian)),
            new CardData(MakeCardPath(CardType.gascan)),
            new CardData(MakeCardPath(CardType.gascan)),
            new CardData(MakeCardPath(CardType.gascan)),
            new CardData(MakeCardPath(CardType.gascan)),
            new CardData(MakeCardPath(CardType.gascan)),
            new CardData(MakeCardPath(CardType.gascan)),
            new CardData(MakeCardPath(CardType.gascan)),
            new CardData(MakeCardPath(CardType.gascan)),
            new CardData(MakeCardPath(CardType.gascan)),
            new CardData(MakeCardPath(CardType.gascan)),
            new CardData(MakeCardPath(CardType.gascan)),
            new CardData(MakeCardPath(CardType.crate)),
            new CardData(MakeCardPath(CardType.crate)),
            new CardData(MakeCardPath(CardType.crate)),
            new CardData(MakeCardPath(CardType.crate)),
            new CardData(MakeCardPath(CardType.crate)),
            new CardData(MakeCardPath(CardType.steelcrate)),
            new CardData(MakeCardPath(CardType.steelcrate)),
        };

        while(deck.Count < deckSize)
        {
            deck.Add(Rand());
        }

        




        return deck;
    }

    public static CardType GetWeightedRandCard()
    {
        int rand = (int)(GD.Randi() % 101);

        //25% chance
        if (rand >= 0 && rand < 29)
        {
            CardType[] cardTypes = new CardType[]
            {
                CardType.toilet,
                //CardType.crate,
                //CardType.scaffleting,
                CardType.tree
            };
            int innerRand = (int)(GD.Randi() % cardTypes.Length);

            return cardTypes[innerRand];
        }
        //25% chance
        else if (rand >= 30 && rand <= 54)
        {
            CardType[] cardTypes = new CardType[]
            {
                CardType.piano,
                CardType.lamp,
                //CardType.staircase,
                CardType.table
            };
            int innerRand = (int)(GD.Randi() % cardTypes.Length);

            return cardTypes[innerRand];
        }
        //30% chance
        else if (rand >= 55 && rand <= 79)
        {
            CardType[] cardTypes = new CardType[]
            {
                CardType.steelcrate,
                CardType.stoneball,
                CardType.truck,
                CardType.mattress
            };
            int innerRand = (int)(GD.Randi() % cardTypes.Length);
            return cardTypes[innerRand];
        }
        //19% chance
        else if (rand >= 80 && rand <= 99)
        {
            CardType[] cardTypes = new CardType[]
            {
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
                CardType.zeekplushy
             };
            int innerRand = (int)(GD.Randi() % cardTypes.Length);


            return cardTypes[innerRand];
        }
        return CardType.crate;
    }


    public static CardType GetRiggedCard()
    {
            CardType[] cardTypes = new CardType[]
            {
                CardType.toilet,
                CardType.ship,
                CardType.shop,
                CardType.obsidian
            };
            int innerRand = (int)(GD.Randi() % cardTypes.Length);

            return cardTypes[innerRand];

    }


}
