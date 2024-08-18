using Godot;
using System;
using static CardAssembler;

public partial class CardData : Node
{

    public string PathToPhysObj { get; private set; }
    public bool discardable;
    public bool playable;
    public bool sellable;
    public CardSlot Slot;
    public bool singleUse;

    public CardAssembler.CardType Type;

    public Vector2 OGPosition;
    public bool inShop = true;

    public int cost;
    public Texture2D symbol;
    public CardData(string obj)
    {
        this.PathToPhysObj = obj;
        discardable = false;
        playable = false;
        sellable = false;
       var ps = GD.Load<PackedScene>(obj);
        var myObject = ps.Instantiate<physics_object>();
        cost = myObject.cost;
        symbol = myObject.symbol;
        singleUse = myObject.singleUse;

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
        string other = "null";
        if(PathToPhysObj != null)
        {
            other = PathToPhysObj.ToString();
        }
        return "Value: " + other;
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
