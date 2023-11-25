using Godot;
using System;

public partial class HUD : CanvasLayer
{
	[Signal]
	public delegate void DrawCardEventHandler();

	public void OnDrawCardButtonPressed()
	{
		EmitSignal(SignalName.DrawCard);
	}
}
