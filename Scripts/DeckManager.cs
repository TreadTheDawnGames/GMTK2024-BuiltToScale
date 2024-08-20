using Godot;
using System;
using System.Collections;
using System.Collections.Generic;
using Medallion;
using System.Linq;
using System.IO;
using System.Diagnostics;

public partial class DeckManager : Control
{

	PackedScene CardScene = GD.Load<PackedScene>("res://Scenes/Cards/card.tscn");

	Queue<CardData> deck = new Queue<CardData>();
	List<CardData> discard = new List<CardData>();

	List<CardData> hand = new List<CardData>();

	Area2D playArea, discardArea;

	HandSlot[] cardSlots = new HandSlot[5];

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
	DiscardSlot discardSlot;

	TextureButton cardsInDeckButton;
	CardDisplay cardDisplay;
	bool cardDisplayActivated = false;


	[Export]
	public int deckSize { get; private set; } = 20;

	bool displayIsOpen = false;

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

	public List<CardData> AllCardsVirtual
	{
		get
		{
			List<CardData> cards = new();

			foreach (CardData card in discard)
			{
				cards.Add(card.Copy());
			}
			foreach (CardData card in hand)
			{
				cards.Add(card.Copy());
			}
			foreach (CardData card in deck)
			{
				cards.Add(card.Copy());
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
		cardSlots = GetNode("Hand").GetChildren().OfType<HandSlot>().ToArray<HandSlot>();
		playArea = GetNode<Area2D>("PlayArea");
		shuffleSound = GetNode<AudioStreamPlayer>("ShuffleSounds");
		flipSound = GetNode<AudioStreamPlayer>("CardFlipSound");

		scoreLabel = GetNode<RichTextLabel>("Scores/ScoreLabel");
		highScoreLabel = GetNode<RichTextLabel>("Scores/HighScoreLabel");

		cardsInDeckButton = GetNode<TextureButton>("TextureProgressBar/CardsInDeckButton");

		discardArea = GetNode<Area2D>("DiscardPile");
		discardSpriteSymbol = GetNode<Sprite2D>("DiscardPile/Backing/Symbol");
		discardSpriteBacking = GetNode<Sprite2D>("DiscardPile/Backing");
		discardMoneyLabel = (Godot.RichTextLabel)GetNode<Godot.RichTextLabel>("DiscardPile/Backing/DiscardMoneyText");
		discardSlot = GetNode<DiscardSlot>("DiscardPile/DiscardSlot");
		buyForNode = GetNode<Node2D>("DiscardPile/BuyForNode");
		buyForAmount = GetNode<RichTextLabel>("DiscardPile/BuyForNode/BuyForAmount");
		deckFullWarning = GetNode<Sprite2D>("DiscardPile/DeckFullWarning");
		notEnoughCashWarning = GetNode<Sprite2D>("DiscardPile/NotEnoughCashWarning");


		cardDisplay = GetNodeOrNull<CardDisplay>("CardDisplay");

		deckFullWarning.Hide();
		notEnoughCashWarning.Hide();

		cardCountBar = GetNode<TextureProgressBar>("TextureProgressBar");

		//SetupDeck(CardAssembler.BalancedStarter(deckSize));
		SetupDeck(CardAssembler.OneEach()); 
		//SetupDeck(CardAssembler.Artistic()); 

		discardSpriteBacking.Hide();

        cardsInDeckButton.Pressed += DoDisplay;

		//DisplayDeck(AllCards);


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


		HandSlot attemptedSlot;
		int i = 0;
		do
		{

			if (i > cardSlots.Length - 1)
			{
				GD.Print("Hand Full!");
				return;
			}
			attemptedSlot = (HandSlot)cardSlots[i];
			i++;

		}
		while (attemptedSlot.occupied);



		CardData cardData = null;
		try
		{
			cardData = deck.Dequeue();



		}
		catch
		{
			if (discard.Count > 0)
			{

				Shuffle();
				DrawCard();
			}
		}


        if (cardData == null)
		{
			GD.Print("CardData was null");
			return;
		}
     




		attemptedSlot.SetOccupied(true);



		cardData.Slot = (HandSlot)attemptedSlot;

		cardData.OGPosition = attemptedSlot.Position;

		Card card = SpawnCard(cardData, cardCountBar.GlobalPosition);
        card.SetDrawn(true);
        card.Data.inShop = false;

        hand.Add(cardData);
		cardCountBar.Value = deck.Count;

		PlayFlipSound();

	}

	void PlayFlipSound()
	{
		int rand = (int)(GD.Randi() % 3);
		flipSound.Stream = GD.Load<AudioStream>("res://Assets/Sounds/Cards/FlipSounds/CardFlip" + rand + ".wav");

		flipSound.Play();
	}
	void PlayShuffleSound()
	{
		int rand = (int)(GD.Randi() % 3);
		shuffleSound.Stream = GD.Load<AudioStream>("res://Assets/Sounds/Shuffle" + rand + ".wav");

        shuffleSound.Play();
	}

	public void RemoveCardFromDeck(Card card)
	{
		card.Data.Slot.SetOccupied(false);
		hand.Remove(card.Data);
		card.QueueFree();

	}
	public void DiscardCard(Card card)
	{

		if (!card.Data.inShop && !card.Data.playable)
		{
			return;
		}

		if (discard.Count + hand.Count + deck.Count >= deckSize && !card.Data.playable)
		{
			GD.Print("Deck Full!");

			return;
		}

		if (!card.Data.aesthetic)
		{

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
					GD.Print("Bought card: " + card.name);

				}
			}
		}


		PlayFlipSound();

		card.Data.Slot.SetOccupied(false);
		hand.Remove(card.Data);


		if (!atShop)
		{
			ReplenishHand();
		}


		if (card.Data.singleUse && !card.Data.buyable)
		{
			AddCardToDeckFromBank();
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
		PlayShuffleSound();
		foreach (CardData card in discard.Shuffled().ToList<CardData>())
		{
			deck.Enqueue(card);
		}
		cardCountBar.MaxValue = deck.Count;
		cardCountBar.Value = deck.Count;

		discard.Clear();

	}

	public Card SpawnCard(CardData data, Vector2 spawnPosition, bool isAesthetic = false)
	{
		if (data == null) return null;

		var spawnedCard = CardScene.Instantiate<Card>();

		data.Slot.GetParent().AddChild(spawnedCard);

		spawnedCard.GlobalPosition = spawnPosition;
		spawnedCard.SetUp(data, isAesthetic);
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
				if (card.Data.buyable && card.grabbed)
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
							}
							if (notEnoughCashWarning.Visible)
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
				if (card.Data.playable && card.grabbed)
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
		if (CheckForDiscard()) return;
		if (CheckForPlay()) return;

		//if(mouse is on ege of screen)
		//DisplayCards();



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



		SetupShuffle(setupDeck);





		ReplenishHand();

		cardCountBar.MaxValue = deck.Count;
		cardCountBar.Value = deck.Count;
	}

