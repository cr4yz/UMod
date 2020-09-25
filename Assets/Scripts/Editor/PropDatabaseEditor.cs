using UnityEngine;
using UnityEditor;
using System.IO;
using System.Linq;
using System.Collections.Generic;

[CustomEditor(typeof(PropDatabase))]
[CanEditMultipleObjects]
public class PropDatabaseEditor : Editor
{
    public override void OnInspectorGUI()
    {
        if(GUILayout.Button("Generate From Resources"))
        {
            Generate();
        }
        GUILayout.Space(24);
        base.OnInspectorGUI();
    }

    private void Generate()
    {
        var propDatabase = base.target as PropDatabase;
        var propsPath = Application.dataPath + "/Resources/Props/";
        var files = Directory.GetFiles(propsPath, "*.*", SearchOption.AllDirectories).Where(x => !x.Contains(".meta"));
        var props = new List<Prop>();
        foreach(var file in files)
        {
            var name = Path.GetFileNameWithoutExtension(file);
            var relativePath = file.Remove(0, propsPath.Length);
            var tags = string.Join(",", relativePath.Split('\\').Reverse().Skip(1).ToArray());
            var objectPath = Path.ChangeExtension("Props\\" + relativePath, null);
            var resource = Resources.Load(objectPath);
            var prop = new Prop()
            {
                Name = name,
                Tags = tags,
                Object = resource
            };
            props.Add(prop);
        }
        propDatabase.Props = props.ToArray();
        serializedObject.ApplyModifiedProperties();
    }

}