using Godot;
using System;

public partial class GameManager : Node2D
{
	public static GameManager Instance { get; private set; }
    player_char Rufus;

    public override void _Ready()
    {
        base._Ready();
        if (Instance == null)
        {
            Instance = this;
        }
        Instance.Rufus = GetNode<player_char>("Rufus");
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
}
