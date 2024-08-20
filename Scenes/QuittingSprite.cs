using Godot;
using System;

public partial class QuittingSprite : AnimatedSprite2D
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Hide();
		AnimationFinished += () => CheckCanQuit(allowedToQuit);
	}

	bool allowedToQuit = false;

	void CheckCanQuit(bool canQuit)
	{
		if(canQuit)
		{
			GetTree().Quit();
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (Input.IsActionJustPressed("Quit"))
		{
			Show();
			Play("default");
		}
		else if (Input.IsActionJustReleased("Quit"))
		{
			Hide();
			Stop();
		}
		allowedToQuit = Input.IsActionPressed("Quit");

	}
}
