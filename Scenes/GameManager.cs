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

    int moneyLineHeight = -880;

    PackedScene moneyLineScene = GD.Load<PackedScene>("res://Scenes/money_line.tscn");

    public CameraZoomer Camera { get; private set; }
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
    public player_char Rufus { get; private set; }
    RichTextLabel moneyLabel;
    public Node2D gameOverInst;
    public override void _Ready()
    {
        base._Ready();
        instance = this;
        Camera = GetNode<CameraZoomer>("Camera");
        moneyLabel = GetNode<RichTextLabel>("Camera/CanvasLayer/Deck/MoneyLabel");
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
        DeckManager.Instance.highScoreLabel.Text = "High Score: " + (-highScore -169).ToString();
        DeckManager.Instance.scoreLabel.Text = "Score: " + (-score-169).ToString();

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
        Instance.Rufus.ProcessMode = isPaused? ProcessModeEnum.Disabled: ProcessModeEnum.Inherit;

        foreach (var node in GetTree().GetNodesInGroup("PhysicsObjects"))
        {
            
            if(node is RigidBody2D)
            {
                var rigid = node as RigidBody2D;
                rigid.Freeze = isPaused;
            }

            if (node is physics_object)
            {
               
                var po = node as physics_object;
                if (po.rigids.Count > 0)
                {

                    foreach (var rigid in po.rigids)
                    {
                        if (rigid is physics_body_RigidBody)
                        {

                            var spclRigid = rigid as physics_body_RigidBody;

                            if (spclRigid.Static)
                            {
                                continue;
                            }

                            rigid.Freeze = isPaused;
                        }
                    }
                }
            }
        }
/*
        if (isPaused == true)
        {
            foreach (var physObj in GetTree().GetNodesInGroup("PhysicsObjects"))
                physObj.ProcessMode = ProcessModeEnum.Disabled;
        }
        else
        {
            foreach (var physObj in GetTree().GetNodesInGroup("PhysicsObjects"))
                physObj.ProcessMode = ProcessModeEnum.Inherit;
        }*/
    }
    public override void _Process(double delta)
    {
        base._Process(delta);
        if (musicPlayer.VolumeDb < -20)
            musicPlayer.VolumeDb += 2 * (float)delta;
        if (Instance.HasNode("Rufus") && Instance.Rufus.GlobalPosition.Y > Camera.loseHeight)
        {
            musicPlayer.VolumeDb = -15;
            musicPlayer.Stream = GD.Load<AudioStream>("res://Assets/Sounds/GameOver.wav");
            musicPlayer.Play();

            


            Instance.Rufus.Despawn();

            Instance.Rufus = null;

		    var ps = GD.Load<PackedScene>("res://Scenes/game_over.tscn");
            var inst = ps.Instantiate<Control>();
            inst.AddToGroup("PhysicsObjects");
            Camera.GetNode("CanvasLayer").AddChild(inst);
        }

        UpdateScore();

        /*if (Input.IsActionJustPressed("Debug-PlayerPrefs"))
        {
            PlayerPrefs.DeleteAll();
        } */

        if (Input.IsActionJustPressed("FullScreenToggle"))
        {
            var mode = DisplayServer.WindowGetMode() == DisplayServer.WindowMode.Windowed ? DisplayServer.WindowMode.Fullscreen : DisplayServer.WindowMode.Windowed;
            DisplayServer.WindowSetMode(mode);
            //PlayerPrefs.DeleteAll();
        }

        if (Instance.Rufus!=null && Rufus.Position.Y < moneyLineHeight)
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
        
        if (Instance.Rufus!=null && Mathf.CeilToInt(Rufus.GlobalPosition.Y)<score)
        {
            score = Mathf.CeilToInt(Rufus.GlobalPosition.Y);
            DeckManager.Instance.scoreLabel.Text = "Score: " + (-score-169).ToString();
        }
        

        if(score < highScore)
        {
            highScore = score;
            DeckManager.Instance.highScoreLabel.Text = "High Score: " + (-highScore-169).ToString();
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
