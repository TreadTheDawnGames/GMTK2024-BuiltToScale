using Godot;
using System;

public partial class ShopArea : Area2D
{
	private bool isInteractable = false;
	private bool used = false;
	private Sprite2D backSprite;
	private RigidBody2D rigid;
	player_char lastRufus;

	physics_object Parent;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		SetUp((physics_object)Owner);
		Parent.SpecialNodes.Add(this);
		//GD.Print(Parent.SpriteList.Count+ " Shop sprites");
	}

	public void SetUp(physics_object newOwner)
	{
		GD.Print("SettingUp");
        backSprite = GetNodeOrNull<Sprite2D>("../Sprite2D2");
        this.rigid = GetNodeOrNull<RigidBody2D>("../..");
        Owner = Parent = newOwner;

    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
	{
		if (used == false)
			backSprite.Texture = GD.Load<Texture2D>("res://Assets/Sprites/ShopBack.png");
		else
			backSprite.Texture = GD.Load<Texture2D>("res://Assets/Sprites/ShopBackClosed.png");

		if (DeckManager.Instance.atShop)
		{
			isInteractable = false;
			return;
		}
		if (Owner == null|| !HasNode(rigid.GetPath())  || rigid.IsQueuedForDeletion() || rigid == null || !HasNode(Parent.GetPath())|| Parent == null)
		{
			//GD.Print("Re-setting up");
/*            backSprite = GetNode<Sprite2D>("../Sprite2D2");
            this.rigid = GetNode<RigidBody2D>("../..");
			
*/            //SetUp((physics_object)Owner);
            //GD.Print(Parent.ToString(), rigid, backSprite);
            return;
		}
			bool usable = (used == false && rigid.GetCollisionLayerValue(1) == true && Parent.NeverCheckAgain);

		if (HasOverlappingBodies())
		{
			//GD.Print("-" + Parent.Name + " has Bodies at time " + Time.GetTimeStringFromSystem() + "-");
			foreach (var body in GetOverlappingBodies())
			{
				//GD.Print(body.Name);
				if (body == GameManager.Instance.Rufus && usable)
				{
					isInteractable = true;

					continue;
				}
				else
				{
					isInteractable = false;
				}
			}
			//GD.Print("--------");
		}
		else
		{
			isInteractable = false;

		}
		HandleShiftyThoughts(isInteractable);

		if (!isInteractable)
		{
			return;
		}
		if (isInteractable)
		{
			if (Input.IsActionJustPressed("OpenShop") && ((GameManager.Instance.Rufus == null) || (GameManager.Instance.Rufus != null && GameManager.Instance.Rufus.isHolding == false)))
			{
				OpenShop();

				GameManager.Instance.SetPauseGame(true);
				used = true;
			}
		}
		if (used)
		{
            backSprite.Texture = GD.Load<Texture2D>("res://Assets/Sprites/ShopBackClosed.png");
			Parent.SpecialNodes.Remove(this);
            QueueFree();
		}

	} 
	void HandleShiftyThoughts(bool usable)
	{
        if (usable && ((GameManager.Instance.Rufus == null) || (GameManager.Instance.Rufus != null && GameManager.Instance.Rufus.isHolding == false)))
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
		
		if(Parent.NeverCheckAgain)
		{

			//isInteractable = true;
		}
	}

	public void _on_body_exited(Node2D body)
	{
		
        //isInteractable = false;
	}
	
	public void OpenShop()
    {
        var packedScene = GD.Load<PackedScene>("res://Scenes/PhysicsCardObjects/Shop/Shop.tscn");
        var shop = packedScene.Instantiate();
        DeckManager.Instance.AddChild(shop);
        DeckManager.Instance.MoveChild(shop, 3);
		HandleShiftyThoughts(false);
	}

    
}
