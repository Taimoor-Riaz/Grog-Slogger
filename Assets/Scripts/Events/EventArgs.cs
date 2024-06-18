using System;

public class OnInteractableChangedEventArgs : EventArgs
{
    public BaseInteractable selectedCounter;
    public OnInteractableChangedEventArgs(BaseInteractable selectedCounter)
    {
        this.selectedCounter = selectedCounter;
    }
}
public class OnControllerStateChangedEventArgs : EventArgs
{
    public ControllerState controllerState;
    public OnControllerStateChangedEventArgs(ControllerState controllerState)
    {
        this.controllerState = controllerState;
    }
}
public class OnCoinCollectedEventArgs : EventArgs
{
    public Coin Coin { get; private set; }
    public OnCoinCollectedEventArgs(Coin coin)
    {
        this.Coin = coin;
    }
}