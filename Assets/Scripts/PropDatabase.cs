using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PropDatabase", menuName = "Data/Prop Database")]
public class PropDatabase : ScriptableObject
{
    public Prop[] Props;
}

[Serializable]
public class Prop
{
    public UnityEngine.Object Object;
    public string Name;
    public string Tags;
}
