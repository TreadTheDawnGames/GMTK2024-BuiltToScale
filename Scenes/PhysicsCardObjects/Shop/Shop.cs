using Godot;
using System;
using System.Linq;

public partial class Shop : TextureRect
{
    PermShopSlot[] permCardSlots = new PermShopSlot[3];
	RandShopSlot[] randCardSlots = new RandShopSlot[4];
	//Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		permCardSlots = GetChildren().OfType<PermShopSlot>().ToArray();
		randCardSlots = GetChildren().OfType<RandShopSlot>().ToArray();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
