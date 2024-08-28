using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class physics_object : Node2D
{
    [Export]
    public Texture2D symbol;
    [Export]
    public int cost = 10;
    [Export]
    public bool singleUse;

    [Export]
    public CardData.BackingColor backingColor { get; private set; } = CardData.BackingColor.green;

    public List<RigidBody2D> rigids = new();
    public List<Node> shapesList = new List<Node>();
    public List<Sprite2D> SpriteList { get; private set; } = new List<Sprite2D>();
    public bool isHeld = false;
    public bool NeverCheckAgain = false;

    public bool selecting = false;

    public List<Node> SpecialNodes = new List<Node>();
    [Signal]
    public delegate void ItemPlacedEventHandler();

    public override void _Ready()
    {
        
        base._Ready();
        rigids.Add(GetNode<RigidBody2D>("RigidBody2D"));
        rigids[0].ContactMonitor = true;
        rigids[0].MaxContactsReported = 25;
        foreach (var node in GetChildren(true).OfType<RigidBody2D>())
        {
            GetAllSprites(node);
        }
        GD.Print(SpriteList.Count + " " + Name + " sprites");
        
    }
    
    void GetAllSprites(Node starterNode)
    {
        foreach (var node in starterNode.GetChildren(true))
        {
            if(node is Sprite2D)
            {
                SpriteList.Add((Sprite2D)node);
                GD.Print("Added");
            }
            GetAllSprites(node);
        }
    }

    public override void _Process(double delta)
    {
        if (!selecting)
        {
            foreach (var item in SpriteList)
            {
                if (HasNode(item.GetPath())&&item != null)
                {

                    if (item is Sprite2D)
                    {
                        
                        var sprt = (Sprite2D)item;
                        sprt.Scale = sprt.Scale.Lerp(new Vector2(1, 1), 0.25f);
                        sprt.Modulate = new Color(1,1,1, 1);
                        

                    }
                }
            }
        }

        if (HasNode("LockedSprite"))
        {
            var locked = GetNode<Sprite2D>("LockedSprite");
            if (locked != null)
                locked.GlobalPosition = rigids[0].GlobalPosition;
        }
        if (!isHeld && !NeverCheckAgain)
        {
            rigids[0].SetCollisionLayerValue(4, true);
            rigids[0].SetCollisionMaskValue(4, true);
            rigids[0].SetCollisionLayerValue(1, false);
            rigids[0].SetCollisionMaskValue(1, false);
            if (rigids[0].GetCollisionLayerValue(1) == false)
            {
                rigids[0].SetCollisionLayerValue(1, true);
                rigids[0].SetCollisionMaskValue(2, true);
                rigids[0].SetCollisionLayerValue(4, false);
                rigids[0].SetCollisionMaskValue(4, false);
                if (rigids[0].MoveAndCollide(new Vector2(), true) != null)
                {
                    var modu = Modulate;
                    modu.A = 0.5f;
                    Modulate = modu;
                    rigids[0].SetCollisionLayerValue(1, false);
                    rigids[0].SetCollisionMaskValue(2, false);
                }
                else
                {
                    var modu = Modulate;
                    modu.A = 1;
                    Modulate = modu;
                    rigids[0].SetCollisionLayerValue(1, true);
                    rigids[0].SetCollisionMaskValue(2, true);
                    EmitSignal(SignalName.ItemPlaced);
                    NeverCheckAgain = true;
                    if (rigids[0].HasNode("RigidBody2DBackground"))
                    {
                        var rigidBackground = rigids[0].GetNode<RigidBody2D>("RigidBody2DBackground");
                        if (rigidBackground != null)
                            rigidBackground.QueueFree();
                    }
                }
                rigids[0].SetCollisionLayerValue(4, true);
                rigids[0].SetCollisionMaskValue(4, true);
            }
            rigids[0].SetCollisionMaskValue(1, true);
        }
    }

    
}
