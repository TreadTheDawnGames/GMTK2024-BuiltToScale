using Godot;
using System;

public partial class GameManager : Node2D
{
    private static GameManager instance = null;
	AudioStreamPlayer musicPlayer;
    Camera2D cam;

    int moneyOwned = 20;

    int score = 0;
    int highScore;

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
            musicPlayer = GetTree().Root.GetNode<AudioStreamPlayer>("MusicPlayer");
        UpdateMoney(0);

        highScore = PlayerPrefs.GetInt("HighScore", 0);
            DeckManager.Instance.highScoreLabel.Text = "High Score: " + (-highScore - 49).ToString();
            DeckManager.Instance.scoreLabel.Text = "Score: " + (-score-49).ToString();
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
        if (musicPlayer.VolumeDb < -5)
            musicPlayer.VolumeDb += 2 * (float)delta;
        if (Instance.HasNode("Rufus") && Instance.Rufus.GlobalPosition.Y > cam.GlobalPosition.Y + 1080/2+60)
        {
            musicPlayer.VolumeDb = -10;
            musicPlayer.Stream = GD.Load<AudioStream>("res://Assets/Sounds/GameOver.wav");
            musicPlayer.Play();
            Instance.Rufus.QueueFree();
        }

        UpdateScore();

        if (Input.IsActionJustPressed("Debug-ResetSavedData"))
        {
            PlayerPrefs.DeleteAll();
            GD.Print("Deleted PlayerPrefs");
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

    void UpdateScore()
    {
        
        if (Mathf.CeilToInt(Rufus.GlobalPosition.Y)<score)
        {
            score = Mathf.CeilToInt(Rufus.GlobalPosition.Y);
            DeckManager.Instance.scoreLabel.Text = "Score: " + (-score-49).ToString();
        }
        

        if(score < highScore)
        {
            highScore = score;
            DeckManager.Instance.highScoreLabel.Text = "High Score: " + (-highScore-49).ToString();
            PlayerPrefs.SetInt("HighScore", highScore);
        }
    }
    

}
