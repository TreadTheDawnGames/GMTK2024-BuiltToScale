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
    public Node2D gameOverInst;
    public override void _Ready()
    {
        base._Ready();
        instance = this;
        moneyLabel = GetNode<RichTextLabel>("Camera/Deck/MoneyLabel");
        Instance.Rufus = GetNode<player_char>("Rufus");
        cam = GetNode<Camera2D>("Camera");
        if (!GetTree().Root.HasNode("MusicPlayer"))
        {
            musicPlayer = GetNode<AudioStreamPlayer>("MusicPlayer");
            // musicPlayer.Reparent(GetTree().Root, false);
            musicPlayer.CallDeferred("reparent", GetTree().Root, false);
            musicPlayer.Stream = GD.Load<AudioStream>("res://Assets/Sounds/CalmPiggiesLoop.wav");
            musicPlayer.VolumeDb = -40;
            musicPlayer.Play();
        }
        else
        {
            musicPlayer = GetTree().Root.GetNode<AudioStreamPlayer>("MusicPlayer");
            if (musicPlayer.Stream.ResourcePath.GetFile() != "CalmPiggiesLoop.wav")
            {
                musicPlayer.Stop();
                musicPlayer.Stream = GD.Load<AudioStream>("res://Assets/Sounds/CalmPiggiesLoop.wav");
                musicPlayer.VolumeDb = -40;
                musicPlayer.Play();
            }
        }
        UpdateMoney(0);
    }
    public bool TriggerCard(string CardPath)
	{
        try
        {

        GD.Print(CardPath);
        return Rufus.SpawnObject(CardPath);
        }
        catch
        {
            return false;
        }
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
        if (musicPlayer.VolumeDb < -20)
            musicPlayer.VolumeDb += 2 * (float)delta;
        if (Instance.HasNode("Rufus") && Instance.Rufus.GlobalPosition.Y > cam.GlobalPosition.Y + 1080/2+60)
        {
            musicPlayer.VolumeDb = -15;
            musicPlayer.Stream = GD.Load<AudioStream>("res://Assets/Sounds/GameOver.wav");
            musicPlayer.Play();
            Instance.Rufus.QueueFree();
            
		    var ps = GD.Load<PackedScene>("res://Scenes/game_over.tscn");
            var inst = ps.Instantiate<Node2D>();
            inst.AddToGroup("PhysicsObjects");
            GetTree().Root.AddChild(inst);
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
