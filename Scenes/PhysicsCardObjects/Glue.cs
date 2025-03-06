using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class Glue : Area2D
{
	PackedScene splotchPS = GD.Load<PackedScene>("res://Scenes/PhysicsCardObjects/glue_splotch.tscn");
	physics_object Parent;
	AudioStreamPlayer glueSound;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Parent = GetParent<physics_object>();
		glueSound = GetNode<AudioStreamPlayer>(Parent.GetPath() + "/GlueSound");
		glueSound.Finished += Parent.QueueFree;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		foreach (var obj in GetTree().GetNodesInGroup("PhysicsObjects"))
		{
			if (obj is physics_object)
			{

				var physObj = obj as physics_object;

				physObj.selecting = false;
			}
		}
		if (Parent.NeverCheckAgain)
		{
			GD.Print("Placed");
			//Parent.Hide();
			Parent.SetDeferred("visible", false);
			SetDeferred("monitoring", false);
		//			Parent.QueueFree();
		}

		if (!Monitoring)
			return;
        if (!HasOverlappingBodies())
            return;

        List<RigidBody2D> prospectiveNewTargetRigids = new();
		physics_object gluingTarget = null;
		List<Sprite2D> addToTargetSpriteList = new();
		List<Sprite2D> targetSpritelist = new();
		targetSpritelist.Clear();


		


		var potentialGluingTarget = GetOverlappingBodies()[0];
		
			gluingTarget = potentialGluingTarget.GetParent<physics_object>();
			gluingTarget.selecting = true;

		

		//for every rigidbody owned by the gluingTarget
		foreach (var rigid in gluingTarget.rigids)
		{

			foreach (Sprite2D sprite in gluingTarget.SpriteList)
			{
				if (targetSpritelist.Contains(sprite))
					continue;
				targetSpritelist.Add(sprite);
			}               

			//and each body colliding with it
			foreach (var toucher in rigid.GetCollidingBodies())
			{
				//skip non rigidbodies and gascans
				if (toucher is not RigidBody2D || toucher is gascan)
				{
					continue;
				}

				//skip if it's not a physics_object
				if (toucher.GetParent() is not physics_object)
				{

					GD.Print("Parent is not PhysObj: " + toucher.GetParent().Name);
					continue;
				}


				//convert the body to a rigidbody
				var toucherRigid = toucher as RigidBody2D;



				var toucherParent = toucher.GetParent<physics_object>();
				foreach (Sprite2D sprite in toucherParent.SpriteList)
				{
					if (targetSpritelist.Contains(sprite))
						continue;
					targetSpritelist.Add(sprite);
				}

				//get the physics_object of the colliding rigid

				if (Parent.NeverCheckAgain)
				{
					//set flag to false (for animation)
					gluingTarget.selecting = false;

                    
                    //foreach existing splotch
                    //delete splotch

                    //check for active collision points
                    //foreach collision point
                    //instantiate splotch
                    //splotch.position = collisionPoint.position
                    //gluingtarget.addchild(splotch)

                    //					GetWorld2D().DirectSpaceState.IntersectShape()

                    /*var theThing = rigid.CreateShapeOwner(rigid);
					rigid.shapeID
					var c_shape = rigid.ShapeOwnerGetShape(theThing,) //to get the collision Shape2D

var intersect_list = c_shape.collide_and_get_contacts(Matrix32 local_xform, Shape2D with_shape, Matrix32 shape_xform)*/

                    if (rigid is physics_body_RigidBody)
					{
						var pbrb = (physics_body_RigidBody)rigid;
						foreach(var contactPoint in pbrb.ContactPoints)
						{

						GD.Print("You a table boi");

						var splotchPS = GD.Load<PackedScene>("res://Scenes/PhysicsCardObjects/glue_splotch.tscn");
						var splotch = splotchPS.Instantiate<Node2D>();
						splotch.Position = pbrb.ToLocal(contactPoint);
							pbrb.AddChild(splotch);
						}
						pbrb.Parent = gluingTarget;
					}








                    DoGluing(prospectiveNewTargetRigids, gluingTarget, rigid, toucherParent);

					foreach (var sprite in targetSpritelist)
					{
						if (addToTargetSpriteList.Contains(sprite))
							continue;
						addToTargetSpriteList.Add(sprite);

					}
				}
			}

			GD.Print(targetSpritelist.Count);
			foreach (var sprite in targetSpritelist)
			{
				var actualSprite = (Sprite2D)sprite;
				actualSprite.Scale = actualSprite.Scale.Lerp(new Vector2(1.1f, 1.1f), 0.25f);
				actualSprite.Modulate = new Color(2, 2, 2, 1);

			}


			

			//QueueFree();
		}
		if (Parent.NeverCheckAgain)
		{
			//set flag to false (for animation)
			PlayGlueSound();
			gluingTarget.selecting = false;
		}


            if (addToTargetSpriteList.Count>0)
		{
			foreach (Sprite2D sprite in addToTargetSpriteList)
			{
				if (gluingTarget.SpriteList.Contains(sprite))
					continue;

				gluingTarget.SpriteList.Add(sprite);
			}
		}

		if (gluingTarget != null && prospectiveNewTargetRigids != null)
		{
			foreach (var body in prospectiveNewTargetRigids)
			{
                if (gluingTarget.rigids.Contains(body))	
					continue;
                gluingTarget.rigids.Add(body);
			}

		}

	}

	private static void DoGluing(List<RigidBody2D> prospectiveNewTargetRigids, physics_object gluingTarget, RigidBody2D rigid, physics_object toucherParent)
    {
		List<Node> specialNodes = new();
        foreach(var specialNode in toucherParent.SpecialNodes)
		{
			if(specialNodes.Contains(specialNode))
				continue; 
			specialNodes.Add(specialNode);
		}

        foreach (var body in toucherParent.rigids)
		{
			if (body.GetNodeOrNull<DoublePinJoint>("DoublePinJoint") != null)
			{
				body.GetNodeOrNull<DoublePinJoint>("DoublePinJoint").QueueFree();

			}



			body.Reparent(gluingTarget);
			body.Owner = gluingTarget;
			prospectiveNewTargetRigids.Add(body);
			

			//create fixed joint
			var fixedJointPS = GD.Load<PackedScene>("res://Scenes/double_pin_joint.tscn");
			var fixedJoint = fixedJointPS.Instantiate<DoublePinJoint>();
			rigid.AddChild(fixedJoint);


			//connect fixed points
			fixedJoint.ConnectJoints(rigid, body);
		}

        foreach (var node in specialNodes)
        {

            if (node is ShopArea)
            {
                var shopArea = (ShopArea)node;

                if (shopArea != null)
                {
                    var shop = (ShopArea)shopArea;
                    GD.Print("ShopArea not null");
                    //shop.Reparent(gluingTarget);
                    shop.SetUp(gluingTarget);

                }
                else
                {
                    GD.Print("ShopAreaNull");
                }
            }
            gluingTarget.SpecialNodes.Add(node);
        }
    }

    void PlayGlueSound()
    {
        int rand = (int)(GD.Randi() % 2);
        glueSound.Stream = GD.Load<AudioStream>("res://Assets/Sounds/Cards/GlueSounds/Glue" + rand + ".wav");

        glueSound.Play();
    }
}

