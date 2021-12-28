namespace Server.HellPortalSystem;

public class StateTransition
{
    private readonly HellPortalState _currentState;
    private readonly HellPortalCommand _command;

    public StateTransition(HellPortalState currentState, HellPortalCommand command)
    {
        _currentState = currentState;
        _command = command;
    }

    public override int GetHashCode()
    {
        return 17 + 31 * _currentState.GetHashCode() + 31 * _command.GetHashCode();
    }

    public override bool Equals(object obj)
    {
        StateTransition other = obj as StateTransition;
        return other != null && this._currentState == other._currentState && this._command == other._command;
    }
}
