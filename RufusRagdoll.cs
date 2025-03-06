using Godot;
using System;

public partial class RufusRagdoll : RigidBody2D
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{

		//ApplyTorqueImpulse(50);
		Rotation = GD.Randi() %2 == 0 ? 25 : -25;
			AddToGroup("PhysicsObjects");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		foreach (var body in GetCollidingBodies())
		{
			if (body is StaticBody2D)
			{
		//		Modulate = Modulate.Lerp(new Color(1, 1, 1, 0), 0.1f);
			}
		}



	}
}
