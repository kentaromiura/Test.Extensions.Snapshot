# [Test.Extensions.Snapshot](https://github.com/kentaromiura/Test.Extensions.Snapshot)
C# Snapshots utilities


This package provides a ToMatchSnapshot extension method that will be available for testing:

```c#
using Snapshot.Extensions;

public static class Test {
    public static bool ItWritesANiceSnapshot() {
        return "Anything you want to test".ToMatchSnapshot();            
    }
}
```

Which you can run with something like:
```c#
using System;
using System.Reflection;

class Program
{
    static void Main(string[] args)
    {
        foreach (var test in typeof(Test).GetMethods(BindingFlags.Static | BindingFlags.Public))
        {
            Console.Write(test.Name);
            dynamic result = test.Invoke(null, new object[] {});

            if (result)
            {
                Console.WriteLine(" : Passed");
            } else {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Failed");
                Console.ForegroundColor = ConsoleColor.White;
            }
        }

        Console.Read();
    }
}
```

It will work similarly to [Jest snapshots](https://facebook.github.io/jest/docs/en/snapshot-testing.html) and create a `__snapshot__` folder where it will create a .snap file that in this version is a JSON file.

This package doesn't provide a test runner, nor a way to track which snapshot is obsolete.
