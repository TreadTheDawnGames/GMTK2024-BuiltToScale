using Godot;
using System;

public partial class CardSlot : Marker2D
{

    public bool occupied = false;


    public override void _Ready()
    {
        base._Ready();
        GetParent<Area2D>().AreaEntered += MarkDrawn;
    }

    void MarkDrawn(Node2D node)
    {
        Card card = (Card)node.Owner;
        card.SetDrawn(true);

    }
}
