using Godot;
using System;

public partial class DoublePinJoint : PinJoint2D
{

	[Export]
	PhysicsBody2D A, B;

	PinJoint2D childPin;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{

        //ConnectJoints(A, B);
	}


	public void ConnectJoints(PhysicsBody2D body1, PhysicsBody2D body2)
	{

        childPin = GetNode<PinJoint2D>("PinJoint2D");
        NodeA = body1.GetPath();
		NodeB = body2.GetPath();
		GlobalPosition = GetParent<Node2D>().GlobalPosition;

        childPin.GlobalPosition = body2.GlobalPosition;
		childPin.NodeA = NodeB;
		childPin.NodeB = NodeA;
	}

    
}
