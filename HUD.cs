using Godot;
using System;

public partial class HUD : CanvasLayer
{
	[Signal]
	public delegate void HitEventHandler();
	[Signal]
	public delegate void EndTurnEventHandler();
	[Signal]
	public delegate void NewHandEventHandler();

	public void OnHitButtonPressed()
	{
		EmitSignal(SignalName.Hit);
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
