using Godot;
using System;
using System.Xml.Serialization;

public partial class MoneyLine : Node2D
{
	Area2D zone;
	Area2D topZone;
	Sprite2D line;
	RichTextLabel nextLineAtText;
	AudioStreamPlayer moneyDing;
	bool active = true;
	bool ready = false;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		line = GetNode<Sprite2D>("Sprite2D");
		nextLineAtText = GetNode<RichTextLabel>("RichTextLabel");
		moneyDing = GetNode<AudioStreamPlayer>("MoneyDing");

		Vector2 lineOffset;
		lineOffset.Y = 0;
		lineOffset.X = GD.RandRange(-540, 540);

		line.Position = lineOffset;

		nextLineAtText.Text = "Next Money Line at " + -(int)(GlobalPosition.Y - 2000);
	}

    public override void _Process(double delta)
    {
        base._Process(delta);
		if (GameManager.Instance.Camera.Zooming)
		{
            Modulate = Modulate.Lerp(new Color(1, 1, 1, 0), 0.75f);
		}
		else
		{

            Modulate = Modulate.Lerp(new Color(1, 1, 1, 1), 0.1f);
		}

    }


    public void PlayDing()
    {
        moneyDing.Play();
    }

    void EndAttemptAward(Node2D node)
	{
		if(ready)
		{
			ready = false;
		}
	}

	
}
