using Godot;
using System;
using System.Net.Sockets;

public partial class reset_button : Button
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void _on_button_up()
	{
		GD.Print("Game reset start");
		foreach (var physObj in GetTree().GetNodesInGroup("PhysicsObjects"))
		{
			if (physObj.Name != "Rufus")
				physObj.QueueFree();
		}
		GetTree().ReloadCurrentScene();
	}
}
