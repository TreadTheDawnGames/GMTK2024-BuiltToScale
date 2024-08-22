using Godot;
using System;

public partial class GameManager : Node2D
{
    [Export]
    int startingMoney = 20;

    private static GameManager instance = null;
	AudioStreamPlayer musicPlayer;
    Camera2D cam;
    int moneyOwned;

    int score = 0;
    int highScore;

    int moneyLineHeight = -1000;

    PackedScene moneyLineScene = GD.Load<PackedScene>("res://Scenes/money_line.tscn");
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
        UpdateMoney(startingMoney);

        highScore = PlayerPrefs.GetInt("HighScore", 0);
        DeckManager.Instance.highScoreLabel.Text = "High Score: " + (-highScore - 49).ToString();
        DeckManager.Instance.scoreLabel.Text = "Score: " + (-score-49).ToString();

        SpawnMoneyLine(moneyLineHeight);
    }

    MoneyLine SpawnMoneyLine(float height)
    {
        var ml = moneyLineScene.Instantiate<MoneyLine>();
        Vector2 goToHeight = new Vector2(1641, height);
        ml.GlobalPosition = goToHeight;
        AddChild(ml);
        return ml;
    }

    public bool TriggerCard(string CardPath)
	{
        try
        {
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

        UpdateScore();

        if (Input.IsActionJustPressed("Debug-PlayerPrefs"))
        {
            PlayerPrefs.DeleteAll();
        }

        if(Instance.HasNode("Rufus") && Rufus.Position.Y < moneyLineHeight)
        {
            moneyLineHeight -= 2000;
            SpawnMoneyLine(moneyLineHeight).PlayDing();
            UpdateMoney(25);


        }

    }

    public bool CanBuy(int speculatedCost)
    {
        if (speculatedCost == 0) return true;

        GD.Print(speculatedCost + " | " + moneyOwned);
        return moneyOwned >= speculatedCost;
    }

    public bool UpdateMoney(int amount)
    {
        moneyOwned += amount;
        if(moneyOwned < 0)
        {
            moneyOwned = 0;
            moneyLabel.Text = moneyOwned.ToString();

            return false;
        }
        moneyLabel.Text = moneyOwned.ToString();
            return true;
    }

    void UpdateScore()
    {
        
        if (Instance.HasNode("Rufus") && Mathf.CeilToInt(Rufus.GlobalPosition.Y)<score)
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

    

    void AwardMoney()
    {
        UpdateMoney(25);

        var ps = GD.Load<PackedScene>("res://Scenes/money_line.tscn");
        var newLine = ps.Instantiate<MoneyLine>();

        Vector2 newHeight = GlobalPosition;
        newHeight.Y = (int)(newHeight.Y - 2000f);

        newLine.GlobalPosition = newHeight;
        GetParent().CallDeferred("add_child", newLine);
    }

}
