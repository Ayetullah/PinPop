using System.IO;
using PinPop;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

// Render the actual pin mesh and printed coat, so collection keeps the same identity.
public static class PinIconBaker
{
    [MenuItem("PinPop/Bake collection pin icons")]
    public static void Bake()
    {
        const string folder="Assets/Resources/PinIcons";
        bool complete=true;for(int color=0;color<PinPopPalette.Count;color++)complete &= File.Exists(folder+"/pin-"+color+".png");if(complete)return;
        Directory.CreateDirectory(folder);
        var previousAmbient=RenderSettings.ambientLight;var previousMode=RenderSettings.ambientMode;
        var stage=new GameObject("Pin icon photo studio"){hideFlags=HideFlags.HideAndDontSave};stage.transform.position=new Vector3(1000,-1000,0);
        var cameraObject=new GameObject("Icon camera");cameraObject.transform.SetParent(stage.transform,false);cameraObject.transform.localPosition=new Vector3(0,.79f,-4);
        var camera=cameraObject.AddComponent<Camera>();camera.enabled=false;camera.orthographic=true;camera.orthographicSize=.83f;camera.clearFlags=CameraClearFlags.SolidColor;camera.backgroundColor=Color.clear;camera.cullingMask=1<<31;camera.nearClipPlane=.1f;camera.farClipPlane=10;camera.allowHDR=false;camera.allowMSAA=true;
        var lightObject=new GameObject("Icon key light");lightObject.transform.SetParent(stage.transform,false);lightObject.transform.localRotation=Quaternion.Euler(35,-25,0);var light=lightObject.AddComponent<Light>();light.type=LightType.Directional;light.intensity=1.15f;light.cullingMask=1<<31;
        var pin=new GameObject("Photographed bowling pin");pin.layer=31;pin.transform.SetParent(stage.transform,false);pin.AddComponent<MeshFilter>().sharedMesh=Art.PinMesh();var skin=pin.AddComponent<MeshRenderer>();skin.shadowCastingMode=ShadowCastingMode.Off;skin.receiveShadows=false;
        var target=new RenderTexture(192,256,24,RenderTextureFormat.ARGB32,RenderTextureReadWrite.sRGB){antiAliasing=4};camera.targetTexture=target;
        var previous=RenderTexture.active;
        try {
            RenderSettings.ambientMode=AmbientMode.Flat;RenderSettings.ambientLight=new Color(.65f,.65f,.65f);
            for(int color=0;color<PinPopPalette.Count;color++){
                skin.sharedMaterial=Art.PinCoat(color);camera.Render();RenderTexture.active=target;
                var texture=new Texture2D(192,256,TextureFormat.RGBA32,false);texture.ReadPixels(new Rect(0,0,192,256),0,0);texture.Apply();File.WriteAllBytes(folder+"/pin-"+color+".png",texture.EncodeToPNG());Object.DestroyImmediate(texture);
            }
        } finally {RenderTexture.active=previous;camera.targetTexture=null;target.Release();Object.DestroyImmediate(target);Object.DestroyImmediate(stage);RenderSettings.ambientMode=previousMode;RenderSettings.ambientLight=previousAmbient;}
        AssetDatabase.Refresh();
        for(int color=0;color<PinPopPalette.Count;color++){
            var importer=(TextureImporter)AssetImporter.GetAtPath(folder+"/pin-"+color+".png");importer.textureType=TextureImporterType.Sprite;importer.spriteImportMode=SpriteImportMode.Single;importer.spritePixelsPerUnit=100;importer.alphaIsTransparency=true;importer.mipmapEnabled=false;importer.textureCompression=TextureImporterCompression.Uncompressed;importer.SaveAndReimport();
        }
        Debug.Log("PINPOP: Actual bowling pin icons baked.");
    }
}
