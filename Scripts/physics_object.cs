using Godot;
using System;

public partial class physics_object : Node2D
{
    [Export]
    public Texture2D symbol;

    public bool isHeld = false;
    public override void _Process(double delta)
    {
        if (!isHeld)
        {
            var rigid = GetNode<RigidBody2D>("RigidBody2D");
            if (rigid.GetCollisionLayerValue(1) == false)
            {
                rigid.SetCollisionLayerValue(1, true);
                rigid.SetCollisionMaskValue(2, true);
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
                }
            }
        }
    }
}
