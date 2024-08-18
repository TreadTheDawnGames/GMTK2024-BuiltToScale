using Godot;
using System;

public partial class GameManager : Node2D
{
    private static GameManager instance = null;
	AudioStreamPlayer musicPlayer;
    Camera2D cam;

    int moneyOwned = 20;
    private GameManager()
    {
    }

    public static GameManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new GameManager();
            }
            return instance;
        }
    }
    player_char Rufus;
    RichTextLabel moneyLabel;
    public override void _Ready()
    {
        base._Ready();
        instance = this;
        moneyLabel = GetNode<RichTextLabel>("Camera/Deck/MoneyLabel");
        Instance.Rufus = GetNode<player_char>("Rufus");
        cam = GetNode<Camera2D>("Camera");
        musicPlayer = GetNode<AudioStreamPlayer>("MusicPlayer");
        musicPlayer.Stream = GD.Load<AudioStream>("res://Assets/Sounds/CalmPiggiesLoop.wav");
        musicPlayer.VolumeDb = -40;
        musicPlayer.Play();
        UpdateMoney(0);
    }
    public bool TriggerCard(string CardPath)
	{
        GD.Print(CardPath);
        return Rufus.SpawnObject(CardPath);
	}

    public void SetPauseGame(bool isPaused)
    {
        if (isPaused == true)
        {
            foreach (var physObj in GetTree().GetNodesInGroup("PhysicsObjects"))
                physObj.ProcessMode = ProcessModeEnum.Disabled;
        }
        else
        {
            foreach (var physObj in GetTree().GetNodesInGroup("PhysicsObjects"))
                physObj.ProcessMode = ProcessModeEnum.Inherit;
        }
    }
    public override void _Process(double delta)
    {
        base._Process(delta);
        if (musicPlayer.VolumeDb < -5)
            musicPlayer.VolumeDb += 2 * (float)delta;
        if (Instance.HasNode("Rufus") && Instance.Rufus.GlobalPosition.Y > cam.GlobalPosition.Y + 1080/2+60)
        {
            musicPlayer.VolumeDb = -10;
            musicPlayer.Stream = GD.Load<AudioStream>("res://Assets/Sounds/GameOver.wav");
            musicPlayer.Play();
            Instance.Rufus.QueueFree();
        }
        
    }

    public bool UpdateMoney(int amount)
    {
        moneyOwned += amount;
        if(moneyOwned < 0)
        {
            moneyOwned = 0;
            return false;
        }
        moneyLabel.Text = moneyOwned.ToString();
            return true;
    }


}
