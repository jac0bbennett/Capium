using System.Runtime.CompilerServices;

namespace Inferno;

public abstract class StateContainerBase : IStateContainer
{
    public event Action? OnChange;

    private readonly ConditionalWeakTable<object, object> _propertyValues = new();

    protected T GetProperty<T>([CallerMemberName] string? propertyName = null)
    {
        if (propertyName != null && _propertyValues.TryGetValue(propertyName, out var value))
        {
            return (T)value;
        }

        return default!;
    }

    protected void SetProperty<T>(T value, [CallerMemberName] string? propertyName = null, IEqualityComparer<T>? comparer = null)
    {
        comparer ??= EqualityComparer<T>.Default;

        if (propertyName == null) return;

        var currentValue = GetProperty<T>(propertyName);

        if (comparer.Equals(currentValue, value)) return;
        
        _propertyValues.Remove(propertyName);
        if (value != null) _propertyValues.Add(propertyName, value);

        NotifyStateChanged();
    }

    public IDisposable Subscribe(Action? callback)
    {
        OnChange += callback;
        return new Subscription(() => OnChange -= callback);
    }

    public void NotifyStateChanged() => OnChange?.Invoke();

    private class Subscription : IDisposable
    {
        private readonly Action _unsubscribe;

        public Subscription(Action unsubscribe)
        {
            _unsubscribe = unsubscribe;
        }

        public void Dispose()
        {
            _unsubscribe?.Invoke();
        }
    }
}