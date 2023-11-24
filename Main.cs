using Godot;
using System;
using System.Linq;

public partial class Main : Node2D
{
	[Export]
	public PackedScene CardScene { get; set; }

	private Card[] _deck = Array.Empty<Card>();

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
		var deck = GetNode<Node2D>("Table").GetNode<Node2D>("Deck");
		GD.Print("Generate the Deck");
		// Generate Cards for all values (1-13) and suits (club, diamond, heart, spade)
		foreach (Card.Suits suit in Enum.GetValues(typeof(Card.Suits)))
		{
			for (int value = 1; value < 14; value++)
			{
				var card = CardScene.Instantiate<Card>();
				card.Init(suit,value);
				_deck = _deck.Append(card).ToArray();
				deck.AddChild(card);
			}
		}
		ArrangeDeck();
	}

	public void ShuffleDeck()
	{
		GD.Print("Shuffle the Deck");
		Card[] newDeck = Array.Empty<Card>();
		for (int i = _deck.Length - 1; i > 0; i--)
		{
			var j = GD.RandRange(0,i);
			newDeck = newDeck.Append(_deck[j]).ToArray();
			_deck = _deck.Where((value,index)=> index != j).ToArray();
		}
		// Move the final item to the new deck
		newDeck = newDeck.Append(_deck[0]).ToArray();
		// Update the deck with the shuffled deck
		_deck = newDeck;
		ArrangeDeck();
	}

	private void ArrangeDeck()
	{
		// Arrange the cards in the deck
		var deck = GetNode<Node2D>("Table").GetNode<Node2D>("Deck");
		var position = deck.Position;
		for (int i = 0; i < _deck.Length; i++)
		{
			deck.MoveChild(_deck[i],_deck.Length-i);
			float x = i*-1;
			float y = i;
			_deck[i].Position = new Vector2(x, y);
		}
	}
}
