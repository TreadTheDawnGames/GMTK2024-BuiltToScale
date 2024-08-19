using Godot;
using System;
using System.Collections;
using System.Collections.Generic;
using Medallion;
using System.Linq;
using System.IO;

public partial class DeckManager : Control
{

	PackedScene CardScene = GD.Load<PackedScene>("res://Scenes/Cards/card.tscn");

	Queue<CardData> deck = new Queue<CardData>();
	List<CardData> discard = new List<CardData>();

	List<CardData> hand = new List<CardData>();

	Area2D playArea, discardArea;

	CardSlot[] cardSlots = new CardSlot[5];

	TextureProgressBar cardCountBar;

	AudioStreamPlayer shuffleSound;
	AudioStreamPlayer flipSound;

	Sprite2D discardSpriteSymbol;
	Sprite2D discardSpriteBacking;
    Godot.RichTextLabel discardMoneyLabel;

	RichTextLabel buyForAmount;
	Node2D buyForNode;
	Sprite2D deckFullWarning;

	public bool atShop = false;

	public RichTextLabel scoreLabel;
	public RichTextLabel highScoreLabel;

	Sprite2D notEnoughCashWarning;



	[Export]
	public int deckSize { get; private set; } = 20;

	public List<CardData> AllCards
	{
		get
		{
			List<CardData> cards = new();

			foreach (CardData card in discard)
			{
				cards.Add(card);
			}
			foreach (CardData card in hand)
			{
				cards.Add(card);
			}
			foreach (CardData card in deck)
			{
				cards.Add(card);
			}
			return cards;
		}
	}

	private static DeckManager instance = null;

	private DeckManager()
	{
	}

	public static DeckManager Instance
	{
		get
		{
			if (instance == null)
			{
				instance = new DeckManager();
			}
			return instance;
		}
	}
	public bool cardHeld = false;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		instance = this;
		cardSlots = GetNode("Hand").GetChildren().OfType<CardSlot>().ToArray();
		playArea = GetNode<Area2D>("PlayArea");
		shuffleSound = GetNode<AudioStreamPlayer>("AudioStreamPlayer");
		flipSound = GetNode<AudioStreamPlayer>("CardFlipSound");

		scoreLabel = GetNode<RichTextLabel>("Scores/ScoreLabel");
		highScoreLabel = GetNode<RichTextLabel>("Scores/HighScoreLabel");


		discardArea = GetNode<Area2D>("DiscardPile");
		discardSpriteSymbol = GetNode<Sprite2D>("DiscardPile/Backing/Symbol");
		discardSpriteBacking = GetNode<Sprite2D>("DiscardPile/Backing");
		discardMoneyLabel = (Godot.RichTextLabel)GetNode<Godot.RichTextLabel>("DiscardPile/Backing/DiscardMoneyText");
		buyForNode = GetNode<Node2D>("DiscardPile/BuyForNode");
		buyForAmount = GetNode<RichTextLabel>("DiscardPile/BuyForNode/BuyForAmount");
		deckFullWarning = GetNode<Sprite2D>("DiscardPile/DeckFullWarning");
		notEnoughCashWarning = GetNode<Sprite2D>("DiscardPile/NotEnoughCashWarning");

		deckFullWarning.Hide();
		notEnoughCashWarning.Hide();

		cardCountBar = GetNode<TextureProgressBar>("TextureProgressBar");

		SetupDeck(CardAssembler.ShopTestDeck(5));

