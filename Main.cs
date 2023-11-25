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
		StartRound();
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

	public void DealCard(Hand hand, bool faceDown)
	{
		var deck = GetNode<Node2D>("Table").GetNode<Node2D>("Deck");
		if (_deck.Length > 0)
		{
			// Draw the top card from the deck and put it in the hand
			var card = _deck[_deck.Length-1];
			_deck = _deck.Where((value,index)=> index != _deck.Length-1).ToArray();
			deck.RemoveChild(card);
			var cardCount = hand.GetChildren().Count;
			hand.AddChild(card);
			card.Position = new Vector2(40 * cardCount,0);
			if (!faceDown)
				card.Flip();
			hand.CalculateScore();
		}
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

	private void Hit()
	{
		var playerHand = GetNode<Node2D>("Table").GetNode<Hand>("PlayerHand");

		if (playerHand.IsActive())
		{
			DealCard(playerHand,false);
		}
	}
	
	public void EndTurn()
	{
		GetNode<Node2D>("Table").GetNode<Hand>("PlayerHand").EndTurn();
		DealerTurn();
	}

	public void TurnOver()
	{
		var hud = GetNode<HUD>("HUD");
		hud.GetNode<Button>("HitButton").Hide();
		hud.GetNode<Button>("StandButton").Hide();
		hud.GetNode<Button>("NewHandButton").Show();
	}

	public async void DealerTurn()
	{
		var dealerHand = GetNode<Node2D>("Table").GetNode<Hand>("DealerHand");
		var actionTimer = GetNode<Timer>("ActionTimer");
		// Flip the dealer's cards
		dealerHand.ShowAllCards();
		dealerHand.CalculateScore();
		await ToSignal(actionTimer, "timeout");
		while (dealerHand.IsActive() && _deck.Length > 0) {
			if (dealerHand.GetScore() >= 17) {
				dealerHand.EndTurn();
			} else {
				DealCard(dealerHand,false);
			}
		}
		TurnOver();
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
		StartRound();
	}

	public async void StartRound()
	{
		var hud = GetNode<HUD>("HUD");
		hud.GetNode<Button>("NewHandButton").Hide();
		var actionTimer = GetNode<Timer>("ActionTimer");
		var table = GetNode<Node2D>("Table");
		var playerHand = table.GetNode<Hand>("PlayerHand");
		var dealerHand = table.GetNode<Hand>("DealerHand");
		// Deal to the player
		await ToSignal(actionTimer, "timeout");
		DealCard(playerHand,false);
		await ToSignal(actionTimer, "timeout");
		DealCard(playerHand,false);
		// Deal to the dealer
		await ToSignal(actionTimer, "timeout");
		DealCard(dealerHand,false);
		await ToSignal(actionTimer, "timeout");
		DealCard(dealerHand,true);
		// Update HUD
		hud.GetNode<Button>("HitButton").Show();
		hud.GetNode<Button>("StandButton").Show();
	}

	public void PlayerBust()
	{
		TurnOver();
	}

	public void PlayerBlackJack()
	{
		var dealerHand = GetNode<Node2D>("Table").GetNode<Hand>("DealerHand");
		// Flip the dealer's cards
		dealerHand.ShowAllCards();
		dealerHand.CalculateScore();
		dealerHand.EndTurn();
		TurnOver();
	}
}
