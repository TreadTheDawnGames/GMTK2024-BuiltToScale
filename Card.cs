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

    Godot.RichTextLabel moneyLabel;

    public void SetUp(CardData data)
    {
        this.Data = data;
        symbol = GetNode<Sprite2D>("Backing/Symbol");

        var physObj = GD.Load<PackedScene>(Data.PathToPhysObj);

        var scene = physObj.Instantiate<physics_object>();

        OGPos = Data.OGPosition;

        symbol.Texture = scene.symbol;

        moneyLabel = (Godot.RichTextLabel)GetNode<Godot.RichTextLabel>("MoneyText");
        moneyLabel.Text = data.cost.ToString();
        // symbol.Hide();

        GrabbableSprite = GetNode<Area2D>("Backing");
        GrabbableSprite.AreaEntered += CardEnteredZone;
        GrabbableSprite.AreaExited += CardExitedZone;
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
                        grabbedOffset = Position - GetGlobalMousePosition();
                        grabbed = true;
                        GetParent().MoveChild(this, GetParent().GetChildCount() - 1);
                    }
                }
                else if (!Input.IsMouseButtonPressed(MouseButton.Left) && grabbed)
                {
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

    void CardEnteredZone(Node2D node)
    {
        switch (node.Name)
        {
            case "PlayArea":
        ReadyToPlay();
                break;

            case "DiscardPile":
        ReadyToDiscard();
                break;

            case "SellPanel":
        ReadyToSell();
                break;
        }
    }
    
    void CardExitedZone(Node2D node)
    {
        switch (node.Name)
        {
            case "PlayArea":
        UnreadyToPlay();
                break;

            case "DiscardPile":
        UnreadyToDiscard();
                break;

            case "SellPanel":
        UnreadyToSell();
                break;
        }
    }


    void ReadyToDiscard()
    {
        if (!grabbed)
            return;
        Data.discardable = true;
    }

    void UnreadyToDiscard()
    {
        if (!grabbed)
            return;
        Data.discardable = false;

    }

    void ReadyToPlay()
    {
        if (!grabbed || Data.inShop)
            return;
        Data.playable = true;
    }

    void UnreadyToPlay()
    {
        if (!grabbed || Data.inShop)
            return; 
        Data.playable = false;

    }

    void ReadyToSell()
    {
        if (!grabbed || Data.inShop)
            return;
        Data.sellable = true;
    }

    void UnreadyToSell()
    {
        if (!grabbed || Data.inShop)
            return; 
        Data.sellable = false;
    }
    


}

