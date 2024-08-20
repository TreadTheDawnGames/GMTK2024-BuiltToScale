using Godot;
using System;
using System.Net.Sockets;
using System.Runtime.Serialization;

public partial class reset_button : Button
{
	Sprite2D sprite, white, black;
	bool hovered = false;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		sprite = GetNode<Sprite2D>(GetParent().GetPath() + "/Sprite2D");
		white = GetNode<Sprite2D>(GetParent().GetPath() + "/Sprite2D/White");
		black = GetNode<Sprite2D>(GetParent().GetPath() + "/Sprite2D/Black");
		MouseEntered += () => hovered = true;
		MouseExited += () => hovered = false;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
        if (hovered)
        {
            if (Input.IsMouseButtonPressed(MouseButton.Left))
            {
                sprite.Scale = sprite.Scale.Lerp(new Vector2(0.90f, 0.90f), 0.25f);
				white.Hide();
				black.Show();
            }
            else
            {
                sprite.Scale = sprite.Scale.Lerp(new Vector2(1.15f, 1.15f), 0.25f);
                black.Hide();
                white.Show();
            }
        }
        else
        {
            sprite.Scale = sprite.Scale.Lerp(new Vector2(1, 1), 0.25f);
            black.Hide();
            white.Hide();

        }
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
