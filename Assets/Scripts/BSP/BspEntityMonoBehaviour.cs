using SourceUtils.ValveBsp.Entities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BspEntityMonoBehaviour : MonoBehaviour
{
    public Entity Entity;
    public BspToUnityOptions BspOptions;
}

public class GenericBspEntityMonoBehaviour<T> : BspEntityMonoBehaviour
    where T : Entity
{
    public new T Entity => base.Entity as T;
}
