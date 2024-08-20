using Godot;
using System;
using System.ComponentModel;

public partial class DrawPileVisual : Area2D
{
	TextureProgressBar drawPileGraphic;
	TextureButton cardsInDeckButton;

	bool hovered = false;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		drawPileGraphic = GetParent<TextureProgressBar>();
		cardsInDeckButton = drawPileGraphic.GetNode<TextureButton>("CardsInDeckButton");
		cardsInDeckButton.MouseEntered += () => hovered = true;
		cardsInDeckButton.MouseExited += () => hovered = false;
	}


	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (HasOverlappingBodies())
		{
			var graphic = drawPileGraphic.Modulate;
				graphic.A = Mathf.Lerp(drawPileGraphic.Modulate.A, 0.15f, 0.5f);
			drawPileGraphic.Modulate = graphic;
		}
		else
		{
            var graphic = drawPileGraphic.Modulate;
            graphic.A = Mathf.Lerp(drawPileGraphic.Modulate.A, 1f, 0.25f);
            drawPileGraphic.Modulate = graphic;
        }
        if (hovered)
        {
			if (Input.IsMouseButtonPressed(MouseButton.Left))
            {
                drawPileGraphic.Scale = drawPileGraphic.Scale.Lerp(new Vector2(0.90f, 0.90f), 0.25f);

            }
            else
            {
	            drawPileGraphic.Scale = drawPileGraphic.Scale.Lerp(new Vector2(1.15f, 1.15f), 0.25f);

            }
        }
        else
        {
            drawPileGraphic.Scale = drawPileGraphic.Scale.Lerp(new Vector2(1, 1), 0.25f);
            
        }
    }
}
