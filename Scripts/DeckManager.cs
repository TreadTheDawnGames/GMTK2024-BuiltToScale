using Godot;
using System;
using System.Collections;
using System.Collections.Generic;
using Medallion;
using System.Linq;

public partial class DeckManager : Control
{
	
	TextureButton DrawPile;
	Button ShuffleButton;
	PackedScene CardScene = GD.Load<PackedScene>("res://Scenes/Cards/card.tscn");

	Queue<CardData> deck = new Queue<CardData>();
	List<CardData> discard = new List<CardData>();

	List<CardData> hand = new List<CardData>();

	Area2D playArea, discardArea;

	CardSlot[] cardSlots = new CardSlot[5];

	public static DeckManager Instance { get; private set; }

	public bool cardHeld = false;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		DrawPile = GetNode<TextureButton>("DrawPile");
		DrawPile.Pressed += DrawCard;
		ShuffleButton = GetNode<Button>("Button");
		ShuffleButton.Pressed += Shuffle;

		if(Instance == null)
		{
			Instance = this;
		}

		for (int i = 0; i < 5; i++)
		{
			deck.Enqueue(new CardData("This/is/a/path", "Card" + i));
			GD.Print(i + " cards in the deck");


		}

		playArea = GetNode<Area2D>("PlayArea");
		

		discardArea = GetNode<Area2D>("DiscardPile");

		cardSlots = GetNode("Hand").GetChildren().OfType<CardSlot>().ToArray();

	}

	Texture2D GetRandType()
	{
        int rand = (int)GD.RandRange(0, 2);

        GD.Print("Rand: " + rand);
        Texture2D symbol;
        switch (rand)
        {
            case 0:
                symbol = GD.Load<Texture2D>("res://Assets/Sprites/Objects/Log.png");
                break;
            case 1:
                symbol = GD.Load<Texture2D>("res://Assets/Sprites/Objects/Bowl.png");
                break;
            case 2:
                symbol = GD.Load<Texture2D>("res://Assets/Sprites/Objects/Rock.png");
                break;

            default:
                symbol = GD.Load<Texture2D>("res://Assets/Sprites/Objects/Bowl.png");
                GD.Print("GOT BAD NUMBER");
                break;

        }

		return symbol;
    }
	

	void DrawCard()
	{
		var card = deck.Dequeue();
		SpawnCard(card);
		


		hand.Add(card);
	}


	void DiscardCard(Card card)
	{
		card.Data.Slot.occupied = false;
		hand.Remove(card.Data);
		discard.Add(card.Data);
		card.QueueFree();
	}
	
	void PlayCard(CardData card)
	{
		hand.Remove(card);
		discard.Add(card);
	}

	void Shuffle()
	{

		foreach(CardData card in discard.Shuffled().ToList<CardData>())
		{
			deck.Enqueue(card);
			GD.Print(card.ToString());
		}
		discard.Clear();

	}

	void SpawnCard(CardData data)
	{
		var spawnedCard = CardScene.Instantiate<Card>();

		CardSlot attempt;
			int i = 0;
		do
		{
			
			if (i > cardSlots.Length)
			{
				GD.Print("Hand Full!");
				return;
			}
			attempt = cardSlots[i++];

			
		}
		while (attempt.occupied);

		attempt.occupied = true;

		attempt.GetParent().AddChild(spawnedCard);
		data.Slot = attempt;
		spawnedCard.Position = attempt.Position;
        spawnedCard.SetUp(data);

    }

	void CheckForDiscard()
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
                    }
                }
            }
        }
    }

	void CheckForPlay()
	{
		if (discardArea.HasOverlappingAreas())
		{
			foreach (var area in discardArea.GetOverlappingAreas())
			{
				GD.Print("Got card");

				Card card = (Card)area.Owner;

				GameManager.Instance.TriggerCard(card.Data.PathToPhysObj);
			}
		}
	}

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
	{
		CheckForDiscard();

		CheckForPlay();
	}
}
