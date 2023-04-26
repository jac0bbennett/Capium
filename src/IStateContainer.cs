namespace Capium;

public interface IStateContainer
{
    IDisposable Subscribe(Action? callback);
    void NotifyStateChanged();
}