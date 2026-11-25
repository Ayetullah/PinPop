using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

// These shaders are looked up by name when the procedural scene is created.
// Scene dependency scanning cannot discover those runtime references.
public sealed class RuntimeShaderBuild : IPreprocessBuildWithReport
{
    public int callbackOrder => 0;
    static readonly string[] Names = {
        "Hidden/PinPop/HtmlLook", "PinPop/HTML Surface",
        "PinPop/HTML Transparent", "PinPop/HTML Unlit",
        "Standard", "Sprites/Default"
    };

    public void OnPreprocessBuild(BuildReport report) => EnsureIncluded();

    public static void EnsureIncluded()
    {
        var settings = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/GraphicsSettings.asset")[0]);
        var shaders = settings.FindProperty("m_AlwaysIncludedShaders");
        foreach (var name in Names)
        {
            var shader = Shader.Find(name);
            if (shader == null) throw new BuildFailedException("Missing runtime shader: " + name);
            bool found = false;
            for (int i = 0; i < shaders.arraySize; i++)
                if (shaders.GetArrayElementAtIndex(i).objectReferenceValue == shader) { found = true; break; }
            if (found) continue;
            int index = shaders.arraySize;
            shaders.InsertArrayElementAtIndex(index);
            shaders.GetArrayElementAtIndex(index).objectReferenceValue = shader;
        }
        settings.ApplyModifiedPropertiesWithoutUndo();
        AssetDatabase.SaveAssets();
        Debug.Log("PINPOP: All runtime shaders retained for player build.");
    }
}
