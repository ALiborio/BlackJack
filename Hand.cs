using Godot;
using System;
using System.Runtime.InteropServices;

public partial class Hand : Node2D
{
	[Export]
	public string HandOwner { get; set; }
	[Signal]
	public delegate void BustEventHandler();

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
		UpdateLabel();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	private void UpdateLabel([Optional]string additionalText)
	{
		GetNode<Label>("HandLabel").Text = HandOwner + "'s Hand" + additionalText;
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
				if (card.IsFaceUp())
				{
					score += card.GetScoreValue();
					if (card.GetScoreValue()==1)
					{
						aces++;
					}
				}
			}
		}
		
		if (aces > 0)
		{
			_altScore = (aces * 10) + score;
		}

		if (score > 0)
		{
			_score = score;
			if (_score > 21) {
				_status = Status.Bust;
				EmitSignal(SignalName.Bust);
			} else if (_altScore == 21 && cards == 2) {
				_status = Status.BlackJack;
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
			if (_altScore > 0 &&_altScore < 22)
			{
				altScoreText = "/"+_altScore.ToString();
			}
			string additionalText;
			if (_status == Status.BlackJack)
			{
				additionalText = " "+statusText;
			}
			else 
			{
				additionalText = " (" + _score + altScoreText + ") " + statusText;
			}
			UpdateLabel(additionalText);
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

	public void ShowAllCards()
	{
		foreach (var child in GetChildren())
		{
			if (child is Card)
			{
				var card = child as Card;
				card.FlipUp();
			}
		}
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
		if (_altScore > 0 &&_altScore < 22)
			return _altScore;
		return _score;
	}
}
