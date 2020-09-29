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

        var spawns = GameObject.FindObjectsOfType<BspInfoPlayerStart>();
        if(spawns.Length > 0)
        {
            var randomSpawn = spawns[Random.Range(0, spawns.Length)];
            var movement = GameObject.FindObjectOfType<Movement>();
            movement.Origin = randomSpawn.transform.position;
            movement.Angles = randomSpawn.transform.eulerAngles;
        }
    }
}
