using UnityEngine;
using UnityEditor;
using System.IO;

public class SimpleVATBaker : EditorWindow
{
    GameObject targetObject;
    AnimationClip animationClip;
    int frameCount = 30;

    [MenuItem("Tools/VAT Baker V4 (Normals + Color)")]
    public static void ShowWindow()
    {
        GetWindow<SimpleVATBaker>("VAT Baker");
    }

    void OnGUI()
    {
        GUILayout.Label("VAT Baker - MWA", EditorStyles.boldLabel);
        targetObject = (GameObject)EditorGUILayout.ObjectField("Model", targetObject, typeof(GameObject), false);
        animationClip = (AnimationClip)EditorGUILayout.ObjectField("Animation", animationClip, typeof(AnimationClip), false);
        frameCount = EditorGUILayout.IntField("Frame Count", frameCount);

        if (GUILayout.Button("Bake") && targetObject != null && animationClip != null)
        {
            BakeVAT();
        }
    }

    void BakeVAT()
    {
        GameObject instance = Instantiate(targetObject);
        SkinnedMeshRenderer smr = instance.GetComponentInChildren<SkinnedMeshRenderer>();

        if (smr == null) { Debug.LogError("No SkinnedMeshRenderer!"); DestroyImmediate(instance); return; }

        Mesh originalMesh = smr.sharedMesh;
        int vertexCount = originalMesh.vertexCount;

        Texture2D posTex = new Texture2D(vertexCount, frameCount, TextureFormat.RGBAFloat, false);
        posTex.filterMode = FilterMode.Point;
        posTex.wrapMode = TextureWrapMode.Clamp;


        Texture2D normTex = new Texture2D(vertexCount, frameCount, TextureFormat.RGBAFloat, false);
        normTex.filterMode = FilterMode.Point;
        normTex.wrapMode = TextureWrapMode.Clamp;

        Mesh tempMesh = new Mesh();

        for (int i = 0; i < frameCount; i++)
        {
            float normalizedTime = (float)i / (frameCount - 1);
            animationClip.SampleAnimation(instance, normalizedTime * animationClip.length);
            smr.BakeMesh(tempMesh);

            Vector3[] vertices = tempMesh.vertices;
            Vector3[] normals = tempMesh.normals;

            for (int v = 0; v < vertexCount && v < vertices.Length; v++)
            {
                posTex.SetPixel(v, i, new Color(vertices[v].x, vertices[v].y, vertices[v].z, 1f));

                float r = normals[v].x * 0.5f + 0.5f;
                float g = normals[v].y * 0.5f + 0.5f;
                float b = normals[v].z * 0.5f + 0.5f;
                normTex.SetPixel(v, i, new Color(r, g, b, 1f));
            }
        }
        posTex.Apply();
        normTex.Apply();

        string path = "Assets/MWA_VATOutput_";
        if (!Directory.Exists(path)) Directory.CreateDirectory(path);

        SaveTexture(posTex, path + "/PositionTexture.exr");
        SaveTexture(normTex, path + "/NormalTexture.exr");

        Mesh finalMesh = Instantiate(originalMesh);
        Vector2[] uv2 = new Vector2[vertexCount];
        for (int i = 0; i < vertexCount; i++)
        {
            uv2[i] = new Vector2((i + 0.5f) / vertexCount, 0);
        }
        finalMesh.uv2 = uv2;

        AssetDatabase.CreateAsset(finalMesh, path + "/BakedMesh.asset");
        AssetDatabase.Refresh();

        SetupImporter(path + "/PositionTexture.exr");
        SetupImporter(path + "/NormalTexture.exr");

        DestroyImmediate(instance);
        DestroyImmediate(tempMesh);
        Debug.Log("BAKE Done: Assets/MWA_VATOutput_");
    }

    void SaveTexture(Texture2D tex, string path)
    {
        byte[] bytes = tex.EncodeToEXR(Texture2D.EXRFlags.None);
        File.WriteAllBytes(path, bytes);
    }

    void SetupImporter(string path)
    {
        TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
        if (importer != null)
        {
            importer.sRGBTexture = false;
            importer.filterMode = FilterMode.Point;
            importer.mipmapEnabled = false;
            importer.npotScale = TextureImporterNPOTScale.None;
            importer.maxTextureSize = 8192;
            importer.textureCompression = TextureImporterCompression.Uncompressed;
            importer.SaveAndReimport();
        }
    }
}