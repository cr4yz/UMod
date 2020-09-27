using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class LoadBsp : MonoBehaviour
{
    [Header("Helpers")]
    public string MapDirectory;

    public BspToUnityOptions Options;

    private void Start()
    {
        if (!string.IsNullOrEmpty(MapDirectory))
        {
            Options.FilePath = Path.Combine(MapDirectory, Options.FilePath);
        }
        var bspRoot = new BspToUnity(Options).Generate();
        bspRoot.transform.SetParent(transform, true);
    }
}
