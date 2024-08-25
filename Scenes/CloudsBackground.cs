using Godot;
using System;

public partial class CloudsBackground : TextureRect
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if(GameManager.Instance.Rufus==null)
			return;
		
		var goToPos = GameManager.Instance.Camera.GlobalPosition;
		var screenX = (GlobalPosition.Y - GameManager.Instance.Camera.GlobalPosition.Y)/* * 0.5f*/;
		goToPos.Y = screenX;
		GlobalPosition = goToPos;

    }
}
