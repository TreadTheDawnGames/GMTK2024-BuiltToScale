using Godot;
using System;
using System.Xml.Serialization;

public partial class MoneyLine : Node2D
{
	Area2D zone;
	Sprite2D line;
	RichTextLabel nextLineAtText;
	AudioStreamPlayer moneyDing;
	bool active = true;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		zone = GetNode<Area2D>("Area2D");
		line = GetNode<Sprite2D>("Sprite2D");
		nextLineAtText = GetNode<RichTextLabel>("RichTextLabel");
		moneyDing = GetNode<AudioStreamPlayer>("MoneyDing");

		zone.BodyEntered += AwardMoney;

		Vector2 lineOffset;
		lineOffset.Y = 0;
		lineOffset.X = GD.RandRange(0, 540);

		line.Position = lineOffset;
	}

	void AwardMoney(Node2D node)
	{
		if (!active) return;
		active = false;
		GameManager.Instance.UpdateMoney(25);
		moneyDing.Play();
	}
}
