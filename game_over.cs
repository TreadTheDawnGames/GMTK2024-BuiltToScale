using Godot;
using System;

public partial class game_over : Node2D
{
    Vector2 TargetPos;
    int wait = 90;

	public override void _Ready()
	{
		var cam = GetTree().Root.GetNode<Camera2D>("LevelField/Camera");
        TargetPos = cam.GlobalPosition;
        GlobalPosition = new Vector2(cam.GlobalPosition.X, cam.GlobalPosition.Y - 4000);
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
