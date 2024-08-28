using Godot;
using System;
using System.Collections.Generic;
using static Godot.WebSocketPeer;

public partial class physics_body_RigidBody : RigidBody2D
{
	public List<Vector2> ContactPoints = new List<Vector2>();
    public override void _Ready()
    {
        base._Ready();
    }
    public override void _IntegrateForces(PhysicsDirectBodyState2D state)
    {
        base._IntegrateForces(state);
        //this check is needed or it will throw errors 
        if(state.GetContactCount() >= 1)
        {
            ContactPoints.Clear();
            for (int i = 0; i < state.GetContactCount(); i++)
            {
                if (state.GetContactColliderObject(i) is RigidBody2D)
                {
                    var rigid = (RigidBody2D)state.GetContactColliderObject(i);
                    if (rigid.GetCollisionLayerValue(3))
                    {
                         var local_collision_pos = state.GetContactLocalPosition(i);
                        if (ContactPoints.Contains(local_collision_pos))
                            continue;
                        ContactPoints.Add(local_collision_pos);

                    }
                }
            }
        }
                GD.Print(ContactPoints.Count);
                
    }

    
}
