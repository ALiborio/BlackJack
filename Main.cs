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
		ShuffleDeck();
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
		for (int i = 0; i < _deck.Length; i++)
		{
			deck.MoveChild(_deck[i],_deck.Length-i);
			_deck[i].Position = new Vector2(-i, i);
		}
	}

	private void DrawCard()
	{
		var deck = GetNode<Node2D>("Table").GetNode<Node2D>("Deck");
		var playerHand = GetNode<Node2D>("Table").GetNode<Hand>("PlayerHand");

		if (_deck.Length > 0 && playerHand.IsActive())
		{
			// Draw the top card from the deck and put it in the hand
			var card = _deck[_deck.Length-1];
			_deck = _deck.Where((value,index)=> index != _deck.Length-1).ToArray();
			deck.RemoveChild(card);
			var cardCount = playerHand.GetChildren().Count;
			playerHand.AddChild(card);
			card.Position = new Vector2(40 * cardCount,0);
			playerHand.CalculateScore();
		}

		if (_deck.Length == 0)
		{
			GetNode<Node2D>("Table").GetNode<Panel>("CardBack").Hide();
		}
	}
	
	public void EndTurn()
	{
		GetNode<Node2D>("Table").GetNode<Hand>("PlayerHand").EndTurn();
		// TODO: Dealer turn
		TurnOver();
	}

	public void TurnOver()
	{
		var hud = GetNode<HUD>("HUD");
		hud.GetNode<Button>("HitButton").Hide();
		hud.GetNode<Button>("StandButton").Hide();
		hud.GetNode<Button>("NewHandButton").Show();
	}

	public void ClearTable()
	{
		var table = GetNode<Node2D>("Table");
		var playerHand = table.GetNode<Hand>("PlayerHand");
		var dealerHand = table.GetNode<Hand>("DealerHand");
		var discardPile = table.GetNode<Node2D>("DiscardPile");
		foreach(var child in playerHand.GetChildren().ToArray())
		{
			if (child is Card)
			{
				var card = child as Card;
				playerHand.RemoveChild(card);
				discardPile.AddChild(card);
			}
		}
		foreach(var child in dealerHand.GetChildren().ToArray())
		{
			if (child is Card)
			{
				var card = child as Card;
				dealerHand.RemoveChild(card);
				discardPile.AddChild(card);
			}
		}
		playerHand.ResetHand();
		dealerHand.ResetHand();

		var hud = GetNode<HUD>("HUD");
		hud.GetNode<Button>("HitButton").Show();
		hud.GetNode<Button>("StandButton").Show();
		hud.GetNode<Button>("NewHandButton").Hide();
	}

	public void PlayerBust()
	{
		TurnOver();
	}

	public void PlayerBlackJack()
	{
		// TODO: Dealer turn
		TurnOver();
	}
}
