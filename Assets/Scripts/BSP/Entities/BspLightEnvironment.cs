using SourceUtils.ValveBsp.Entities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[EntityClass("light_environment")]
public class BspEntityLightEnvironment : Entity
{
    [EntityField("_ambient")]
    public SourceUtils.Color32 Ambient { get; private set; }
    [EntityField("_ambienthdr")]
    public SourceUtils.Color32 AmbientHDR { get; private set; }
    [EntityField("_Ambientscalehdr")]
    public float AmbientScaleHDR { get; private set; }
    [EntityField("_light")]
    public SourceUtils.Color32 Light { get; private set; }
    [EntityField("_lighthdr")]
    public SourceUtils.Color32 LightHDR { get; private set; }
    [EntityField("_lightscalehdr")]
    public float LightScaleHDR { get; private set; }
    [EntityField("pitch")]
    public int Pitch { get; private set; }
}

[EntityComponent("light_environment")]
public class BspLightEnvironment : GenericBspEntityMonoBehaviour<BspEntityLightEnvironment>
{
    private void Start()
    {
        if (BspOptions.Sun == null)
        {
            return;
        }
        BspOptions.Sun.transform.localPosition = Vector3.zero;
        BspOptions.Sun.transform.rotation = Quaternion.Euler(new Vector3(-Entity.Pitch, -Entity.Angles.Y + 90, -Entity.Angles.X));
        BspOptions.Sun.color = new Color32(Entity.Light.R, Entity.Light.G, Entity.Light.B, Entity.Light.A);
    }
}
