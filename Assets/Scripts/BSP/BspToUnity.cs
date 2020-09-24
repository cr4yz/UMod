using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using SourceUtils;

public partial class BspToUnity
{

    private BspToUnityOptions _options;
    private ResourceLoader _resourceLoader;
    private ValveBspFile _bsp;
    private GameObject _rootObject;
    private Dictionary<int, GameObject> _models;
    private int _currentLightmap = 0;

    public BspToUnity(BspToUnityOptions options) 
    {
        if (options.FilePath.StartsWith("sa://", StringComparison.InvariantCultureIgnoreCase))
        {
            options.FilePath = Path.Combine(Application.streamingAssetsPath, options.FilePath.Remove(0, 5));
        }
        if (!options.FilePath.EndsWith(".bsp"))
        {
            options.FilePath = options.FilePath + ".bsp";
        }
        _options = options;
        _bsp = new ValveBspFile(options.FilePath);
    }

    public GameObject Generate()
    {
        _rootObject = new GameObject(_bsp.Name);
        _resourceLoader = new ResourceLoader();
        _resourceLoader.AddResourceProvider(_bsp.PakFile);

        foreach (var appid in _options.GamesToMount)
        {
            if (SourceMounter.Mount(appid))
            {
                foreach (var vpk in SourceMounter.MountedContent[appid])
                {
                    _resourceLoader.AddResourceProvider(vpk);
                }
            }
        }

        if (_options.WithModels)
        {
            GenerateModels(0, _bsp.Models.Count());
        }

        if (_options.WithDisplacements)
        {
            GenerateDisplacements();
        }

        if (_options.WithLightmaps)
        {
            GenerateLightmapPixels();
        }

        if (_options.WithConvexColliders)
        {
            GeneratePhysModels();
        }
        else
        {
            // add mesh colliders
        }

        if (_options.WithEntities)
        {
            GenerateEntities();
        }

        return _rootObject;
    }

    private GameObject CreateGameObject(string name = null, GameObject parent = null)
    {
        var obj = new GameObject();
        obj.name = name ?? obj.name;
        obj.transform.SetParent(parent ? parent.transform : _rootObject.transform);
        obj.transform.localPosition = UnityEngine.Vector3.zero;
        obj.transform.localScale = UnityEngine.Vector3.one;
        return obj;
    }

}
