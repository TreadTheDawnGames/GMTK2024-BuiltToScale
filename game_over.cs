using Godot;
using System;

public partial class game_over : Node2D
{
    Vector2 TargetPos;
    int wait = 90;
    AnimationPlayer animator;

	public override void _Ready()
	{
		var cam = GetTree().Root.GetNode<Camera2D>("LevelField/Camera");
        TargetPos = cam.GlobalPosition;
        GlobalPosition = new Vector2(cam.GlobalPosition.X, cam.GlobalPosition.Y - 4000);
        animator = GetNode<AnimationPlayer>("AnimationPlayer");

        int high = PlayerPrefs.GetInt("HighScore", 0);
        int old = PlayerPrefs.GetInt("OldHigh", 0);

        if( high<old )
        {
            GD.Print("New High!");
            PlayerPrefs.SetInt("OldHigh", PlayerPrefs.GetInt("HighScore"));
            GetNode<Sprite2D>("NewHigh").Show();
        }
    }

    public override void _PhysicsProcess(double delta)
    {
        if (wait < 0)
        {
            var pos = GlobalPosition;
            pos.X = TargetPos.X;
            pos.Y += (TargetPos.Y - pos.Y) * .05f;
            GlobalPosition = pos;
        }
        wait--;
    }
}