	void SetupShuffle(List<CardData> setupDeck)
	{
		var sneakyDeck = setupDeck;
		CardData shop;
		List<CardData> startingHand = new List<CardData>();

		foreach (CardData card in setupDeck)
		{
			if (card.Type == CardAssembler.CardType.shop)
			{
				shop = card;
				sneakyDeck.Remove(card);
				startingHand.Add(card);
				break;
			}
		}

		sneakyDeck.Shuffle();

		for (int i = 0; i < 4; i++)
		{
			if (i < sneakyDeck.Count)
			{

			startingHand.Add(sneakyDeck[i]);
			sneakyDeck.Remove(sneakyDeck[i]);
			}
		}

		startingHand.Shuffle();


		PlayShuffleSound();
		foreach (CardData card in startingHand)
		{
			deck.Enqueue(card);
		}

		foreach (CardData card in sneakyDeck)
		{
			deck.Enqueue(card);
		}
		cardCountBar.MaxValue = deck.Count;
		cardCountBar.Value = deck.Count;

	}

	public void AddCardToDeckFromBank(CardAssembler.CardType type = CardAssembler.CardType.beachball)
	{

		var card = new CardData(CardAssembler.MakeCardPath(type));

		card.Slot = discardSlot;

		SpawnCard(card, new Vector2(960 + GD.RandRange(-400, 400), 500), true);
		//discard.Add(card);


	}

	public void RefillDeckWith(CardAssembler.CardType type = CardAssembler.CardType.beachball)
	{
		int cardsToDraw = deckSize - AllCards.Count;
		for (int i = 0; i < cardsToDraw; i++)
		{
			AddCardToDeckFromBank(type);
		}
	}

	void DisplayDeck(List<CardData> cards)
	{

		cardDisplay.TogglePanelVisibility(cards);






	}


	

	void DoDisplay()
	{
		if (cardDisplayActivated)
		{
			DisplayDeck(null);
			cardDisplayActivated = false;
		}
		else
		{
			if (!cardDisplayActivated)
			{
				DisplayDeck(AllCardsVirtual);
				cardDisplayActivated = true;
			}
		}

	}
}
