using Godot;
using System;

public partial class CustomerChar : CharacterBody2D
{
	float grav = 40f;
	float acc = 100f;
	float spd = 500f;
	float dec = 100f;
	float jump = -1000f;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _PhysicsProcess(double delta)
	{
		var vel = Velocity;

		if (MoveAndCollide(new Vector2(0, 1), true) == null)
			vel.Y += grav;
		else
		{
			vel.Y = 0;
			if (Input.IsActionJustPressed("Jump"))
				vel.Y = jump;
		}

		if (Input.IsActionPressed("Right") && vel.X < spd)
			vel.X += acc;
		if (Input.IsActionPressed("Left") && vel.X > -spd)
			vel.X -= acc;
		if (!Input.IsActionPressed("Left") && !Input.IsActionPressed("Right"))
			if (vel.X > 0)
				vel.X -= dec;
			else if (vel.X < 0)
				vel.X += dec;

		Velocity = vel;
		MoveAndSlide();
	}
}
