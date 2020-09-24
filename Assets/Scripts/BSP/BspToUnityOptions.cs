using System;
using UnityEngine;

[Serializable]
public class BspToUnityOptions
{
    [Header("Import Options")]
    public string FilePath;
    public float WorldScale = .0254f;
    public bool WithModels = true;
    public bool WithDisplacements = true;
    public bool WithLightmaps = true;
    public bool WithConvexColliders = true;
    public bool WithEntities = true;
    public string[] GamesToMount;

    [Header("Generate Options")]
    public Material SkyboxMaterial;
    public Material FaceMaterial;
    public Material WaterMaterial;

    [Header("Entity Options")]
    public Light Sun;
}