		discardSpriteBacking.Hide();


	}


	void UpdateDiscardSprite(Card card)
	{
		if (discard.Count == 0)
		{
            discardSpriteBacking.Hide();
		}
		else
		{
            //if(!card.Data.singleUse)
            discardSpriteBacking.Show();

			discardSpriteBacking.Texture = card.backing.Texture;
			discardSpriteSymbol.Texture = card.symbol.Texture;
			
			discardMoneyLabel.Text = card.Data.cost.ToString();
		}
	}

	public void DrawCard()
	{


		CardSlot attempt;
		int i = 0;
		do
		{


			if (i > cardSlots.Length - 1)
			{
				GD.Print("Hand Full!");
				return;
			}
			attempt = cardSlots[i++];



		}
		while (attempt.occupied);



		CardData card = null;
		try
		{
			card = deck.Dequeue();



		}
		catch
		{
			if (discard.Count > 0)
			{

				Shuffle();
				DrawCard();
			}
		}

		if (card == null)
		{
			GD.Print("CardData was null");
			return;
		}



		attempt.occupied = true;

		card.Slot = attempt;

		card.OGPosition = attempt.Position;

		SpawnCard(card, cardCountBar.GlobalPosition);
		hand.Add(card);
		cardCountBar.Value = deck.Count;

		PlayFlipSound();

	}

	void PlayFlipSound()
	{
		int rand = (int)(GD.Randi() % 2);
		flipSound.Stream = GD.Load<AudioStream>("res://Assets/Sounds/Cards/FlipSounds/CardFlip" + rand + ".wav");



		flipSound.Play();
	}

	public void SellCard(Card card)
	{
		card.Data.Slot.occupied = false;
		hand.Remove(card.Data);
		card.QueueFree();

	}
	void DiscardCard(Card card)
	{

		if (!card.Data.inShop && !card.Data.playable)
		{
			return;
		}

		if (discard.Count + hand.Count + deck.Count >= 20 && !card.Data.playable)
		{
			GD.Print("Deck Full!");

			return;
		}

		if (card.Data.inShop)
		{
			if (!GameManager.Instance.CanBuy(card.Data.cost))
			{
				GD.Print("Insufficient funds: " + card.Name);

				return;
			}
			else
			{

				GameManager.Instance.UpdateMoney(-card.Data.cost);

			}
		}
		GD.Print("Bought card: " + card.Name);


		PlayFlipSound();
		card.Data.Slot.occupied = false;
		hand.Remove(card.Data);


		if (!atShop)
		{
			DrawCard();
		}

		if (card.Data.singleUse && !card.Data.buyable)
		{
			FillWithBeachBalls();
			card.QueueFree();
			return;
		}

		discard.Add(card.Data);


		UpdateDiscardSprite(card);


		card.QueueFree();


	} 
	
	void Shuffle()
	{
		GD.Print("SHUFFLED!");
		shuffleSound.Play();
		foreach(CardData card in discard.Shuffled().ToList<CardData>())
		{
			deck.Enqueue(card);
		}
		cardCountBar.MaxValue = deck.Count;
        cardCountBar.Value = deck.Count;

        discard.Clear();

	}

	public Card SpawnCard(CardData data, Vector2 spawnPosition)
	{
		var spawnedCard = CardScene.Instantiate<Card>();

        data.Slot.GetParent().AddChild(spawnedCard);

        spawnedCard.GlobalPosition = spawnPosition;
		
		spawnedCard.SetUp(data);
		return spawnedCard;
    }

	bool CheckForDiscard()
	{
        if (buyForNode.Visible)
        {
            buyForNode.Hide();
        }
        if (deckFullWarning.Visible)
        {
            deckFullWarning.Hide();
        }
        if (notEnoughCashWarning.Visible)
        {
            notEnoughCashWarning.Hide();
        }

        if (discardArea.HasOverlappingAreas())
        {
            foreach (var area in discardArea.GetOverlappingAreas())
            {
                Card card = (Card)area.Owner;
                if (card.Data.buyable)
                {
                    if (AllCards.Count >= deckSize && !card.Data.playable)
                    {
						deckFullWarning.Show();
					}
					else
					{
						if (!GameManager.Instance.CanBuy(card.Data.cost))
						{
							notEnoughCashWarning.Show();
						}
						else
						{

							if (deckFullWarning.Visible)
							{
								deckFullWarning.Hide();
							}if (notEnoughCashWarning.Visible)
							{
                                notEnoughCashWarning.Hide();
							}
							buyForNode.Show();
							buyForAmount.Text = card.Data.cost.ToString(); ;
						}

					}
                    


                    if (!Input.IsMouseButtonPressed(MouseButton.Left))
                    {
                        DiscardCard(card);
						return true;
                    }
                }
            }
        }
       
        return false;
    }

	bool CheckForPlay()
	{
		if (playArea.HasOverlappingAreas())
		{
			foreach (var area in playArea.GetOverlappingAreas())
			{

				Card card = (Card)area.Owner;
				if (card.Data.playable)
				{

                    if (!Input.IsMouseButtonPressed(MouseButton.Left))
					{
						if (GameManager.Instance.TriggerCard(card.Data.PathToPhysObj))
						{
							DiscardCard(card);
							return true;
						}
					}
				}
			}
		}
		return false;
	}

	public void ReplenishHand()
	{

        for (int i = 0; i < cardSlots.GetLength(0); i++)
        {
            DrawCard();
        }
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
	{
		if(CheckForDiscard()) return;
		if(CheckForPlay()) return;

		if (Input.IsActionJustPressed("Debug-PrintDeckData"))
		{
			foreach(CardData card in AllCards)
			{
				GD.Print(card.Type);
			}
		}

    }
	
	void OpenShop()
        {
            var packedScene = GD.Load<PackedScene>("res://Scenes/PhysicsCardObjects/Shop/Shop.tscn");
            var shop = packedScene.Instantiate();
            DeckManager.Instance.AddChild(shop);
			DeckManager.Instance.MoveChild(shop, 3);
        }

	void SetupDeck(List<CardData> setupDeck)
	{

		

		deck.Clear();
		hand.Clear();
		discard.Clear();


        discard = setupDeck;
        Shuffle();

        foreach (CardData card in discard)
        {
            deck.Enqueue(card);
        }


        for (int i = 0; i < cardSlots.GetLength(0); i++)
        {
            DrawCard();
        }

        cardCountBar.MaxValue = deck.Count;
        cardCountBar.Value = deck.Count;
    }

	public void FillWithBeachBalls()
	{
		while (AllCards.Count < deckSize)
		{
			discard.Add(new CardData(CardAssembler.MakeCardPath(CardAssembler.CardType.beachball)));
		}
	}

}
