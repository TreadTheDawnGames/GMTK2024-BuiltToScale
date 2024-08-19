using Godot;
using System;

public partial class CardSlot : Marker2D
{

    public bool occupied = false;


    public override void _Ready()
    {
        base._Ready();
        //GetParent<Area2D>().AreaEntered += MarkDrawn;
    }

    void MarkDrawn(Node2D node)
    {
        
       // Card card = (Card)node.Owner;
    }

    public void SetOccupied(bool occupied)
    {
        this.occupied = occupied;
    }

    public override string ToString()
    {
        string str = Name + "\n" + occupied + "\n" + GetType().ToString() + "\n" + "----------";

       
        return str;
    }
}
