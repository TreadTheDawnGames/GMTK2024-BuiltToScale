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

	Sprite2D discardSprite;
	Godot.RichTextLabel discardMoneyLabel;

	public bool atShop = false;

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

		discardArea = GetNode<Area2D>("DiscardPile");
		discardSprite = GetNode<Sprite2D>("DiscardPile/Sprite/Symbol");
		discardMoneyLabel = (Godot.RichTextLabel)GetNode<Godot.RichTextLabel>("DiscardPile/Sprite/DiscardMoneyText");

        cardCountBar = GetNode<TextureProgressBar>("TextureProgressBar");

		discard = CardAssembler./*TestDeck();//*/Starter();
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

        discardSprite.GetParent<Sprite2D>().Hide();


    }


    void UpdateDiscardSprite(Card card)
	{
		if (discard.Count == 0)
		{
			discardSprite.GetParent<Sprite2D>().Hide();
		}
		else
		{

            discardSprite.GetParent<Sprite2D>().Show();
			discardSprite.Texture = card.Data.symbol;
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

		if(!card.Data.inShop && !card.Data.playable)
		{
			return;
		}
		
		if(discard.Count + hand.Count + deck.Count >= 20)
		{
			GD.Print("Deck Full!");

			return;
			//shop.hide
			//prompt sell
		}

		if (card.Data.inShop)
		{
			GD.Print("Bought card: " + card.Name);
			if (!GameManager.Instance.UpdateMoney(-card.Data.cost))
			{
				return;
			}
		}


		card.Data.Slot.occupied = false;
		hand.Remove(card.Data);
		discard.Add(card.Data);
		card.QueueFree();

		if(!atShop) 
		{ 
			DrawCard();
		}
			UpdateDiscardSprite(card);
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
        if (discardArea.HasOverlappingAreas())
        {
            foreach (var area in discardArea.GetOverlappingAreas())
            {
                Card card = (Card)area.Owner;

                if (card.Data.discardable)
                {

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



    }
	
	void OpenShop()
        {
            var packedScene = GD.Load<PackedScene>("res://Scenes/PhysicsCardObjects/Shop/Shop.tscn");
            var shop = packedScene.Instantiate();
            DeckManager.Instance.AddChild(shop);
			DeckManager.Instance.MoveChild(shop, 3);
        }
}
