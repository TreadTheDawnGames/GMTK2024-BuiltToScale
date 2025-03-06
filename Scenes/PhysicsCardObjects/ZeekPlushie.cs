using Godot;
using System;

public partial class ZeekPlushie : physics_body_RigidBody
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		base._Ready();
		Parent.ItemPlaced += Placed;
	}
	void Placed()
	{
		var rufusPos = GameManager.Instance.Rufus.GlobalPosition;
		var goDirection =  GlobalPosition-rufusPos;
		ApplyCentralImpulse(goDirection.Normalized() * 1000);
		ApplyTorqueImpulse(2000 *GD.RandRange(-1,1));
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		
	}
}
