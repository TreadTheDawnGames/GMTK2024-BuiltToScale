using Godot;
using System;

public partial class DisplaySlot : CardSlot
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		occupied = true;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
