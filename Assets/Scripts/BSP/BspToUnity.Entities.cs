using System;
using System.Linq;
using UnityEngine;
using SourceUtils.ValveBsp.Entities;

public partial class BspToUnity
{

    private void GenerateEntities()
    {
        EntityComponentCache.Registrations.Clear();

        foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
        {
            foreach (var currentType in assembly.GetTypes().Where(_ => typeof(MonoBehaviour).IsAssignableFrom(_)))
            {
                var attributes = currentType.GetCustomAttributes(typeof(EntityComponentAttribute), false);
                if (attributes.Length > 0)
                {
                    var targetAttribute = attributes.First() as EntityComponentAttribute;
                    if (EntityComponentCache.Registrations.ContainsKey(targetAttribute.Name))
                    {
                        Debug.LogError("Duplicate entity component: " + targetAttribute.Name);
                        continue;
                    }
                    EntityComponentCache.Registrations.Add(targetAttribute.Name, currentType);
                }
            }
        }

        var entityParent = CreateGameObject("[Entities]");

        foreach (var e in _bsp.Entities)
        {
            GameObject obj = null;

            if (e.Properties.ContainsKey("model")
                && ((string)e.Properties["model"])[0] == '*')
            {
                var modelIndex = int.Parse(((string)e.Properties["model"]).Replace("*", null));
                obj = _models[modelIndex];
                obj.gameObject.SetActive(true);
            }

            if (obj == null)
            {
                obj = new GameObject();
                obj.transform.SetParent(entityParent.transform, true);
            }

            obj.transform.position = e.Origin.ToUVector() * _options.WorldScale;
            obj.transform.Rotate(new Vector3(-e.Angles.X, -e.Angles.Y, -e.Angles.Z));
            obj.name = $"{e.ClassName} [${e.TargetName}]";

            if (EntityComponentCache.Registrations.ContainsKey(e.ClassName))
            {
                var typeToLoad = EntityComponentCache.Registrations[e.ClassName];
                var component = obj.AddComponent(typeToLoad);
                if(component is BspEntityMonoBehaviour bspEntity)
                {
                    bspEntity.Entity = e;
                    bspEntity.BspOptions = _options;
                }
            }
        }
    }

}
