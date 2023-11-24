using Godot;
using System;
using System.Linq;
using System.Net.Security;

public partial class Main : Node2D
{
	[Export]
	public PackedScene CardScene { get; set; }

	private Card[] cards = Array.Empty<Card>();
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		GenerateDeck();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	private void GenerateDeck()
	{
		// Generate Cards for all values (1-12) and suits (club, diamond, heart, spade)
		foreach (Card.Suits suit in Enum.GetValues(typeof(Card.Suits)))
		{
			for (int val=1; val<13; val++)
			{
				var card = CardScene.Instantiate<Card>();
				card.Init(suit,val);
				cards.Append(card);
				AddChild(card);
				card.Position = new Vector2(val*45,(int)suit*80);
			}
		}
	}
}
