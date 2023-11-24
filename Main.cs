using Godot;
using System;
using System.Linq;

public partial class Main : Node2D
{
	[Export]
	public PackedScene CardScene { get; set; }

	private Card[] deck = Array.Empty<Card>();

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		GenerateDeck();
		ShuffleDeck();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	private void GenerateDeck()
	{
		var table = GetNode<Node2D>("Table");
		GD.Print("Generate the Deck");
		// Generate Cards for all values (1-13) and suits (club, diamond, heart, spade)
		foreach (Card.Suits suit in Enum.GetValues(typeof(Card.Suits)))
		{
			for (int val = 1; val < 14; val++)
			{
				var card = CardScene.Instantiate<Card>();
				card.Init(suit,val);
				deck = deck.Append(card).ToArray();
				table.AddChild(card); 
				card.Position = new Vector2(val * 45, (int) suit * 80);
			}
		}
	}

	public void ShuffleDeck()
	{
		var table = GetNode<Node2D>("Table");
		GD.Print("Shuffle the Deck");
		Card[] newDeck = Array.Empty<Card>();
		for (int i = deck.Length - 1; i > 0; i--)
		{
			var j = GD.RandRange(0,i);
			newDeck = newDeck.Append(deck[j]).ToArray();
			table.RemoveChild(deck[j]);
			deck = deck.Where((val,index)=> index != j).ToArray();
		}
		// Move the final item to the new deck and remove it from the scene
		newDeck = newDeck.Append(deck[0]).ToArray();
		table.RemoveChild(deck[0]);
		// Update the deck with the shuffled deck
		deck = newDeck;
		// Arrange the newly shuffled deck for display
		for (int i = 0; i < deck.Length; i++)
		{
			table.AddChild(deck[i]);
			int x = i % 13 * 45;
			int y = i / 13 * 80;
			deck[i].Position = new Vector2(x, y);
		}
	}
}
