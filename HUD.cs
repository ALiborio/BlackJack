using Godot;
using System;

public partial class HUD : CanvasLayer
{
	[Signal]
	public delegate void DrawCardEventHandler();
	[Signal]
	public delegate void EndTurnEventHandler();
	[Signal]
	public delegate void NewHandEventHandler();

	public void OnHitButtonPressed()
	{
		EmitSignal(SignalName.DrawCard);
	}

	public void OnStandButtonPressed()
	{
		EmitSignal(SignalName.EndTurn);
	}

	public void OnNewHandButtonPressed()
	{
		EmitSignal(SignalName.NewHand);
	}
}
