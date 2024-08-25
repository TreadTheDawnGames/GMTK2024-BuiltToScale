using Godot;
using System;

public partial class game_over : Control
{
    Vector2 TargetPos;
    int wait = 90;
    AnimationPlayer animator;
    Control ResetButton;
	public override void _Ready()
	{
       // Reparent(GetTree().GetFirstNodeInGroup("Camera/CanvasLayer"));
        GetParent().MoveChild(this, 0);
        GetNode<Sprite2D>("NewHigh").Hide();
        var cam = GetTree().Root.GetNode<Camera2D>("LevelField/Camera");
        TargetPos = new Vector2(1920 / 2, 1080 / 2);//cam.GlobalPosition;
//        TargetPos.Y *= 0.5f;
        GlobalPosition = new Vector2(GlobalPosition.X, GlobalPosition.Y - 4000);
        animator = GetNode<AnimationPlayer>("AnimationPlayer");
        ResetButton = GetParent().GetNode<Control>("ResetGameButton");

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
        
            ResetButton.Modulate = ResetButton.Modulate.Lerp(new Color(1, 1, 1, 1), 0.25f);
            ResetButton.Scale = ResetButton.Scale.Lerp(new Vector2(1, 1), 0.25f);
        
        GameManager.Instance.Camera.GameOver = true;
        GameManager.Instance.Camera.FullZoomOut(0.1f); ;
        
        wait--;
    }
}
