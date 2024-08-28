using Godot;
using System;

public partial class SignPost : Area2D
{
	Sprite2D sprite;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		sprite = GetNode<Sprite2D>("Sprite2D");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{

        if (HasOverlappingBodies())
		{
			sprite.Modulate = Modulate.Lerp(new Color(2,2,1, 1), 0.25f);
			sprite.Scale = sprite.Scale.Lerp(new Vector2(1.25f,1.25f),0.25f);
		}
		else
		{
			sprite.Modulate = Modulate.Lerp(new Color(1, 1, 1, 1), 0.25f);
			sprite.Scale = sprite.Scale.Lerp(new Vector2(1f,1f),0.25f);
		}
	}

	public void _on_body_entered(Node2D node)
	{
        var ht = GetTree().Root.GetNode<HowTo>("LevelField/HowTo");
		ht.appear = true;
	}

	public void _on_body_exited(Node2D node)
	{
        var ht = GetTree().Root.GetNode<HowTo>("LevelField/HowTo");
		ht.appear = false;
	}
}
