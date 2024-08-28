using Godot;
using System;

public partial class ObsidianRB : RigidBody2D
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}
	bool activated = false;
	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _PhysicsProcess(double delta)
	{
		if (GetParent<physics_object>().NeverCheckAgain &&!activated)
		{
			Freeze = false;
			FreezeMode = FreezeModeEnum.Kinematic;
			Freeze = true;
			activated = true;
			//SetScript("null");
		}
	}
}
