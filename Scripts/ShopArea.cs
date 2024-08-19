using Godot;
using System;

public partial class ShopArea : Area2D
{
	private bool isInteractable = false;
	private bool used = false;
	private Sprite2D backSprite;
	private RigidBody2D rigid;
	player_char lastRufus;

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
			if (Input.IsActionJustPressed("OpenShop"))
			{
				OpenShop();

				GameManager.Instance.SetPauseGame(true);
				used = true;
			}
		}
			HandleShiftyThoughts();
	}

	void HandleShiftyThoughts()
	{
        if (!used && rigid.GetCollisionLayerValue(1) == true)
        {
            if (HasOverlappingBodies())
            {
                foreach (var body in GetOverlappingBodies())
                {

                    if (body is player_char)
                    {


                        player_char rufus = (player_char)body;
                        lastRufus = rufus;
                        lastRufus.ThinkShifyThoughts(true);

                    }
                }
            }
            else
            {
				if (lastRufus != null)
				{
	                lastRufus?.ThinkShifyThoughts(false);
					lastRufus=null;
				}
            }
        }
        else
        {
			if (lastRufus != null)
			{
				lastRufus?.ThinkShifyThoughts(false);
				lastRufus = null;
			}			
        }
    }

	public void _on_body_entered(Node2D body)
	{
		

		isInteractable = true;
	}

	public void _on_body_exited(Node2D body)
	{
		
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
