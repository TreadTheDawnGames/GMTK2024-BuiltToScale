using Godot;
using System;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Runtime.Serialization;

public partial class gascan : RigidBody2D
{


	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		ContactMonitor = true;
		MaxContactsReported = 10;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (GetCollisionLayerValue(1) != false)
		{
			var hit = GetCollidingBodies();
			if (hit != null && hit.Count > 0)
			{
				Vector2 nowvel2d = LinearVelocity;
				foreach (var obj in hit)
					if (obj is RigidBody2D)
					{
						if (obj.Name != "Rufus")
							nowvel2d += ((RigidBody2D)obj).LinearVelocity;
					}
				if (hit[0].Name == "Rufus" && hit.Count == 1)
					nowvel2d = new Vector2(0,0);
				var nowvel = Math.Sqrt(Math.Pow(nowvel2d.X, 2) + Math.Pow(nowvel2d.Y,2));
				GD.Print(nowvel);


				if (nowvel > 0)
				{
					GetParent().QueueFree();
					foreach (var obj in GetTree().GetNodesInGroup("PhysicsObjects"))
					{
						if (obj is RigidBody2D)
						{
							var objNode = (Node2D)obj;
							var force = 0f;// / ((RigidBody2D)obj).GlobalPosition.DistanceTo(GlobalPosition);
							var angle = ((RigidBody2D)obj).GetAngleTo(GlobalPosition);

							float ex = force * (float)Math.Cos(angle);
							float ey = force * (float)Math.Sin(angle);

							var eDir =  objNode.GlobalPosition - GlobalPosition;

                            /*							var eDir = new Vector2(ex, ey) *0;
                            */
                            var e = (force * (0.5f*((RigidBody2D)obj).GlobalPosition.DistanceTo(GlobalPosition)));                //if (force > 0.0625)
                            ((RigidBody2D)obj).ApplyForce(eDir*e, GlobalPosition);
						}
						else if (obj.GetChildOrNull<RigidBody2D>(0) != null)
						{
							var child = obj.GetChild<RigidBody2D>(0);
                            /*var rigid = obj.GetChild<RigidBody2D>(0); // obj.GetNode<RigidBody2D>(obj.Name + "/RigidNode2D");
							var force = 1 / rigid.GlobalPosition.DistanceTo(GlobalPosition);
							var angle = rigid.GetAngleTo(GlobalPosition);
							float ex = force * (float)Math.Cos(angle);
							float ey = force * (float)Math.Sin(angle);
							//if (force > 0.00625)
							rigid.ApplyForce(new Vector2(ex, ey) * 100000000, GlobalPosition);*/
                            var objNode = (Node2D)child;
							var objBody = (RigidBody2D)child;


                            var force = 10000;// / ((RigidBody2D)obj).GlobalPosition.DistanceTo(GlobalPosition);
                            var angle = (objBody).GetAngleTo(GlobalPosition);

                            float ex = force * (float)Math.Cos(angle);
                            float ey = force * (float)Math.Sin(angle);

							
							
                            var eDir =  child.GlobalPosition-GlobalPosition;
							QueueRedraw();
							GD.Print(eDir);

							/*							var eDir = new Vector2(ex, ey) *0;
                            */
							int maxLength = 1100;

							var length = eDir.Length();

							/*if (eDir.Length() < maxLength)*/
							{


								var e = force /** length/maxLength*//*-Mathf.Pow(length,2)*/;// force / (1/eDir.Length());				//if (force > 0.0625)
								((RigidBody2D)child).ApplyImpulse(eDir.Normalized() * e, child.ToLocal(GlobalPosition));
							
								
							}

							
                        }
					}
				}
			}
		}



    }

	public override void _Draw()
	{
		base._Draw();

		

		//DrawCircle(Position, 200f, new Color(1, 0, 0, 1));
	}

}
