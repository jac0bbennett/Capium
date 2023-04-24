namespace Inferno;

public interface IStateContainer
{
    IDisposable Subscribe(Action? callback);
    void NotifyStateChanged();
}