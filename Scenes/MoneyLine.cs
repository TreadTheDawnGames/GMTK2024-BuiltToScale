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
		zone = GetNode<Area2D>("Area2D");
		topZone = GetNode<Area2D>("Area2D2");
		line = GetNode<Sprite2D>("Sprite2D");
		nextLineAtText = GetNode<RichTextLabel>("RichTextLabel");
		moneyDing = GetNode<AudioStreamPlayer>("MoneyDing");

		zone.BodyEntered += AttemptToAward;
		topZone.BodyEntered += AttemptToAward;
		zone.BodyExited += EndAttemptAward;
		topZone.BodyExited += EndAttemptAward;

		Vector2 lineOffset;
		lineOffset.Y = 0;
		lineOffset.X = GD.RandRange(0, 540);

		line.Position = lineOffset;

		nextLineAtText.Text = "Next Money Line at " + -(int)(GlobalPosition.Y * 2);
	}

	void AttemptToAward(Node2D node)
	{
		if(!ready)
		{
			ready = true;
			return;
		}
		else
		{
			AwardMoney();
		}

	}

	void EndAttemptAward(Node2D node)
	{
		if(ready)
		{
			ready = false;
		}
	}

	void AwardMoney()
	{
		if (!active) return;
		active = false;
		GameManager.Instance.UpdateMoney(25);
		moneyDing.Play();

		var ps = GD.Load<PackedScene>("res://Scenes/money_line.tscn");
		var newLine = ps.Instantiate<MoneyLine>();

		Vector2 newHeight = Position;
		newHeight.Y = (int)(newHeight.Y * 2f);

		newLine.GlobalPosition = newHeight;
		GetParent().CallDeferred("add_child",newLine);
	}
}
