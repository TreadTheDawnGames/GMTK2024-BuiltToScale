using Godot;
using System;
using System.Data.Common;

public partial class Card : Control
{
    [Export]
    Texture2D reg, singleUse;

    Area2D GrabbableSprite;
    public CardData Data{get; private set;}

    Sprite2D background;
    Sprite2D symbol;

    bool grabbed = false;
    bool hovered = false;
    public bool beingDrawn = true;

    Vector2 grabbedOffset;
    public Vector2 OGPos;

    bool usable;

    uint OGMask;

    Godot.RichTextLabel moneyLabel;

    public void SetUp(CardData data)
    {
        this.Data = data;
        symbol = GetNode<Sprite2D>("Backing/Background/Symbol");
        background = GetNode<Sprite2D>("Backing/Background");

        if (data.singleUse)
        {
            background.Texture = singleUse;
        }
        else
        {
            background.Texture = reg;
        }

        var physObj = GD.Load<PackedScene>(Data.PathToPhysObj);

        var scene = physObj.Instantiate<physics_object>();

        OGPos = Data.OGPosition;

        symbol.Texture = scene.symbol;

        moneyLabel = (Godot.RichTextLabel)GetNode<Godot.RichTextLabel>("Backing/Background/MoneyText");
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
            if (!usable)
            {
                if(grabbed)
                {
                    background.Scale = background.Scale.Lerp(new Vector2(1.33f, 1.33f), 0.25f);

                }
                else
                {
                    background.Scale = background.Scale.Lerp(new Vector2(1.25f, 1.25f), 0.25f);

                }
            }
            else
            {
                background.Scale = background.Scale.Lerp(new Vector2(0.75f, 0.75f), 0.25f);

            }

            if (Input.IsMouseButtonPressed(MouseButton.Left) && !grabbed)
            {
                if (IsOnTop())
                {
                    grabbedOffset = Position - GetGlobalMousePosition();
                    grabbed = true;
                }
            }
            else if (!Input.IsMouseButtonPressed(MouseButton.Left) && grabbed)
            {
                grabbed = false;
            }
        }
        else
        {
            background.Scale = background.Scale.Lerp(new Vector2(1, 1), 0.25f);

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
        GetParent().MoveChild(this, GetParent().GetChildCount() - 1);

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
        usable = (Data.discardable || Data.playable || Data.sellable);

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
        usable = (Data.discardable || Data.playable || Data.sellable);
    }


    void ReadyToDiscard()
    {
        if (!grabbed || !Data.inShop)
            return;
        Data.discardable = true;
        GD.Print("Discardable");
    }

    void UnreadyToDiscard()
    {
        if (!grabbed || !Data.inShop)
            return;
        Data.discardable = false;
        GD.Print("NOT Discardable");

    }

    void ReadyToPlay()
    {
        if (!grabbed || Data.inShop)
            return;
        Data.playable = true;
        GD.Print("Playable");

    }

    void UnreadyToPlay()
    {
        if (Data.inShop)
            return; 
        Data.playable = false;
        GD.Print("NOT Playable");

    }

    void ReadyToSell()
    {
        
        if (!grabbed || Data.inShop)
            return;
        Data.sellable = true;
        GD.Print("Sellable");

    }

    void UnreadyToSell()
    {
        if (!grabbed || Data.inShop)
            return; 
        Data.sellable = false;
        GD.Print("NOT Sellable");

    }



}

