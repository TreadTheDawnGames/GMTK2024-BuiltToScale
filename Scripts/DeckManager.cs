using Godot;
using System;
using System.Collections;
using System.Collections.Generic;
using Medallion;
using System.Linq;
using System.IO;

public partial class DeckManager : Control
{
	
	TextureButton DrawPile;
	PackedScene CardScene = GD.Load<PackedScene>("res://Scenes/Cards/card.tscn");

	Queue<CardData> deck = new Queue<CardData>();
	List<CardData> discard = new List<CardData>();

	List<CardData> hand = new List<CardData>();

	Area2D playArea, discardArea;

	CardSlot[] cardSlots = new CardSlot[5];

	TextureProgressBar cardCountBar;

	AudioStreamPlayer shuffleSound;
	AudioStreamPlayer flipSound;

	public static DeckManager Instance { get; private set; }

	public bool cardHeld = false;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		DrawPile = GetNode<TextureButton>("TextureProgressBar/DrawPile");
		DrawPile.Pressed += DrawCard;
		cardSlots = GetNode("Hand").GetChildren().OfType<CardSlot>().ToArray();
        playArea = GetNode<Area2D>("PlayArea");
		shuffleSound = GetNode<AudioStreamPlayer>("AudioStreamPlayer");
		flipSound = GetNode<AudioStreamPlayer>("CardFlipSound");

        discardArea = GetNode<Area2D>("DiscardPile");

        cardCountBar = GetNode<TextureProgressBar>("TextureProgressBar");

		if(Instance == null)
		{
			Instance = this;
		}

		for (int i = 0; i < 10; i++)
		{
			deck.Enqueue(CardAssembler.Rand());
		}


		GD.Print(deck.Count  +" cards in deck");
		for (int i = 0; i < cardSlots.GetLength(0); i++)
		{
			GD.Print(cardSlots.GetLength(0));
			DrawCard();
		}

		cardCountBar.MaxValue = deck.Count;
		cardCountBar.Value = deck.Count;



	}

	


	void DrawCard()
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

		SpawnCard(card);
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


	void DiscardCard(Card card)
	{
		card.Data.Slot.occupied = false;
		hand.Remove(card.Data);
		discard.Add(card.Data);
		card.QueueFree();
		DrawCard();
	}
	
	void Shuffle()
	{
		GD.Print("SHUFFLED!");
		shuffleSound.Play();
		foreach(CardData card in discard.Shuffled().ToList<CardData>())
		{
			deck.Enqueue(card);
			GD.Print(card.ToString());
		}
		cardCountBar.MaxValue = deck.Count;
        cardCountBar.Value = deck.Count;

        discard.Clear();

	}

	void SpawnCard(CardData data)
	{
		var spawnedCard = CardScene.Instantiate<Card>();

        data.Slot.GetParent().AddChild(spawnedCard);

        spawnedCard.GlobalPosition = DrawPile.GlobalPosition;
        
		
		spawnedCard.SetUp(data);

    }

	bool CheckForDiscard()
	{
        if (discardArea.HasOverlappingAreas())
        {
            foreach (var area in discardArea.GetOverlappingAreas())
            {
                GD.Print("Got card");

                Card card = (Card)area.Owner;

                if (card.Data.discardable)
                {
                    GD.Print("Discardable");
                    if (!Input.IsMouseButtonPressed(MouseButton.Left))
                    {
                        GD.Print("Discarded");
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
				GD.Print("Got card");

				Card card = (Card)area.Owner;
				if (card.Data.playable)
				{
					GD.Print("Playable");
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

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
	{
		if(CheckForDiscard()) return;
		if(CheckForPlay()) return;

	}
}
