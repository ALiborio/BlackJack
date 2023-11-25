using Godot;
using System;

public partial class Card : Node2D
{
	private int _value { get; set; } = 1;
	private Suits _suit { get; set;}
	private bool _faceDown = true;

	public void Init(Suits initSuit, int initValue)
	{
		_value = initValue;
		_suit = initSuit;
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
		valueLabel.Text = valueDisplay[_value-1].ToString();
		var suitLabel = GetNode<Label>("SuitLabel");
		switch (_suit)
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
		showHideCardBack();
	}

	private void showHideCardBack()
	{
		if (_faceDown)
		{
			GetNode<Panel>("CardBack").Show();
		}
		else
		{
			GetNode<Panel>("CardBack").Hide();
		}
	}

	public int GetScoreValue()
	{
		// _values of > 10 have a score of 10
		if (_value > 10)
		{
			return 10;
		} else {
			return _value;
		}
	}

	public void FlipUp()
	{
		_faceDown = false;
		showHideCardBack();
	}

	public void FlipDown()
	{
		_faceDown = true;
		showHideCardBack();
	}

	public bool IsFaceUp()
	{
		return !_faceDown;
	}
}
