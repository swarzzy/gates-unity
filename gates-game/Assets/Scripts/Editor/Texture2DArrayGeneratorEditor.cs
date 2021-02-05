using System;
using UnityEditor;
using UnityEngine;

// [https://github.com/XJINE/Unity_Texture2DArrayGenerator]
public class Texture2DArrayGeneratorEditor : EditorWindow
{
    public TextureFormat format = TextureFormat.ARGB32;
    public Texture2D[] textures = new Texture2D[0];
    public string filename = "TextureArray";

    private Vector2 scrollPos;

    [MenuItem("Tools/Texture2D Array Generator")]
    static void Init()
    {
        EditorWindow.GetWindow<Texture2DArrayGeneratorEditor>();
    }

    private void OnGUI()
    {
        GUIStyle marginStyle = GUI.skin.label;
        marginStyle.wordWrap = true;
        marginStyle.margin   = new RectOffset(5, 5, 5, 5);

        filename = EditorGUILayout.TextField(filename);
        format = (TextureFormat)EditorGUILayout.EnumPopup("Format", format);
        int count = EditorGUILayout.IntField("Count", textures.Length);
        if (count != textures.Length)
        {
            var newTextures = new Texture2D[count];
            int copyCount = Math.Min(count, textures.Length);
            for (int i = 0; i < copyCount; i++)
            {
                newTextures[i] = textures[i];
            }

            textures = newTextures;
            scrollPos = Vector2.zero;
        }

        scrollPos = GUILayout.BeginScrollView(scrollPos);
        for (int i = 0; i < textures.Length; i++)
        {
            textures[i] = TextureField("Texture " + i, textures[i]);
        }

        GUILayout.EndScrollView();

        if (GUILayout.Button("Generate"))
        {
            bool allValid = true;
            foreach(var texture in textures)
            {
                if (texture == null)
                {
                    allValid = false;
                    break;
                }
            }

            if (textures.Length == 0)
            {
                allValid = false;
            }

            if (allValid)
            {
                var path = "Assets/" + filename + ".asset";

                Debug.Log(textures.ToString());
                var texture = Texture2DArrayGenerator.Generate(textures, format);
                if (texture != null)
                {
                    AssetDatabase.CreateAsset(texture, path);
                    ShowNotification(new GUIContent("Saved to: " + path));
                }
                else
                {
                    ShowNotification(new GUIContent("Failed to create array. Source textures have different size."));
                }

            }
        }
    }

    private static Texture2D TextureField(string name, Texture2D texture)
    {
        GUILayout.BeginHorizontal();
        var style = new GUIStyle(GUI.skin.label);
        style.alignment = TextAnchor.LowerLeft;
        style.fixedWidth = 70;
        GUILayout.Label(name, style);
        var result = (Texture2D)EditorGUILayout.ObjectField(texture, typeof(Texture2D), false, GUILayout.Width(70), GUILayout.Height(70));
        GUILayout.EndHorizontal();
        return result;
    }
}
