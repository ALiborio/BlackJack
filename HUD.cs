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
    [Signal]
    public delegate void PlaceBetEventHandler();

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

    private void OnPlaceBetButtonPressed()
    {
        EmitSignal(SignalName.PlaceBet);
    }

    public void PlayerTurnUI()
    {
        HideAllButtons();
        GetNode<Button>("HitButton").Show();
        GetNode<Button>("StandButton").Show();
        GetNode<Label>("CurrentBet").Show();
    }

    public void HideAllButtons()
    {
        GetNode<Label>("Message").Hide();
        GetNode<Button>("HitButton").Hide();
        GetNode<Button>("StandButton").Hide();
        GetNode<Button>("NewHandButton").Hide();
        GetNode<Control>("BetControls").Hide();
        GetNode<Label>("CurrentBet").Hide();
    }

    public void EndOfTurnUI([Optional] string messageText)
    {
        HideAllButtons();
        var message = GetNode<Label>("Message");
        message.Text = messageText;
        message.Show();
        GetNode<Button>("NewHandButton").Show();
    }

    public void BettingStageUI(int defaultBet, int maxValue)
    {
        HideAllButtons();
        var betControls = GetNode<Control>("BetControls");
        betControls.Show();
        betControls.GetNode<SpinBox>("BetPicker").Value = defaultBet;
        betControls.GetNode<SpinBox>("BetPicker").MaxValue = maxValue;
    }

    public void UpdateMinMaxBets(int min, int max)
    {
        var betControls = GetNode<Control>("BetControls");
        betControls.GetNode<Label>("MinimumBet").Text = "Minimum Bet: $" + min;
        betControls.GetNode<Label>("MaximumBet").Text = "Maximum Bet: $" + max;
        var betPicker = betControls.GetNode<SpinBox>("BetPicker");
        betPicker.MinValue = min;
        betPicker.MaxValue = max;
        betPicker.Value = min;
    }

    public int GetBetAmount()
    {
        return (int)GetNode<Control>("BetControls").GetNode<SpinBox>("BetPicker").Value;
    }

    public void UpdateCurrentBet(int bet)
    {
        GetNode<Label>("CurrentBet").Text = "Current Bet: $" + bet;
    }

    public void UpdateMoney(int value)
    {
        GetNode<Label>("Money").Text = "$" + value;
    }

    public void GameOver()
    {
        HideAllButtons();
        var message = GetNode<Label>("Message");
        message.Text = "You ran out of money!";
        message.Show();
    }
}
