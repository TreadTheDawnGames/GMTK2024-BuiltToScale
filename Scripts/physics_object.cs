using Godot;
using System;

public partial class physics_object : Node2D
{
    public override void _Process(double delta)
    {
		var rigid = GetNode<RigidBody2D>("RigidBody2D");
        if (rigid.GetCollisionLayerValue(1) == false)
        {
		    rigid.SetCollisionLayerValue(1, true);
		    rigid.SetCollisionMaskValue(2, true);
            if (rigid.MoveAndCollide(new Vector2(), true) != null)
            {
                rigid.SetCollisionLayerValue(1, false);
                rigid.SetCollisionMaskValue(2, false);
            }
            else
            {
                rigid.SetCollisionLayerValue(1, true);
                rigid.SetCollisionMaskValue(2, true);
            }
        }
    }
}
