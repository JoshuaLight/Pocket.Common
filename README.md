# Pocket.Common

[![Build status](https://ci.appveyor.com/api/projects/status/kwed1k33oxbs8a0j/branch/master?svg=true)](https://ci.appveyor.com/project/JoshuaLight/pocket-common/branch/master)
[![codecov](https://codecov.io/gh/JoshuaLight/Pocket.Common/branch/master/graph/badge.svg)](https://codecov.io/gh/JoshuaLight/Pocket.Common)
[![NuGet](https://img.shields.io/nuget/v/Pocket.Common.svg)](https://www.nuget.org/packages/Pocket.Common)

This repository contains a lot of common utilities and extensions, which can be useful in everyday development. They're all implemented using fluent and declarative style.

## Extensions

### Collections

#### `AddRange`

Adds a range of items to the `ICollection<T>` instance (pretty common extension that is missing in standard library).

```cs
ICollection<int> numbersA = { 1, 2, 3 };
IEnumerable<int> numbersB = { 4, 5, 6 };

numbersA.AddRange(numbersB);

Console.WriteLine(numbersA.AsString()); // Prints "[1, 2, 3, 4, 5, 6]".
```

### Dictionary

#### `One`

Represents a consistent access to dictionary elements in four different ways using fluent sentences, starting with `One`. Each way represents a reaction to situation, when key is missing.

These reactions are:

a) throw a `KeyNotFoundException` with specified message:
```cs
var writers = new Dictionary<string, string>
{
    { "Hermann": "Hesse" },
};

var x = writers.One(withKey: "Thomas").OrThrow(withMessage: "Couldn't find `Thomas`.");
// `KeyNotFoundException` is thrown.
```

b) return `default(TValue)`:
```cs
var writers = new Dictionary<string, string>
{
    { "Hermann": "Hesse" },
};

var x = writers.One(withKey: "Thomas").OrDefault();
Console.WriteLine(x ?? "null"); // Prints `null`.
```

c) return specified value:
```cs
var writers = new Dictionary<string, string>
{
    { "Hermann": "Hesse" },
};

var x = writers.One(withKey: "Thomas").Or("");
var y = writers.One(withKey: "Thomas").Or(() => "");
Console.WriteLine(x); // Prints `""`.
Console.WriteLine(y); // Prints `""`.
```

d) return specified value and also write it to dictionary:
```cs
var writers = new Dictionary<string, string>
{
    { "Hermann": "Hesse" },
};

var x = writers.One(withKey: "Thomas").OrNew("Mann");
var y = writers["Thomas"];
Console.WriteLine(x); // Prints `"Mann"`.
Console.WriteLine(y); // Prints `"Mann"`.
```

### Enumerable

#### `OrEmpty`

Returns `Enumerable.Empty<T>` sequence if current is `null`. This method can be used
as a more fluent alternative to `x ?? Enumerable.Empty<T>`.

```cs
IEnumerable<int> x = null;

Console.WriteLine(x.OrEmpty()); // Prints [].
```

