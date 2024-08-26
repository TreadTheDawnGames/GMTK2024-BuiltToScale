using Godot;
using System;
using System.Linq;

public partial class Glue : Area2D
{
	PackedScene splotchPS = GD.Load<PackedScene>("res://Scenes/PhysicsCardObjects/glue_splotch.tscn");
	physics_object Parent;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Parent = GetParent<physics_object>();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if(Parent.NeverCheckAgain)
		{
			GD.Print("Placed");
			Parent.QueueFree();
		}
		foreach (var obj in GetTree().GetNodesInGroup("PhysicsObjects"))
		{
			if (obj is physics_object)
			{

				var physObj = obj as physics_object;

				physObj.gluing = false;
			}
		}

		foreach (var po in GetOverlappingBodies())
		{
			var gluingTarget = po.GetParent<physics_object>();
				gluingTarget.gluing = true;
			
			
			foreach (var sprite in po.GetParent<physics_object>().SpriteList)
			{
				var actualSprite = (Sprite2D)sprite;
				actualSprite.Scale = actualSprite.Scale.Lerp(new Vector2(1.05f, 1.05f), 0.25f);
				
			}

            

            if (Parent.NeverCheckAgain)
			{
				


				gluingTarget.gluing = false;
				foreach (var toucher in gluingTarget.rigid.GetCollidingBodies())
				{
                    

					

                    if (toucher.GetParent() is physics_object)
					{
						var toucherParent = toucher.GetParent<physics_object>();

						gluingTarget.rigid.Mass += toucherParent.rigid.Mass;



                        foreach (var targetShape in gluingTarget.shapesList)
						{
							if (toucherParent.Equals(targetShape.GetParent()))
							{
								continue;
							}
						}



                        foreach (var shape in toucherParent.shapesList)
						{
							/*PhysicsDirectBodyState2D physicsDirectBodyState2D;
							physicsDirectBodyState2D.*/


							//PhysicsDirectBodyState2D;
                            /*var collisionInfo = toucherParent.rigid.MoveAndCollide(new Vector2(0, 0) * (float)delta);
                            if (collisionInfo != null)
                            {
								

                                var collisionPoint = gluingTarget.ToLocal(collisionInfo.GetPosition());
                                var collisionRotation = gluingTarget.ToLocal(collisionInfo.GetNormal());
                                var splotch = splotchPS.Instantiate<Sprite2D>();
                                splotch.Position = collisionPoint;
                                gluingTarget.AddChild(splotch);
                            }*/

                            shape.Reparent(gluingTarget.rigid);
							gluingTarget.shapesList.Add(shape);
							toucherParent.QueueFree();
							GD.Print("Reparented");
						}

						foreach (var node in toucherParent.SpecialNodes)
						{
							gluingTarget.SpecialNodes.Add(node);

							if (node is ShopArea)
							{
								var shopArea = (ShopArea)node;

								if (shopArea != null)
								{
									var shop = (ShopArea)shopArea;
									GD.Print("ShopArea not null");
									shop.SetUp(gluingTarget);
									
								}
								else
								{
									GD.Print("ShopAreaNull");
								}
							}
						}
					}
				}
				//QueueFree();
			}


		}

		
	}
}
