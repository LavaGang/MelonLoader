# MelonLoader Differences

MelonLoader have some "small" things that doesn't work the exact same as if you were modding a Mono game.

> If you find something that doesn't seems natural with Il2Cpp and that isn't listed here, please ping _Slaynash#2879_ on the [MelonLoader Discord](https://discord.gg/2Wn3N2P).

### Custom Components

Custom components (and custom Types in general) are currently not supported by MelonLoader over Il2Cpp.<br/>
This means you can't create a new class that extends `Component`, `MonoBehaviour`, or another existing Il2Cpp type.

### Coroutines

!> This part contains some unreleased code.

In a standard Mono game, you would use `StartCoroutine`. Since MelonMod isn't a component, and that anyway we can't use a Mono IEnumerator on Il2Cpp due to how those are handled in C#, we need to do it another way.

To do that, MelonLoader includes replacement class: `MelonLoader.MelonCoroutines`!
 - `CoroD Start<T>(T routine)`: Starts a new coroutine, which will be ran at the end of the frame.
 - `void Stop(CoroD routine)`: Stops a running coroutine.

### Usage of Il2Cpp Types

In case you want to run an Il2Cpp method taking a type, you may want to use `.GetType()`.<br/>
`.GetType()` would actually returns the Mono type, and not the original Il2Cpp type. To do so, we need to replace it with `.Il2CppType`.
```cs
Resources.FindObjectsOfTypeAll(Camera.Il2CppType);
```
Note: you can use the Mono type directly in case of generic method:
```cs
Resources.FindObjectsOfTypeAll<Camera>();
```

### Il2Cpp types and casting

In a standard Mono game, casting is quite easy.<br/>
Let's say we have a class `MyChildClass`, which inherit from `MyParentClass`.<br />
Casting it in Mono would go like that:
```cs
MyParentClass childInstance; // This values is already defined

MyChildClass childInstanceCasted = childInstance as MyChildClass;
// or
MyChildClass childInstanceCasted = (MyChildClass) childInstance;
```
We can't do that with Il2Cpp objects. We have to use some methods from Il2CppAssemblyUnhollower:
```cs
using UnhollowerBaseLib;
// ...
MyChildClass childInstanceCasted = childInstance.TryCast<MyChildClass>();
// or
MyChildClass childInstanceCasted = childInstance.Cast<MyChildClass>();
```

### System.String vs Il2CppSystem.String

in a lot of languages, the type `string` is used the same way as if it was a primitive type. This isn't a primitive type, so the same rules as Il2Cpp objects apply.

Most methods that was asking for a `string` (`System.String`) will now ask for an `Il2CppSystem.String`. Thankfully, `Il2CppSystem.String` has an explicit cast for `string`.<br/>
This means we can use it like this:
```cs
Debug.Log((Il2CppSystem.String) "Hello World!");
```

### Actions

We know, you all love those delegates. Except they don't work as-is with Il2Cpp.

Most Unity events have an implicit cast of `System.Action`, which is a delegate type.<br>
The current issue is that an Il2Cpp event will now take an `Il2CppSystem.Action`, which isn't a delegate type anymore. We will have to use the implicit cast of `Il2CppSystem.Action` to cast an `System.Action`.

Let's say we have a method `MyComponent.AddAction(Action<Component> onComponentDidSomething)`. We can use it like this:
```cs
MyComponent.AddAction(new System.Action<Component>(component => {
    MelonModLogger.Log($"The component {component.name} did something!");
}))
```
or with `MyComponent.AddUnityEvent(UnityEngine.UnityEvent<Component> onComponentDidSomething)`:
```cs
MyComponent.AddUnityEvent(new System.Action<Component>(component => {
    MelonModLogger.Log($"The component {component.name} did something!");
}))
```

### Events

!> This code is subject to change and become easier with the upcoming updates

Event in unity are kinda similar to properties.<br/>
let's imagine you have an `event Action<Player> onPlayerJoin;`. This code will generate the following methods:
 - `add_onPlayerJoin(Action<Player>)`: equivalent of doing `onPlayerJoin +=`
 - `remove_onPlayerJoin(Action<Player>)`: equivalent of doing `onPlayerJoin -=`

As expected, the `+=` and `-=` methods can't be used on an Il2Cpp type, since the type has already been processed. We will have to use the generated methods. Since these methods were using some `System.Action`, these have been converted to `Il2CppSystem.Action`.<br/>
Using MelonLoader, our event is now something like this:
```cs
Action<Player> onPlayerJoin;
public void add_onPlayerJoin(Il2CppSystem.Action<Player> value) { /* does stuff with onPlayerJoin */ }
public void remove_onPlayerJoin(Il2CppSystem.Action<Player> value) { /* does stuff with onPlayerJoin */ }
```

In case we want to add an event calling `void MyEventListener(Player player)`, we now have to do the following:
```cs
playerManagerInstance.add_onPlayerJoin(new Action<Player>(MyEventListener));
// ...
void MyEventListener(Player player) { /* Do things */ }
```

Sometime, the `add_` and `remove_` methods are stripped by the Il2Cpp compiler. In such cases, we have to use the `CombineImpl` method:
```cs
playerManagerInstance.onPlayerJoin
    .CombineImpl((Il2CppSystem.Action<Player>) MyEventListener)
```
