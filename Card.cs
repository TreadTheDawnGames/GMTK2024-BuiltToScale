using Godot;
using System;

public partial class Card : Control
{
    
    Area2D GrabbableSprite;
    public CardData Data{get; private set;}
    Sprite2D symbol;

    bool grabbed = false;
    bool hovered = false;
    Vector2 grabbedOffset;
    Vector2 OGPos;

    public void SetUp(CardData data)
    {
        this.Data = data;
        symbol = GetNode<Sprite2D>("Backing/Symbol");
        symbol.Texture = Data.PathToPhysObj.symbol;
        // symbol.Hide();
        OGPos = Data.Slot.Position;

        GrabbableSprite = GetNode<Area2D>("Backing");
        GrabbableSprite.AreaEntered += (node) => DiscardReady();
        GrabbableSprite.AreaExited += (node) => DiscardUnready();
        MouseEntered +=   Hovered;
        MouseExited +=  Unhovered;
    }

    public override void _Process(double delta)
    {
        

        base._Process(delta);
        if(hovered)
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
            else if (!Input.IsMouseButtonPressed(MouseButton.Left)&&grabbed)
            {
                GD.Print("Unpressed");
                Position = OGPos;
                grabbed = false;
            }
        }

        if(grabbed)
        {
            Position = GetGlobalMousePosition() + grabbedOffset;
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
        Data.discardable = true;
        GD.Print("Discardable");
    }

    void DiscardUnready()
    {
        Data.discardable = false;
        GD.Print("NOT discardable");

    }

    void ReadyToPlay()
    {
        Data.playable = true;
        GD.Print("Playable");
    }

    void UnreadyToPlay()
    {
        Data.playable = false;
        GD.Print("NOT Playable");

    }


}

