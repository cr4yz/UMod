using SourceUtils;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public partial class BspToUnity
{

    private Dictionary<string, Material> _materialCache = new Dictionary<string, Material>();
    private Dictionary<string, Texture> _textureCache = new Dictionary<string, Texture>();

    private void LoadMaterial(MeshRenderer mr, string path)
    {
        if (!_resourceLoader.ContainsFile(path))
        {
            return;
        }

        if (path.ToLower().Contains("tools"))
        {
            mr.enabled = false;
            return;
        }

        Material mat = null;

        var pathLower = path.ToLower();
        if (_materialCache.ContainsKey(pathLower))
        {
            mat = _materialCache[pathLower];
        }
        else
        {
            using (var fs = _resourceLoader.OpenFile(path))
            {
                var vmt = ValveMaterialFile.FromStream(fs);
                mat = VmtToMaterial(_resourceLoader, vmt, path);
                _materialCache.Add(pathLower, mat);
            }
        }

        mr.material = mat;
    }

    private Material VmtToMaterial(ResourceLoader resourceLoader, ValveMaterialFile vmt, string vmtPath)
    {
        string vmtShader = default;
        bool lightmapped = default;
        bool alphaTest = default;
        bool translucent = default;
        bool noCull = default;
        bool transition = default;
        string baseTex = default;
        string baseTex2 = default;
        string bumpMap = default;
        string surfaceProp = default;
        string include = default;
        float alpha = default;
        float refractAmount = default;
        int compileSky = default;
        SourceUtils.Color32 color = default;
        SourceUtils.Color32 refractTint = default;

        var e = vmt.Shaders.GetEnumerator();
        while (e.MoveNext())
        {
            vmtShader = e.Current.ToLower();
            lightmapped = vmtShader == "lightmappedgeneric" || vmtShader == "worldvertextransition";
            transition = vmtShader == "worldvertextransition";
            alphaTest = vmt[vmtShader].ContainsKey("$alphatest") ? vmt[vmtShader]["$alphatest"] : false;
            translucent = vmt[vmtShader].ContainsKey("$translucent") ? vmt[vmtShader]["$translucent"] : false;
            noCull = vmt[vmtShader].ContainsKey("$nocull") ? vmt[vmtShader]["$nocull"] : false;
            baseTex = vmt[vmtShader].ContainsKey("$basetexture") ? vmt[vmtShader]["$basetexture"] : string.Empty;
            baseTex2 = vmt[vmtShader].ContainsKey("$basetexture2") ? vmt[vmtShader]["$basetexture2"] : string.Empty;
            bumpMap = vmt[vmtShader].ContainsKey("bumpmap") ? vmt[vmtShader]["bumpmap"] : string.Empty;
            color = vmt[vmtShader].ContainsKey("$color") ? vmt[vmtShader]["$color"] : new SourceUtils.Color32(255, 255, 255, 255);
            refractTint = vmt[vmtShader].ContainsKey("$refracttint") ? vmt[vmtShader]["$refracttint"] : new SourceUtils.Color32(255, 255, 255, 255);
            surfaceProp = vmt[vmtShader].ContainsKey("$surfaceprop") ? vmt[vmtShader]["$surfaceprop"] : "grass";
            include = vmt[vmtShader].ContainsKey("include") ? vmt[vmtShader]["include"] : string.Empty;
            alpha = vmt[vmtShader].ContainsKey("$alpha") ? vmt[vmtShader]["$alpha"] : 1.0f;
            refractAmount = vmt[vmtShader].ContainsKey("$refractamount") ? vmt[vmtShader]["$refractamount"] : 0.2f;
            compileSky = vmt[vmtShader].ContainsKey("%compilesky") ? vmt[vmtShader]["%compilesky"] : 0;
            break;
        }

        if (!string.IsNullOrEmpty(include))
        {
            using (var fs = resourceLoader.OpenFile(include))
            {
                var includedVmt = ValveMaterialFile.FromStream(fs);
                return VmtToMaterial(resourceLoader, includedVmt, include);
            }
        }

        Object materialResource;
        Material result;

        materialResource = surfaceProp == "water"
            ? _options.WaterMaterial
            : _options.FaceMaterial;

        if(materialResource == null)
        {
            var shader = Shader.Find("BSP/Lightmapped")
                ?? Shader.Find("HDRP/Lit")
                ?? Shader.Find("URP/Lit")
                ?? Shader.Find("Lightweight Render Pipeline/Lit")
                ?? Shader.Find("Standard");
            result = new Material(shader);
        }
        else
        {
            result = GameObject.Instantiate(materialResource) as Material;
        }

        //result.SetTexture("_MainTex", LoadTexture(resourceLoader, baseTex));
        result.mainTexture = LoadTexture(resourceLoader, baseTex);
        result.SetFloat("_Smoothness", 0);
        result.SetFloat("_Metallic", .8f);
        result.name = vmtPath;

        if (!string.IsNullOrEmpty(bumpMap))
        {
            result.SetTexture("_BumpMap", LoadTexture(resourceLoader, bumpMap));
        }

        if (transition)
        {
            result.SetTexture("_MainTex2", LoadTexture(resourceLoader, baseTex2));
        }

        if (translucent)
        {
            var baseColor = result.GetColor("_BaseColor");
            baseColor.a = alpha;
            result.SetColor("_BaseColor", baseColor);
            result.SetFloat("_SurfaceType", 1);
        }

        return result;
    }

    private Texture LoadTexture(ResourceLoader resourceLoader, string textureName)
    {
        var resourcePath = GetTexturePath(textureName);
        resourcePath = resourcePath.Replace('\\', '/');

        if (!resourceLoader.ContainsFile(resourcePath))
        {
            return null;
        }

        var textureNameLower = textureName.ToLower();
        if (_textureCache.ContainsKey(textureNameLower))
        {
            return _textureCache[textureNameLower];
        }

        using (var fs = resourceLoader.OpenFile(resourcePath))
        {
            try
            {
                var tex = VtfProvider.GetImage(fs);
                _textureCache.Add(textureNameLower, tex);
                return tex;
            }
            catch (System.Exception e) { Debug.LogError(e); }
        }

        return null;
    }

    private string GetTexturePath(string textureName)
    {
        var path = "materials/" + textureName;
        if (!Path.HasExtension(path))
        {
            return "materials/" + textureName + ".vtf";
        }
        return path;
    }

}
