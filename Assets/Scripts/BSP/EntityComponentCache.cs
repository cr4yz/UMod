using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityComponentCache
{
    public static Dictionary<string, Type> Registrations = new Dictionary<string, Type>();
}

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public class EntityComponentAttribute : Attribute
{
    public EntityComponentAttribute(string name)
    {
        Name = name;
    }
    public readonly string Name;
}
