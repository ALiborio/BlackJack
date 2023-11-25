using Godot;
using System;

public partial class Hand : Node2D
{
	[Export]
	public string HandOwner { get; set; }
	[Signal]
	public delegate void BustEventHandler();
	[Signal]
	public delegate void BlackJackEventHandler();

	private int _score;
	private int _altScore; // Aces can be 1 or 11, this score is when they are treated as 11
	private Status _status = Status.Active;

	enum Status
	{
		Active,
		Stand,
		BlackJack,
		Bust
	}

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		GetNode<Label>("HandLabel").Text = HandOwner+"'s Hand";
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void CalculateScore()
	{
		int score = 0;
		int aces = 0;
		int cards = 0;
		foreach (var child in GetChildren())
		{
			if (child is Card)
			{
				cards++;
				var card = child as Card;
				score += card.GetScoreValue();
				if (card.GetScoreValue()==1)
				{
					aces++;
				}
			}
		}

		if (score > 0)
		{
			_score = score;
			if (score > 21) {
				_status = Status.Bust;
				EmitSignal(SignalName.Bust);
			} else if (score == 21 && cards == 2) {
				_status = Status.BlackJack;
				EmitSignal(SignalName.BlackJack);
			} else if (_status != Status.Stand) {
				_status = Status.Active;
			}
			// Update the label with the score
			string statusText = "";
			if (_status != Status.Active)
			{
				statusText = _status.ToString().ToUpper();
			}
			string altScoreText = "";
			if (aces > 0)
			{
				_altScore = aces * 10 + _score;
				if (_altScore < 21)
				{
					altScoreText = "/"+(aces * 10 + _score).ToString();
				}
			}
			GetNode<Label>("HandLabel").Text = HandOwner + "'s Hand (" + _score + altScoreText + ") " + statusText;
		}
	}

	public void EndTurn()
	{
		if (_status == Status.Active)
		{
			_status = Status.Stand;
		}
		CalculateScore();
	}

	public void ResetHand()
	{
		_score = 0;
		_altScore = 0;
		_status = Status.Active;
		GetNode<Label>("HandLabel").Text = HandOwner+"'s Hand";
	}

	public Boolean IsBust()
	{
		return _status == Status.Bust;
	}

	public Boolean IsBlackJack()
	{
		return _status == Status.BlackJack;
	}

	public Boolean IsStand()
	{
		return _status == Status.Stand;
	}

	public Boolean IsActive()
	{
		return _status == Status.Active;
	}

	public int GetScore()
	{
		if (_altScore < 22)
			return _altScore;
		return _score;
	}
}
