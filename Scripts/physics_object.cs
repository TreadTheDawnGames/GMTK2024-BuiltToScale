using Godot;
using System;

public partial class physics_object : Node2D
{
    [Export]
    public Texture2D symbol;
    [Export]
    public int cost = 10;
    [Export]
    public bool singleUse;

    public bool isHeld = false;
    public bool NeverCheckAgain = false;
    public override void _Process(double delta)
    {
        var rigid = GetNode<RigidBody2D>("RigidBody2D");
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
