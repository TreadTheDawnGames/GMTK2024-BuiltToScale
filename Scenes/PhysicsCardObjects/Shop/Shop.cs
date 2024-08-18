using Godot;
using System;
using System.Linq;
using static CardAssembler;

public partial class Shop : TextureRect
{
    PermShopSlot[] permCardSlots = new PermShopSlot[3];
	RandShopSlot[] randCardSlots = new RandShopSlot[4];

	CollisionShape2D playArea;
	Button closeButton;

	Area2D sellArea;

	int cardsBought = 0;
	
	//Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		playArea = GetNode<CollisionShape2D>(GetParent().GetPath()+"/PlayArea/CollisionShape2D");
		permCardSlots = GetChildren().OfType<PermShopSlot>().ToArray();
		randCardSlots = GetChildren().OfType<RandShopSlot>().ToArray();
		closeButton = GetNode<Button>("Button");

		cardsBought = 0;
		sellArea = GetNode<Area2D>("SellPanelVisual/SellPanel");

		closeButton.Pressed += CloseShop;

		playArea.SetDeferred("disabled", true) ;

		foreach(var slot in permCardSlots)
		{
			GD.Print(slot.GetIndex());
		}

		CreateGuaranteedCard((GuaranteedCardType)0, (CardSlot)permCardSlots[0]);
        CreateGuaranteedCard((GuaranteedCardType)1, (CardSlot)permCardSlots[1]);
        CreateGuaranteedCard((GuaranteedCardType)2, (CardSlot)permCardSlots[2]);
		
		foreach(var slot in randCardSlots)
		{
			CreateCard(CardAssembler.GetWeightedRand(), slot);
		}

		DeckManager.Instance.atShop = true;

    }

	void CreateCard(CardAssembler.CardType type, CardSlot slot)
	{
		CardData data = CardAssembler.Create(type);
		data.Slot = slot;
		data.OGPosition = data.Slot.Position;
		Card card = DeckManager.Instance.SpawnCard(data, data.Slot.GlobalPosition);
		card.SetDrawn(true);
	}
	
	void CreateGuaranteedCard(CardAssembler.GuaranteedCardType type, CardSlot slot)
	{
		CardData data = CardAssembler.Create(type);
		data.Slot = slot;
		data.OGPosition = data.Slot.Position;
		Card card = DeckManager.Instance.SpawnCard(data, data.Slot.GlobalPosition);
		card.SetDrawn(true);
	}

    public void CloseShop()
    {
        playArea.SetDeferred("disabled", false);

		DeckManager.Instance.ReplenishHand();
        DeckManager.Instance.atShop = false;


        CallDeferred("queue_free");
    }

    bool CheckForSell()
    {
		

        if (sellArea.HasOverlappingAreas())
        {
            foreach (var area in sellArea.GetOverlappingAreas())
            {
                Card card = (Card)area.Owner;

				if (card.Data.Type == CardType.shop)
				{
					int shopCount = 0;
					foreach(CardData data in DeckManager.Instance.AllCards)
					{
						if(data.Type == CardType.shop)
						{
							shopCount++;
						}
					}
					if (shopCount < 2) 
					{
						return false;
					}
					
				}

                if (card.Data.sellable)
                {
                    if (!Input.IsMouseButtonPressed(MouseButton.Left))
                    {
						if (GameManager.Instance.UpdateMoney((int)(card.Data.cost * 0.22f)))
                        {
							DeckManager.Instance.SellCard(card);
                            return true;
                        }
                    }
                }
            }
        }
        return false;
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
		if(CheckForSell()) { return; }
    }
}
