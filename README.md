# Capium

<img src="https://github.com/jac0bbennett/Capium/blob/main/assets/logo.png" width="120" />

Capium is a lightweight, boilerplate-less state management package for Blazor. It offers a dead simple solution for your app's global state.

## Features

- Simple and intuitive API
- Automatic handling of different environments (WebAssembly vs Server)
- Automatic container registration as a service

## Installation

To install Capium, add the package to your Blazor project:

```sh
dotnet add package Capium
```

## Usage

### **Step 1: Create your state container**

Inherit from the `StateContainerBase` class and define your properties using the provided GetProperty and SetProperty methods:

```csharp
namespace YourProject.StateContainers;

public class CounterStateContainer : StateContainerBase
{
    public int Count
    {
        get => GetProperty<int>();
        set => SetProperty(value);
    }
}
```

Any subsciptions to the `StateContainerBase.OnChange` event will be notified automatically.

If you want more control over when `NotifyStateChanged` is called, you can omit the `GetProperty` and `SetProperty` methods like so:

```csharp
namespace YourProject.StateContainers;

public class CounterStateContainer : StateContainerBase
{
    public int Count { get; set; }

    public void Increment()
    {
        Count++;
        NotifyStateChanged();
    }
}
```

### **Step 2: Register your state containers**

In your Program.cs file, register your state containers using the RegisterStateContainers extension method:

```csharp
using Capium;

// ...

services.RegisterStateContainers(Assembly.GetExecutingAssembly());
```

For a WASM app, the containers will be registered as singletons. For Blazor Server, the containers will be registered as scoped services.

### **Step 3: Inject and use your state container**

In your Blazor components, inject the state container and use it as needed:

```razor
@inject CounterStateContainer CounterState

<button @onclick="Increment">Increment</button>
<p>Current count: @CounterState.Count</p>

@code {
    private void Increment()
    {
        CounterState.Count++;
    }
}
```

To react to state changes, subscribe to the state container in the OnInitialized lifecycle method and unsubscribe when disposing the component:

```razor
@inject CounterStateContainer CounterState
@implements IDisposable

<button @onclick="Increment">Increment</button>
<p>Current count: @CounterState.Count</p>

@code {
    private IDisposable _subscription;

    protected override void OnInitialized()
    {
        _subscription = CounterState.Subscribe(() => InvokeAsync(StateHasChanged));
    }

    private void Increment()
    {
        CounterState.Count++;
    }

    public void Dispose()
    {
        _subscription?.Dispose();
    }
}
```
