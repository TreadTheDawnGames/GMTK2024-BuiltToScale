using Godot;
using System;

public partial class CameraZoomer : Camera2D
{
	Control deck;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		deck = GetNode<Control>("Deck");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if(Input.IsActionPressed("ZoomOut"))
		{
                Zoom = Zoom.Lerp(new Vector2(0.19f, 0.19f), 0.25f);
            deck.Hide();
			deck.ProcessMode = ProcessModeEnum.Disabled;
        }
        else
            {
            Zoom = Zoom.Lerp(new Vector2(1f, 1f), 0.25f);
            deck.Show();
			deck.ProcessMode = ProcessModeEnum.Always ;

            }
	}
}
