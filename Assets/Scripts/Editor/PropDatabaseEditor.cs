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
        var propsArray = serializedObject.FindProperty("Props");
        var propsPath = Application.dataPath + "/Resources/Props/";
        var files = Directory.GetFiles(propsPath, "*.*", SearchOption.AllDirectories).Where(x => !x.Contains(".meta"));
        var props = new List<Prop>();
        var idx = 0;
        propsArray.arraySize = files.Count();
        foreach(var file in files)
        {
            var name = Path.GetFileNameWithoutExtension(file);
            var relativePath = file.Remove(0, propsPath.Length);
            var tags = string.Join(",", relativePath.Split('\\').Reverse().Skip(1).ToArray());
            var objectPath = Path.ChangeExtension("Props\\" + relativePath, null);
            var resource = Resources.Load(objectPath);
            var el = propsArray.GetArrayElementAtIndex(idx);
            el.FindPropertyRelative("Name").stringValue = name;
            el.FindPropertyRelative("Tags").stringValue = tags;
            el.FindPropertyRelative("Object").objectReferenceValue = resource;
            idx++;
        }
        serializedObject.ApplyModifiedProperties();
    }

}