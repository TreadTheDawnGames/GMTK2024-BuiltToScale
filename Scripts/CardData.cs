using Godot;
using System;
using static CardAssembler;

public partial class CardData : Node
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
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
