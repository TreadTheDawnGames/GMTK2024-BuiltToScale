using Godot;
using System;

public partial class GameManager : Node2D
{
	public static GameManager Instance { get; private set; }

    public override void _Ready()
    {
        base._Ready();
        if (Instance == null)
        {
            Instance = this;
        }
    }
    public void TriggerCard(string CardPath)
	{
        GD.Print(CardPath);
	}

}
