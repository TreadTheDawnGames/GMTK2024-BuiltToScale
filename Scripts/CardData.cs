using Godot;
using System;

public partial class CardData : Node
{

    public string PathToPhysObj { get; private set; }
    public bool discardable;
    public bool playable;
    public CardSlot Slot;

    public Vector2 OGPosition;

    public int cost;

    public CardData(string obj)
    {
        this.PathToPhysObj = obj;
        discardable = false;
        playable = false;
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
