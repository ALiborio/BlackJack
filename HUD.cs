using Godot;
using System;

public partial class HUD : CanvasLayer
{
	[Signal]
	public delegate void ShuffleDeckEventHandler();

	public void OnShuffleDeckButtonPressed()
	{
		EmitSignal(SignalName.ShuffleDeck);
	}
}
