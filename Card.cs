using Godot;
using System;

public partial class Card : Control
{
    
    Area2D GrabbableSprite;
    public CardData Data{get; private set;}
    Sprite2D symbol;

    bool grabbed = false;
    bool hovered = false;
    public bool beingDrawn = true;

    Vector2 grabbedOffset;
    public Vector2 OGPos;

    uint OGMask;

    public void SetUp(CardData data)
    {
        this.Data = data;
        symbol = GetNode<Sprite2D>("Backing/Symbol");

        var physObj = GD.Load<PackedScene>(Data.PathToPhysObj);

        var scene = physObj.Instantiate();
        var sprite = scene.GetNode<Sprite2D>("RigidBody2D/Sprite2D");

        OGPos = Data.OGPosition;

        symbol.Texture = sprite.Texture;
        // symbol.Hide();

        GrabbableSprite = GetNode<Area2D>("Backing");
        GrabbableSprite.AreaEntered += (node) => DiscardReady();
        GrabbableSprite.AreaExited += (node) => DiscardUnready();
        GrabbableSprite.AreaEntered += (node) => ReadyToPlay();
        GrabbableSprite.AreaExited += (node) => UnreadyToPlay();
        MouseEntered +=   Hovered;
        MouseExited +=  Unhovered;

        OGMask = GrabbableSprite.CollisionMask;
        SetDrawn(false);
    }

    public void SetDrawn(bool isDrawn)
    {
        GrabbableSprite.CollisionMask = isDrawn ? OGMask : 0;
        //GrabbableSprite.SetCollisionMaskValue(20, true);
        GrabbableSprite.SetCollisionLayerValue(17, isDrawn);

        beingDrawn = !isDrawn;
    }

    public override void _Process(double delta)
    {
        

        base._Process(delta);

            if (hovered)
            {


                if (Input.IsMouseButtonPressed(MouseButton.Left) && !grabbed)
                {
                    if (IsOnTop())
                    {

                        GD.Print("Pressed");
                        grabbedOffset = Position - GetGlobalMousePosition();
                        grabbed = true;
                        GetParent().MoveChild(this, GetParent().GetChildCount() - 1);

                    }
                }
                else if (!Input.IsMouseButtonPressed(MouseButton.Left) && grabbed)
                {
                    GD.Print("Unpressed");
                    grabbed = false;
                }
            }

        if (grabbed)
        {
            Position = GetGlobalMousePosition() + grabbedOffset;
        }
        else
        {
            Vector2 pos = Position;
            pos.X = Mathf.Lerp(Position.X, OGPos.X, 0.1f);
            pos.Y = Mathf.Lerp(Position.Y, OGPos.Y, 0.1f);

            Position = pos;
        }

    }

    bool IsOnTop()
    {
        foreach (Card card in GetTree().GetNodesInGroup("DraggableHovered"))
        {
            if(card.GetIndex() > GetIndex())
            {
                return false;
            }
        }

        return true;
    }

    void Hovered()
    {
        if (beingDrawn)
            return;

        if (Input.IsMouseButtonPressed(MouseButton.Left))
            return;
        
        GD.Print("Hovered");
        AddToGroup("DraggableHovered");

        hovered = true;
    }

    void Unhovered()
    {
        if (Input.IsMouseButtonPressed(MouseButton.Left))
            return;
        RemoveFromGroup("DraggableHovered");

        hovered = false;
    }

    public override Variant _GetDragData(Vector2 atPosition)
    {
        return base._GetDragData(atPosition);
    }


    void DiscardReady()
    {
        if (!grabbed)
            return;
        Data.discardable = true;
        GD.Print("Discardable");
    }

    void DiscardUnready()
    {
        if (!grabbed)
            return;
        Data.discardable = false;
        GD.Print("NOT discardable");

    }

    void ReadyToPlay()
    {
        if (!grabbed)
            return;
        Data.playable = true;
        GD.Print("Playable");
    }

    void UnreadyToPlay()
    {
        if (!grabbed)
            return; 
        Data.playable = false;
        GD.Print("NOT Playable");

    }


}

