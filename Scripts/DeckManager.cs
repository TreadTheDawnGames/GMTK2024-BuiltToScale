using Godot;
using System;
using System.Collections;
using System.Collections.Generic;
using Medallion;
using System.Linq;

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

	public static DeckManager Instance { get; private set; }

	public bool cardHeld = false;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		DrawPile = GetNode<TextureButton>("TextureProgressBar/DrawPile");
		DrawPile.Pressed += DrawCard;

		cardCountBar = GetNode<TextureProgressBar>("TextureProgressBar");

		if(Instance == null)
		{
			Instance = this;
		}

		for (int i = 0; i < 10; i++)
		{
			deck.Enqueue(new CardData(GetRandType(), "Card" + i));
			GD.Print(i + " cards in the deck");


		}

		cardCountBar.MaxValue = deck.Count;
		cardCountBar.Value = deck.Count;

		playArea = GetNode<Area2D>("PlayArea");
		

		discardArea = GetNode<Area2D>("DiscardPile");

		cardSlots = GetNode("Hand").GetChildren().OfType<CardSlot>().ToArray();

	}

	string GetRandType()
	{
		int rand = 0;//(int)GD.RandRange(0, 2);

        GD.Print("Rand: " + rand);
        string originPath = "res://Scenes/PhysicsCardObjects/";
        switch (rand)
        {
            case 0:
                originPath += "crate.tscn";
                break;
            case 1:
				originPath += "NextNode";
				break;
            case 2:
                originPath += "AnotherNode";
                break;

            default:
                originPath += "crate.tscn";
                GD.Print("GOT BAD NUMBER");
                break;

        }

		return originPath;
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
			return;

		

		attempt.occupied = true;

		card.Slot = attempt;

		card.OGPosition = attempt.Position;

		SpawnCard(card);
		hand.Add(card);
		cardCountBar.Value = deck.Count;

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


						GameManager.Instance.TriggerCard(card.Data.PathToPhysObj);
						DiscardCard(card);
					}
				}
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
