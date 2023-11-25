using Godot;
using System;

public partial class Hand : Node2D
{
	[Export]
	public string HandOwner { get; set; }

	private int _score;
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
		foreach (var child in GetChildren())
		{
			if (child is Card)
			{
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
			} else if (score == 21) {
				_status = Status.BlackJack;
			} else {
				_status = Status.Active;
			}
			// Update the label with the score
			string statusText = "";
			if (_status != Status.Active)
			{
				statusText = _status.ToString().ToUpper();
			}
			string altScoreText = "";
			if (aces > 0 && (aces * 10 + _score) < 21)
			{
				altScoreText = "/"+(aces * 10 + _score).ToString();
			}
			GetNode<Label>("HandLabel").Text = HandOwner + "'s Hand (" + _score + altScoreText + ") " + statusText;
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
}
