using Godot;
using System;

public partial class GameManager : Node2D
{
    private static GameManager instance = null;

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
        UpdateMoney(0);
    }
    public bool TriggerCard(string CardPath)
	{
        GD.Print(CardPath);
        return Rufus.SpawnObject(CardPath);
	}

    public void SetPauseGame(bool isPaused)
    {
        GetTree().Paused = isPaused;
    }
    public override void _Process(double delta)
    {
        base._Process(delta);

        
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
