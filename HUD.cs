using Godot;
using System;
using System.Runtime.InteropServices;

public partial class HUD : CanvasLayer
{
    [Signal]
    public delegate void HitEventHandler();
    [Signal]
    public delegate void EndTurnEventHandler();
    [Signal]
    public delegate void NewHandEventHandler();

    private void OnHitButtonPressed()
    {
        EmitSignal(SignalName.Hit);
    }

    private void OnStandButtonPressed()
    {
        EmitSignal(SignalName.EndTurn);
    }

    private void OnNewHandButtonPressed()
    {
        EmitSignal(SignalName.NewHand);
    }

    public void PlayerTurnUI()
    {
        GetNode<Label>("Message").Hide();
        GetNode<Button>("NewHandButton").Hide();
        GetNode<Button>("HitButton").Show();
        GetNode<Button>("StandButton").Show();
    }

    public void HideAllButtons()
    {
        GetNode<Label>("Message").Hide();
        GetNode<Button>("HitButton").Hide();
        GetNode<Button>("StandButton").Hide();
        GetNode<Button>("NewHandButton").Hide();
    }

    public void EndOfTurnUI([Optional] string messageText)
    {
        var message = GetNode<Label>("Message");
        message.Text = messageText;
        message.Show();
        GetNode<Button>("HitButton").Hide();
        GetNode<Button>("StandButton").Hide();
        GetNode<Button>("NewHandButton").Show();
    }

    public void UpdateWins(int wins)
    {
        GetNode<Label>("Wins").Text = "Wins: "+wins;
    }
}
