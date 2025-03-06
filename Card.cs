using Godot;
using System;
using System.Data.Common;
using System.Diagnostics;

public partial class Card : Control
{
    
    Area2D GrabbableArea;
    public CardData Data{get; private set;}

    public Sprite2D backing { get; private set; }
    public Sprite2D symbol { get; private set; }

    public bool grabbed = false;
    bool hovered = false;
    public bool beingDrawn = true;

    Vector2 grabbedOffset;
    Vector2 lastMousePos;
    public Vector2 OGPos;

    bool usable;


    public string name { get; private set; }

    uint OGMask;

    Godot.RichTextLabel moneyLabel;
    Rect2 myRect;
    public void SetUp(CardData data, bool isAesthetic = false)
    {
        data.aesthetic = isAesthetic;
        this.Data = data;


        symbol = GetNode<Sprite2D>("Backing/Background/Symbol");
        backing = GetNode<Sprite2D>("Backing/Background");

            backing.Texture = GD.Load<Texture2D>("res://Assets/Sprites/Deck/Cards/BackingColors/"+data.backingColor.ToString()+".png");

        var physObj = GD.Load<PackedScene>(Data.PathToPhysObj);

        var scene = physObj.Instantiate<physics_object>();

        OGPos = Data.OGPosition;

        symbol.Texture = scene.symbol;

        moneyLabel = (Godot.RichTextLabel)GetNode<Godot.RichTextLabel>("Backing/Background/MoneyText");
        moneyLabel.Text = data.cost.ToString();
        // symbol.Hide();

        GrabbableArea = GetNode<Area2D>("Backing");
        GrabbableArea.AreaEntered += CardEnteredZone;
        GrabbableArea.AreaExited += CardExitedZone;
        if (!isAesthetic)
        {

            MouseEntered += Hovered;
            MouseExited += Unhovered;
            GrabbableArea.MouseEntered += Hovered;
            GrabbableArea.MouseExited += Unhovered;

        }

        OGMask = GrabbableArea.CollisionMask;
        SetDrawn(isAesthetic);
        name = data.Type.ToString() + Name;
    }

    
    public void SetDrawn(bool isDrawn)
    {
        GrabbableArea.CollisionMask = isDrawn ? OGMask : 0;
        //GrabbableSprite.SetCollisionMaskValue(20, true);
        GrabbableArea.SetCollisionLayerValue(17, isDrawn);

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
                    backing.Scale = backing.Scale.Lerp(new Vector2(1.33f, 1.33f), 0.25f);

                }
                else
                {
                    backing.Scale = backing.Scale.Lerp(new Vector2(1.25f, 1.25f), 0.25f);

                }
            }
            else
            {
                backing.Scale = backing.Scale.Lerp(new Vector2(0.75f, 0.75f), 0.25f);

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
                
                    var spaceState = GetWorld2D().DirectSpaceState;
                    // use global coordinates, not local to node
                    var query = PhysicsRayQueryParameters2D.Create(lastMousePos, GetGlobalMousePosition(), 65536);
                    var result = spaceState.IntersectRay(query);

                    if (result.Count == 0)
                    {
                        hovered = false;
                    }
                grabbed = false;
            }




            lastMousePos = GetGlobalMousePosition();
        }
        else
        {
            backing.Scale = backing.Scale.Lerp(new Vector2(1, 1), 0.25f);

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
/*
        if (hovered && !Input.IsMouseButtonPressed(MouseButton.Left))
        {
            hovered = false;
        }*/

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
        ZIndex = 500;

        hovered = true;
    }

    void Unhovered()
    {
        
        if (Input.IsMouseButtonPressed(MouseButton.Left))
            return;
        RemoveFromGroup("DraggableHovered");

        ZIndex = 0;


        hovered = false;
    }

    public override Variant _GetDragData(Vector2 atPosition)
    {
        return base._GetDragData(atPosition);
    }

    void CardEnteredZone(Node2D node)
    {
        
        if (Data.aesthetic)
        {
            if(node.Name == "DiscardSlotArea")
            {
                DeckManager.Instance.DiscardCard(this);
            }
        }
        else
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
        usable = (Data.buyable || Data.playable || Data.sellable);
            
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
        usable = (Data.buyable || Data.playable || Data.sellable);
    }


    void ReadyToDiscard()
    {
        if (!Data.inShop)
            return;
        Data.buyable = true;
    }

    void UnreadyToDiscard()
    {
        if (!Data.inShop)
            return;
        Data.buyable = false;

    }

    void ReadyToPlay()
    {
        if (!grabbed || Data.inShop)
            return;
        Data.playable = true;

    }

    void UnreadyToPlay()
    {
        if (Data.inShop)
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

    public override string ToString()
    {

        return Data.ToString() ;
    }


}

