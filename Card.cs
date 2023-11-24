using Godot;
using System;

public partial class Card : Node2D
{
	private int value { get; set; } = 1;
	private Suits suit { get; set;}

	public void Init(Suits initSuit, int initValue)
	{
		value = initValue;
		suit = initSuit;
	}

	private static readonly string[] valueDisplay = {"A","2","3","4","5","6","7","8","9","10","J","Q","K"};
	
	public enum Suits
	{
		Spade,
		Heart,
		Club,
		Diamond
	}


	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		InitializeDisplay();
	}


	private void InitializeDisplay()
	{
		var valueLabel = GetNode<Label>("ValueLabel");
		valueLabel.Text = valueDisplay[value-1].ToString();
		var suitLabel = GetNode<Label>("SuitLabel");
		switch (suit)
		{
			case Suits.Spade:
				suitLabel.Text = "♠";
				valueLabel.AddThemeColorOverride("font_color", new Color(0, 0, 0));
				suitLabel.AddThemeColorOverride("font_color", new Color(0, 0, 0));
				break;
			case Suits.Heart:
				suitLabel.Text = "♥";
				valueLabel.AddThemeColorOverride("font_color", new Color(1, 0, 0));
				suitLabel.AddThemeColorOverride("font_color", new Color(1, 0, 0));
				break;
			case Suits.Club:
				suitLabel.Text = "♣";
				valueLabel.AddThemeColorOverride("font_color", new Color(0, 0, 0));
				suitLabel.AddThemeColorOverride("font_color", new Color(0, 0, 0));
				break;
			case Suits.Diamond:
				suitLabel.Text = "♦";
				valueLabel.AddThemeColorOverride("font_color", new Color(1, 0, 0));
				suitLabel.AddThemeColorOverride("font_color", new Color(1, 0, 0));
				break;
			default:
				break;
		}
	}
}
