using Godot;
using System;

public partial class LoopingBackground : ParallaxBackground
{
	Node2D rufus;
	int nextPosition = 0;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		rufus = GetTree().Root.GetNode<Camera2D>("LevelField/Camera");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (rufus.GlobalPosition.Y < nextPosition)
		{
			var ps = GD.Load<PackedScene>("res://Scenes/cloudsBackground.tscn");
            var inst = ps.Instantiate<Node2D>();
			inst.GlobalPosition = new Vector2(480,-2500+nextPosition);
			AddChild(inst);
			nextPosition += -5000;
		}
	}
}
