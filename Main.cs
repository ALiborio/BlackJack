using Godot;
using System;
using System.Linq;

public partial class Main : Node2D
{
    [Export]
    public PackedScene CardScene { get; set; }

    private Card[] _deck = Array.Empty<Card>();
    private HUD _hud;
    private int _wins = 0;
    private int _losses = 0;
    private int _handsPlayed = 0;
    private int _money = 0;
    private int _currentBet = 0;
    private const int _startMoney = 50;
    private const int _minBet = 5;
    private const int _maxBet = 100;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        InitializeGameData();
        GenerateDeck();
        ShuffleDeck();
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
    }

    private void InitializeGameData()
    {
        // In the future, get this from a settings file
        _money = _startMoney;
        _currentBet = _minBet;
        _hud = GetNode<HUD>("HUD");
        _hud.UpdateMoney(_money);
        _hud.UpdateMinMaxBets(_minBet, _maxBet);
        _hud.HideAllButtons();
        _hud.EndOfTurnUI("Welcome to BlackJack");
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
                card.Init(suit, value);
                _deck = _deck.Append(card).ToArray();
                deck.AddChild(card);
            }
        }
        ArrangeDeck();
    }

    private void ShuffleDeck()
    {
        GD.Print("Shuffle the Deck");
        Card[] newDeck = Array.Empty<Card>();
        for (int i = _deck.Length - 1; i > 0; i--)
        {
            var j = GD.RandRange(0, i);
            newDeck = newDeck.Append(_deck[j]).ToArray();
            _deck = _deck.Where((value, index) => index != j).ToArray();
        }
        // Move the final item to the new deck
        newDeck = newDeck.Append(_deck[0]).ToArray();
        // Update the deck with the shuffled deck
        _deck = newDeck;
        GD.Print(_deck[0].GetScoreValue(), _deck[1].GetScoreValue());
        ArrangeDeck();
    }

    private void ArrangeDeck()
    {
        // Arrange the cards in the deck
        var deck = GetNode<Node2D>("Table").GetNode<Node2D>("Deck");
        for (int i = 0; i < _deck.Length; i++)
        {
            deck.MoveChild(_deck[i], -1);
            _deck[i].Position = new Vector2(i, i);
        }
    }

    private void DealCard(Hand hand, bool faceDown)
    {
        var deck = GetNode<Node2D>("Table").GetNode<Node2D>("Deck");
        if (_deck.Length > 0)
        {
            // Draw the top card from the deck and put it in the hand
            var card = _deck[_deck.Length - 1];
            _deck = _deck.Where((value, index) => index != _deck.Length - 1).ToArray();
            deck.RemoveChild(card);
            var cardCount = hand.GetChildren().Count;
            hand.AddChild(card);
            card.Position = new Vector2(40 * cardCount, 0);
            if (!faceDown)
                card.FlipUp();
            hand.CalculateScore();
        }
    }

    private void Hit()
    {
        var playerHand = GetNode<Node2D>("Table").GetNode<Hand>("PlayerHand");

        if (playerHand.IsActive())
        {
            DealCard(playerHand, false);
        }
    }

    private void EndTurn()
    {
        _hud.HideAllButtons();
        GetNode<Node2D>("Table").GetNode<Hand>("PlayerHand").EndTurn();
        DealerTurn();
    }

    private void RoundOver()
    {
        _handsPlayed++;
        var playerHand = GetNode<Node2D>("Table").GetNode<Hand>("PlayerHand");
        var dealerHand = GetNode<Node2D>("Table").GetNode<Hand>("DealerHand");
        string message;
        if (playerHand.IsBust())
        {
            message = "BUSTED!\nYou Lose!";
            PlayerLoses();
        }
        else if (playerHand.IsBlackJack())
        {
            if (dealerHand.IsBlackJack())
            {
                message = "PUSH!";
                Push();
            }
            else
            {
                message = "BLACKJACK!\nYou Win!";
                PlayerWins(true);
            }
        }
        else if (dealerHand.IsBust())
        {
            message = "Dealer Busted!\nYou Win!";
            PlayerWins(false);
        }
        else if (dealerHand.IsBlackJack())
        {
            message = "Dealer Blackjack!\nYou Lose!";
            PlayerLoses();
        }
        else if (playerHand.GetScore() == dealerHand.GetScore())
        {
            message = "PUSH!";
            Push();
        }
        else if (playerHand.GetScore() > dealerHand.GetScore())
        {
            message = "You Win!";
            PlayerWins(false);
        }
        else
        {
            message = "You Lose!";
            PlayerLoses();
        }
        _hud.EndOfTurnUI(message);
        _hud.UpdateMoney(_money);
        GD.Print("Wins:", _wins, " Losses:", _losses, " Hands Played:", _handsPlayed);
    }

    private void PlayerWins(bool blackjack)
    {
        // Award the player their winnings
        _money += _currentBet + (int)(blackjack ? _currentBet * 1.5 : _currentBet);
        _wins++;
    }

    private void Push()
    {
        // Return the player's bet
        _money += _currentBet;
    }

    private void PlayerLoses()
    {
        // Player forfeits their bet
        _losses++;
    }

    private async void DealerTurn()
    {
        var dealerHand = GetNode<Node2D>("Table").GetNode<Hand>("DealerHand");
        var actionTimer = GetNode<Timer>("ActionTimer");
        // Flip the dealer's cards
        dealerHand.ShowAllCards();
        dealerHand.CalculateScore();
        while (dealerHand.IsActive() && _deck.Length > 0)
        {
            await ToSignal(actionTimer, "timeout");
            if (dealerHand.GetScore() >= 17)
            {
                dealerHand.EndTurn();
            }
            else
            {
                DealCard(dealerHand, false);
            }
        }
        RoundOver();
    }

    public void ClearTable()
    {
        var table = GetNode<Node2D>("Table");
        var playerHand = table.GetNode<Hand>("PlayerHand");
        var dealerHand = table.GetNode<Hand>("DealerHand");
        var discardPile = table.GetNode<Node2D>("DiscardPile");
        foreach (var child in playerHand.GetChildren().ToArray())
        {
            if (child is Card)
            {
                var card = child as Card;
                playerHand.RemoveChild(card);
                discardPile.AddChild(card);
            }
        }
        foreach (var child in dealerHand.GetChildren().ToArray())
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
        if (_deck.Length < 20)
        {
            var deck = table.GetNode<Node2D>("Deck");
            foreach (var child in discardPile.GetChildren().ToArray())
            {
                if (child is Card)
                {
                    var card = child as Card;
                    _deck = _deck.Append(card).ToArray();
                    discardPile.RemoveChild(card);
                    deck.AddChild(card);
                    card.FlipDown();
                }
            }
            ShuffleDeck();
        }
        AskForBets();
    }

    private void AskForBets()
    {
        _hud.HideAllButtons();
        _hud.BettingStageUI(
            _currentBet > _money ? _money : _currentBet > _maxBet ? _maxBet : _currentBet,
            _money < _maxBet ? _money : _maxBet
        );
    }

    private void PlaceBet()
    {
        _currentBet = _hud.GetBetAmount();
        _hud.UpdateCurrentBet(_currentBet);
        _money -= _currentBet;
        _hud.UpdateMoney(_money);
        StartRound();
    }

    private async void StartRound()
    {
        _hud.HideAllButtons();
        _currentBet = _hud.GetBetAmount();
        var actionTimer = GetNode<Timer>("ActionTimer");
        var table = GetNode<Node2D>("Table");
        var playerHand = table.GetNode<Hand>("PlayerHand");
        var dealerHand = table.GetNode<Hand>("DealerHand");
        // Deal to the player
        await ToSignal(actionTimer, "timeout");
        DealCard(playerHand, false);
        await ToSignal(actionTimer, "timeout");
        DealCard(playerHand, false);
        // Deal to the dealer
        await ToSignal(actionTimer, "timeout");
        DealCard(dealerHand, false);
        await ToSignal(actionTimer, "timeout");
        DealCard(dealerHand, true);
        if (playerHand.IsBlackJack())
        {
            PlayerBlackJack();
        }
        else
        {
            _hud.PlayerTurnUI();
        }
    }

    private void PlayerBust()
    {
        _hud.HideAllButtons();
        var dealerHand = GetNode<Node2D>("Table").GetNode<Hand>("DealerHand");
        // Flip the dealer's cards
        dealerHand.ShowAllCards();
        dealerHand.CalculateScore();
        RoundOver();
    }

    private void PlayerBlackJack()
    {
        _hud.HideAllButtons();
        var dealerHand = GetNode<Node2D>("Table").GetNode<Hand>("DealerHand");
        // Flip the dealer's cards
        dealerHand.ShowAllCards();
        dealerHand.CalculateScore();
        RoundOver();
    }
}
