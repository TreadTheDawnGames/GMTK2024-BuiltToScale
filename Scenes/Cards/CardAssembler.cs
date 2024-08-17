using Godot;
using System;

public static class CardAssembler 
{

	public static CardData Rand()
	{

        

        return new CardData(GetRandType());
	}

    static string GetRandType()
    {
        int rand = (int)GD.RandRange(0, 2);

        string originPath = "res://Scenes/PhysicsCardObjects/";
        switch (rand)
        {
            case 0:
                originPath += "crate.tscn";
                break;
            case 1:
                originPath += "obsidian.tscn";
                break;
            case 2:
                originPath += "shop.tscn";
                break;

            default:
                originPath += "crate.tscn";
                GD.Print("GOT BAD NUMBER");
                break;

        }

        return originPath;
    }

    static CardData[] Starter()
    {
        return null;
    }
}
