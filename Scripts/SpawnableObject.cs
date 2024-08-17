using Godot;
using System;

public partial class SpawnableObject : Node
{
	public string name;
	public SpawnableObject(string name)
	{
		this.name = name;

		int rand = (int)GD.RandRange(0,2);

		GD.Print("Rand: " + rand);
	Texture2D symbol;
		switch (rand)
		{
            case 0:
                symbol = GD.Load<Texture2D>("res://Assets/Sprites/Objects/Log.png");
                break;
            case 1:
                symbol = GD.Load<Texture2D>("res://Assets/Sprites/Objects/Bowl.png");
                break;
            case 2:
                symbol = GD.Load<Texture2D>("res://Assets/Sprites/Objects/Rock.png");
                break;

			default: 
				GD.Print("GOT BAD NUMBER");
				break;

        }
		
		
	}

	
}
