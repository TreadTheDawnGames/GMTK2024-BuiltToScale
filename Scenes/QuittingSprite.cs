using Godot;
using System;

public partial class QuittingSprite : AnimatedSprite2D
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Hide();
		Stop();
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
			Modulate = new Color(1, 1, 1, 0);

        }
		else if (Input.IsActionJustReleased("Quit"))
		{
            Play("default", -3);
        }
        allowedToQuit = Input.IsActionPressed("Quit");
		if (allowedToQuit)
		{
			Modulate = new Color(1,1,1,Mathf.Lerp(Modulate.A,1, 0.2f));
		}
		else
		{
			Modulate = new Color(1,1,1,Mathf.Lerp(Modulate.A,0, 0.2f));

		}
	}
}
