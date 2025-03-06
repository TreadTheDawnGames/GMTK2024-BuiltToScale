using Godot;
using System;

public partial class FixedJoint : PinJoint2D
{

	float rotation_fix = 0f;

	// Connect both bodies to the joint, save the relative rotation, and
	// connect signals to disconnect the joint if either body is deleted
	public void ConnectBodies(PhysicsBody2D body1, PhysicsBody2D body2)
	{
		NodeA = body1.GetPath();
		NodeB = body2.GetPath();

		// This rotation fix is used in physics_process to keep the
		// rotation of the parent fixed relative to another body as it rotates
		var angle_to_body = (GlobalPosition - body2.GlobalPosition).Angle();

		rotation_fix = GetParent<Node2D>().GlobalRotation - angle_to_body;
		//The game will crash if connected nodes are removed from the scene
		body1.TreeExiting += DisconnectJoint;
		body2.TreeExiting += DisconnectJoint;
	}




    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);

		// All of the standard physics joints allow free rotation around the joint
		// connection. Godot 3.2 doesn't have the concept of a "fixed joint"
		// but there is a proposal.
		//
		// This fixes the rotation of the parent object relative to the rotation
		// of the connected object. This makes the pinjoint work like the two objects
		// are welded together -- a Fixed Joint.

		if (NodeB != "")
		{
			var nodeBphybod = GetNode<Node2D>(NodeB);

			if (nodeBphybod != null)
			{

				var angle_to_body = (GlobalPosition - nodeBphybod.GlobalPosition).Angle();


				GetParent().SetDeferred("rotation", angle_to_body + rotation_fix);

			}
			else
			{
				NodeB = "";

			}
		}


    }



    
	

public void DisconnectJoint()
	{
		NodeA = "";
		NodeB = "";

	}

	

}
