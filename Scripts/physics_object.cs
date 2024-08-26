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

    public RigidBody2D rigid;
    public List<Node> shapesList = new List<Node>();
    public List<Sprite2D> SpriteList { get; private set; } = new List<Sprite2D>();
    public bool isHeld = false;
    public bool NeverCheckAgain = false;

    public bool gluing = false;

    public List<Node> SpecialNodes = new List<Node>();

    public override void _Ready()
    {
        base._Ready();
        rigid = GetNode<RigidBody2D>("RigidBody2D");
        rigid.ContactMonitor = true;
        rigid.MaxContactsReported = 25;
        foreach (var node in GetChildren(true).OfType<RigidBody2D>())
        {
            foreach(var shape in node.GetChildren(true))
            {
                shapesList.Add(shape);
                foreach(var sprite in shape.GetChildren(true).OfType<Sprite2D>())
                {

                      SpriteList.Add((Sprite2D)sprite);
                     GD.Print("Added");
                }
            }
        }
    }
    
    void GetAllSprites()
    {
        foreach (var node in GetChildren(true).OfType<Sprite2D>())
        {
            SpriteList.Add((Sprite2D)node);
            GD.Print("Added");
            GetAllSprites();
        }
    }

    public override void _Process(double delta)
    {

        if (!gluing)
        {
            foreach (var item in SpriteList)
            {
                if (item is Sprite2D)
                {
                    var sprt = (Sprite2D)item;
                    sprt.Scale = sprt.Scale.Lerp(new Vector2(1, 1), 0.25f);

                }
            }
        }

        if (HasNode("LockedSprite"))
        {
            var locked = GetNode<Sprite2D>("LockedSprite");
            if (locked != null)
                locked.GlobalPosition = rigid.GlobalPosition;
        }
        if (!isHeld && !NeverCheckAgain)
        {
            rigid.SetCollisionLayerValue(4, true);
            rigid.SetCollisionMaskValue(4, true);
            rigid.SetCollisionLayerValue(1, false);
            rigid.SetCollisionMaskValue(1, false);
            if (rigid.GetCollisionLayerValue(1) == false)
            {
                rigid.SetCollisionLayerValue(1, true);
                rigid.SetCollisionMaskValue(2, true);
                rigid.SetCollisionLayerValue(4, false);
                rigid.SetCollisionMaskValue(4, false);
                if (rigid.MoveAndCollide(new Vector2(), true) != null)
                {
                    var modu = Modulate;
                    modu.A = 0.5f;
                    Modulate = modu;
                    rigid.SetCollisionLayerValue(1, false);
                    rigid.SetCollisionMaskValue(2, false);
                }
                else
                {
                    var modu = Modulate;
                    modu.A = 1;
                    Modulate = modu;
                    rigid.SetCollisionLayerValue(1, true);
                    rigid.SetCollisionMaskValue(2, true);
                    NeverCheckAgain = true;
                    if (rigid.HasNode("RigidBody2DBackground"))
                    {
                        var rigidBackground = rigid.GetNode<RigidBody2D>("RigidBody2DBackground");
                        if (rigidBackground != null)
                            rigidBackground.QueueFree();
                    }
                }
                rigid.SetCollisionLayerValue(4, true);
                rigid.SetCollisionMaskValue(4, true);
            }
            rigid.SetCollisionMaskValue(1, true);
        }
    }

    
}
