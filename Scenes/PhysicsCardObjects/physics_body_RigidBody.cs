using Godot;
using System;
using System.Collections.Generic;
using static Godot.WebSocketPeer;

public partial class physics_body_RigidBody : RigidBody2D
{
    public physics_object Parent;

    public bool Static = false;

	public List<Vector2> ContactPoints = new List<Vector2>();
    public override void _Ready()
    {
        base._Ready();
        Parent = GetParent<physics_object>();
        Static = (bool)Parent.GetMeta("Static", false);
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
                
    }

    
}
