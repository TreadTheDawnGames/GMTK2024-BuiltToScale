using Godot;
using System;

public partial class LoopingBackground : ParallaxBackground
{
	Node2D cam;
	int nextPosition = 0;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		cam = GetTree().Root.GetNode<Camera2D>("LevelField/Camera");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (cam.GlobalPosition.Y < nextPosition)
		{
			var ps = GD.Load<PackedScene>("res://Scenes/cloudsBackground.tscn");
            var inst = ps.Instantiate<Node2D>();
			inst.GlobalPosition = new Vector2(480,-2500+nextPosition);
			AddChild(inst);
			
			var ps2 = GD.Load<PackedScene>("res://Scenes/sky_color.tscn");
            var sky = ps2.Instantiate<Node2D>();
			sky.GlobalPosition = new Vector2(480,nextPosition);
			AddChild(sky);

			nextPosition += -5000;
		}
	}
}
