using Godot;
using System;
using System.Linq;
using static CardAssembler;

public partial class Shop : TextureRect
{
	[Export]
	Texture2D texNormal, highlighted;
    PermShopSlot[] permCardSlots = new PermShopSlot[3];
	RandShopSlot[] randCardSlots = new RandShopSlot[4];

	CollisionShape2D playArea;
    TextureButton closeButton;

	Area2D sellArea;

	AudioStreamPlayer chaChing;

	TextureRect sellPanel;

	Node2D SellForNode;
	RichTextLabel sellAmount;


	int cardsBought = 0;
	
	//Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		playArea = GetNode<CollisionShape2D>(GetParent().GetPath()+"/PlayArea/CollisionShape2D");
		permCardSlots = GetChildren().OfType<PermShopSlot>().ToArray();
		randCardSlots = GetChildren().OfType<RandShopSlot>().ToArray();
		closeButton = GetNode<TextureButton>("Button");

		chaChing = GetNode<AudioStreamPlayer>("ChaChingSound");

		cardsBought = 0;
		sellArea = GetNode<Area2D>("SellPanelVisual/SellPanel");
		sellPanel = GetNode<TextureRect>("SellPanelVisual");
		SellForNode = GetNode<Node2D>("SellPanelVisual/SellFor");
		sellAmount = GetNode<RichTextLabel>("SellPanelVisual/SellFor/Amount");

		closeButton.Pressed += CloseShop;

		playArea.SetDeferred("disabled", true) ;

		foreach(var slot in permCardSlots)
		{
			GD.Print(slot.GetIndex());
		}

        CreateCard(CardType.obsidian, (CardSlot)permCardSlots[0]);

        //CreateGuaranteedCard((GuaranteedCardType)0, (CardSlot)permCardSlots[0]);
        CreateGuaranteedCard((GuaranteedCardType)1, (CardSlot)permCardSlots[1]);
        CreateGuaranteedCard((GuaranteedCardType)2, (CardSlot)permCardSlots[2]);
		
		foreach(var slot in randCardSlots)
		{
			CreateCard(CardAssembler.GetWeightedRand(), slot);
		}

		DeckManager.Instance.atShop = true;

    }

    void PlaySellSound()
    {
        int rand = (int)(GD.Randi() % 2);
        chaChing.Stream = GD.Load<AudioStream>("res://Assets/Sounds/ChaChing" + rand + ".wav");



		chaChing.Play();
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

		DeckManager.Instance.FillWithBeachBalls();

		GameManager.Instance.SetPauseGame(false);

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

                    sellPanel.Texture = highlighted;
					SellForNode.Show();
					sellAmount.Text = Mathf.CeilToInt(card.Data.cost * 0.22f).ToString(); ;
                    if (!Input.IsMouseButtonPressed(MouseButton.Left))
                    {
						if (GameManager.Instance.UpdateMoney(Mathf.CeilToInt(card.Data.cost * 0.22f)))
                        {
							DeckManager.Instance.SellCard(card);
							PlaySellSound();
                            return true;
                        }
                    }
                }
            }
        }
		else
		{
			if(sellPanel.Texture!= texNormal)
			{
				sellPanel.Texture = texNormal;
            }
			if(SellForNode.Visible)
			{
                SellForNode.Hide();
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
