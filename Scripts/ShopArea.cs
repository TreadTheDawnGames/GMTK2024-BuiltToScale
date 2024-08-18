using Godot;
using System;

public partial class ShopArea : Area2D
{
	private bool isInteractable = false;
	private bool used = false;
	private Sprite2D backSprite;
	private RigidBody2D rigid;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		backSprite = GetNode<Sprite2D>("../Sprite2D2");
		rigid = GetNode<RigidBody2D>("../../RigidBody2D");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (used == false)
			backSprite.Texture = GD.Load<Texture2D>("res://Assets/Sprites/ShopBack.png");
		else
			backSprite.Texture = GD.Load<Texture2D>("res://Assets/Sprites/ShopBackClosed.png");

		if (isInteractable == true && used == false && rigid.GetCollisionLayerValue(1) == true)
		{

			if (Input.IsActionJustPressed("Debug-OpenShop"))
			{
				OpenShop();

				GameManager.Instance.SetPauseGame(true);
				used = true;
			}
		}
	}

	public void _on_body_entered(Node2D body)
	{
		GD.Print("Ye!!");
		isInteractable = true;
	}

	public void _on_body_exited(Node2D body)
	{
		GD.Print("Oh.");
		isInteractable = false;
	}
	
	public void OpenShop()
    {
        var packedScene = GD.Load<PackedScene>("res://Scenes/PhysicsCardObjects/Shop/Shop.tscn");
        var shop = packedScene.Instantiate();
        DeckManager.Instance.AddChild(shop);
        DeckManager.Instance.MoveChild(shop, 3);
    }
}
