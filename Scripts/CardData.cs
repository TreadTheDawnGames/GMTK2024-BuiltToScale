using Godot;
using System;
using System.Collections.Generic;
using static CardAssembler;

public partial class CardData : Node, IComparable<CardData>
{

    public string   PathToPhysObj { get; private set; }
    public bool     buyable;
    public bool     playable;
    public bool     sellable;
    public CardSlot Slot;
    public bool     singleUse;

    public CardAssembler.CardType Type;

    public Vector2 OGPosition;
    public bool inShop = true;

    public int cost;
    public Texture2D symbol;
    public bool aesthetic;
    public CardData(string obj)
    {
        this.PathToPhysObj = obj;
        buyable = false;
        playable = false;
        sellable = false;
       var ps = GD.Load<PackedScene>(obj);
        var myObject = ps.Instantiate<physics_object>();
        cost = myObject.cost;
        symbol = myObject.symbol;
        singleUse = myObject.singleUse;
        aesthetic = false;

        foreach (CardType type in Enum.GetValues(typeof(CardType)))
        {
            if (obj.Contains(type.ToString()))
            {
                Type = type;
                break;
            }
        }
    }

    public CardData(CardData oldData)
    {
        this.PathToPhysObj = oldData.PathToPhysObj;
        this.buyable = oldData.buyable;
        this.playable = oldData.playable;
        this.sellable = oldData.sellable;
        this.cost = oldData.cost;
        this.symbol = oldData.symbol;
        this.singleUse = oldData.singleUse;
        this.aesthetic = oldData.aesthetic;
        this.Type = oldData.Type;
    }

    public CardData Copy()
    {
        return new CardData(this);
    }

    public override string ToString()
    {

        string data = "" +
              "PathToPhysObj: "  + PathToPhysObj   + "\n"
            + "boyable: "        + buyable         + "\n"
            + "playable: "       + playable        + "\n"
            + "sellable: "       + sellable        + "\n"
            + "Slot: "           + Slot            + "\n"
            + "singleUse: "      + singleUse       + "\n"
            + "Type: "           + Type            + "\n"
            + "OGPosition: "     + OGPosition      + "\n"
            + "inShop: "         + inShop          + "\n"
            + "cost: "           + cost            + "\n"
            + "symbol"           + symbol          + "\n"
            ;

        
        
       
        return data;
    }

    public int CompareTo(CardData other)
    {
        /*int alp = Type.ToString().CompareTo(other.Type.ToString());
        if (alp == 0)
        {
            return cost - other.cost;

        }
        else return alp;*/

        if (cost == other.cost)
        {
            return Type.ToString().CompareTo(other.Type.ToString());
        }
        else
        {
            return cost - other.cost;

        }

    }

    public bool IsOfCardType<T>() where T : Enum
    {
        
        foreach(var enumType in Enum.GetValues(typeof(T)))
        {
            if(Type.ToString() == enumType.ToString())
            {
                return true;
            }
        }

        return false;
    }

    

}
