using Godot;
using Medallion;
using System;
using System.Collections.Generic;


public partial class CardDisplay : Control
{

	PackedScene displayPanel = GD.Load<PackedScene>("res://Scenes/Cards/display_card_panel.tscn");
	TextureRect displayBackground;
	AnimationPlayer animator;
	DisplaySlot[] displaySlots;

    bool showing = false;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		displayBackground = GetNode<TextureRect>("TextureRect");
		animator = GetNode<AnimationPlayer>("AnimationPlayer");


		animator.Play("RESET");


    }

    List<DisplaySlot> CreatePanels(List<CardData> cards)
	{
		List<DisplaySlot> cardPanels = new();
        var goToPosition = new Vector2(75, 150);
        for (int i = 0; i < DeckManager.Instance.deckSize; i++)
        {
            var panel = displayPanel.Instantiate<Control>();
            panel.Position = goToPosition;
            goToPosition.X += panel.Size.X * 1.5f * 0.75f;
            if (goToPosition.X > 1500)
            {
                goToPosition.X = 75;
                goToPosition.Y += panel.Size.Y * 1.25f * 0.75f;
            }
            cardPanels.Add(panel.GetNode<DisplaySlot>("DisplaySlot"));

            displayBackground.AddChild(panel);

        }
		return cardPanels;
    }

	bool firstRun = true;
	public void DisplayDeck(List<CardData> cards)
	{
		if (cards == null) return;

		if (firstRun)
		{
			displaySlots = CreatePanels(cards).ToArray();
			firstRun = false;
		}

		foreach (DisplaySlot slot in displaySlots)
		{
			foreach (var child in slot.GetChildren())
			{
				child.QueueFree();
			}
		}
		cards.Sort();

		int slotNum = 0;
		GD.Print(cards.Count);
		foreach (var cardData in cards)
		{
			cardData.Slot = displaySlots[slotNum];
			cardData.OGPosition = cardData.Slot.Position;
			Card card = DeckManager.Instance.SpawnCard(cardData, displaySlots[slotNum].GlobalPosition);
			card.Reparent(cardData.Slot);
			slotNum++;
		}
	}

	public void TogglePanelVisibility(List<CardData> cards)
	{
		showing = !showing;
		PopOut(showing, cards);
	}

        public void PopOut(bool isIn, List<CardData> cards)
        {

            if (animator.IsPlaying())
            {

                animator.SpeedScale *= -1;
            }
            else
            {
                animator.SpeedScale = 1;
			DisplayDeck(cards);

                if (isIn)
                {
                    animator.PlayBackwards("Out");

                }
                else
                {
                    animator.Play("Out");

                }
            }
        }


	

}
