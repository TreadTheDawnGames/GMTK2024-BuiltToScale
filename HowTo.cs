using Godot;
using System;

public partial class HowTo : Node2D
{
	float TargetLocation;
	float StartLocation;
	public bool appear = false;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		TargetLocation = GlobalPosition.X - 350;
		StartLocation = GlobalPosition.X;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (appear == true)
		{
            var pos = GlobalPosition;
            pos.X += (TargetLocation - pos.X) * .05f;
            GlobalPosition = pos;
		}
		else
		{
            var pos = GlobalPosition;
            pos.X += (StartLocation - pos.X) * .05f;
            GlobalPosition = pos;
		}

	}
}
