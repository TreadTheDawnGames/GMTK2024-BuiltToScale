using Godot;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection.PortableExecutable;
using System.Runtime.Serialization;

public partial class gascan : physics_body_RigidBody
{
    bool exploded = false;

    public Timer fuse;

    bool hovered = false;

    Sprite2D sprite, explosion;


    [Export]
    float fuseTime = 0.75f;
    AudioStreamPlayer explosionSound;

    bool reverseExplosion = false;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
        base._Ready();
        fuse = GetNode<Timer>("Fuse");
        sprite = GetNode<Sprite2D>("CollisionShape2D/Sprite2D");
        explosion = GetNode<Sprite2D>("ExplosionSprite");
        explosionSound = GetNode<AudioStreamPlayer>("AudioStreamPlayer");
        ContactMonitor = true;
		MaxContactsReported = 10;
        fuse.Timeout += Explode;
        fuse.WaitTime = fuseTime;
        explosionSound.Finished += GetParent().QueueFree;

        MouseEntered += Hovered;
        MouseExited += Unhovered;

        var connections = GetIncomingConnections();
        for(int i = 0; i< connections.Count; i++)
        {
            GD.Print(connections[i]);
        }

        explosion.Hide();
        explosion.Scale = Vector2.Zero;

    }

    void Hovered()
    {
        GD.Print("Hovered");
        Parent.selecting = true;
        sineCounter = 0;

        hovered = true;
    }

    void Unhovered()
    {
        hovered = false;
        if(fuse.TimeLeft!>0)
        Parent.selecting = false;
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _PhysicsProcess(double delta)
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
					nowvel2d = new Vector2(0, 0);
				var nowvel = Math.Sqrt(Math.Pow(nowvel2d.X, 2) + Math.Pow(nowvel2d.Y, 2));
                //GD.Print(nowvel);



				if (nowvel > 50 && Parent.NeverCheckAgain)
				{
                    StartFuse(0.01f);
//                  Explode();

				}
			}
		}

        if (hovered && !Parent.isHeld && Parent.NeverCheckAgain)
        {
            sprite.Scale = sprite.Scale.Lerp(new Vector2(1.25f, 1.25f), 0.25f);
            sprite.Modulate = new Color(1, 0, 0, 0.75f);
            if (Input.IsMouseButtonPressed(MouseButton.Left))
            {
                StartFuse(fuseTime);
            }
        }
        else
        {
            sprite.Scale = sprite.Scale.Lerp(new Vector2(1f, 1f), 0.25f);
            sprite.Modulate = new Color(1, 1, 1, 1);

        }
        

        if(fuse.TimeLeft > 0)
        {
            //scaleY = maxY * sin(2 * pi * waves per second * time * startSize)
            var wps = 2.5f;
            var scale = Mathf.Abs(0.4f * Mathf.Sin(2 * Mathf.Pi * wps * (float)fuse.TimeLeft * 1f))+1f;
            sprite.Scale = new Vector2(scale, scale) ;
            sprite.Modulate = new Color(1, 0, 0, 1);

        }

        if (exploded)
        {
            sprite.Modulate = new Color(1, 1, 1, 1);

            explosion.Scale = explosion.Scale.Lerp(reverseExplosion? new Vector2(0f, 0f) : new Vector2(1f, 1f), reverseExplosion ? 0.5f : 0.3f);
            if (explosion.Scale >= new Vector2(0.75f, 0.75f))
            {
                reverseExplosion = true;
            }
        }

       

    }
    int sineCounter = 0;
    public void Explode()
    {
        if (exploded) return;
        
        exploded = true;
        GD.Print(Name + " exploded");

        sprite.Hide();
        explosion.Show();
        Freeze = true;
        GetNode<CollisionPolygon2D>("CollisionShape2D").SetDeferred("disabled", true);
        PlayExplosionSound();

        foreach (var obj in GetTree().GetNodesInGroup("PhysicsObjects"))
        {
            if (obj is physics_object)
            {
                var physObj = (physics_object)obj;
                if (physObj.isHeld)
                {
                    GD.Print(physObj.Name + " is Held");
                    return;
                }
            }
            if (obj is RigidBody2D || obj.GetChildOrNull<RigidBody2D>(0) != null)
            {
                RigidBody2D child;
                if(obj is RigidBody2D)
                {
                     child = (RigidBody2D)obj;
                }
                else if (obj.GetChildOrNull<RigidBody2D>(0) != null)
                {
                    child = obj.GetChild<RigidBody2D>(0);

                }
                else
                {
                    return;
                }

                var force = child == obj ? 300 : 5000;// / ((RigidBody2D)obj).GlobalPosition.DistanceTo(GlobalPosition);

                var eDir = child.GlobalPosition - GlobalPosition;
                QueueRedraw();

                /*							var eDir = new Vector2(ex, ey) *0;
                */
                float maxLength = 500;

                var length = eDir.Length();


                if (eDir.Length() < maxLength && child != this)
                {
                    var spaceState = GetWorld2D().DirectSpaceState;
                    // use global coordinates, not local to node

                    var query = PhysicsRayQueryParameters2D.Create(GlobalPosition, child.GlobalPosition, 8388608);
                    var result = spaceState.IntersectRay(query);

                    if (result.Count > 0)
                    {
                        GD.Print("Hit at point: ", result["position"]);
                        continue;
                    }


                    var e = force * maxLength / length;//*-Mathf.Pow(length,2)*/;// force / (1/eDir.Length());				//if (force > 0.0625)
                    if (child is gascan)
                    {
                        var can = (gascan)child;
                        fuseTime *= (float)Mathf.Clamp(fuseTime* (length/maxLength) * 0.5f, 0.1*fuseTime, fuseTime);

                        can.StartFuse(fuseTime) ;
                        continue;
                    }

                    ((RigidBody2D)child).ApplyImpulse(eDir.Normalized() * e, child.ToLocal(GlobalPosition));
                    //child.ApplyTorqueImpulse(500);



                    
                   

                }
            }
        }
    }

    public void StartFuse(float fuseTime)
    {
        Parent.SpriteList.Remove(explosion);
        Parent.selecting = true;
/*        Parent.SpriteList.Remove(sprite);
        Reparent(GetTree().Root);
*/        if(fuse.TimeLeft <= 0)
        {
            GD.Print(Name + ": " + fuseTime);

            fuse.WaitTime = fuseTime <= 0 ? 0.1f : fuseTime;
            fuse.Start() ;
        }
    }

    
    void PlayExplosionSound()
    {
        //if (soundTick != 0)
        //{
        int rand = (int)(GD.Randi() % 3);
        explosionSound.Stream = GD.Load<AudioStream>("res://Assets/Sounds/GasCan/GasCanExplode" + rand + ".wav");

        explosionSound.Play();
        //}
        //else
        //soundTick = 2;
    }
}
