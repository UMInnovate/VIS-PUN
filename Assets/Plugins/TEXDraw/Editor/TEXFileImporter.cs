using UnityEngine;
using UnityEditor.Experimental.AssetImporters;
using System.IO;

[ScriptedImporter(1, "tex")]
public class TEXFileImporter : ScriptedImporter
{
    public override void OnImportAsset(AssetImportContext ctx)
    {
        var text = new TextAsset(File.ReadAllText(ctx.assetPath));
        ctx.AddObjectToAsset("text", text);
        ctx.SetMainObject(text);
    }
}