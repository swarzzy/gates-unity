using System.Collections.Generic;
using UnityEngine;

// [https://github.com/XJINE/Unity_Texture2DArrayGenerator]
public static class Texture2DArrayGenerator
{
    public static Texture2DArray Generate(IList<Texture2D> textures, TextureFormat format)
    {
        if (textures.Count == 0)
        {
            return null;
        }

        int width = textures[0].width;
        int height = textures[0].height;

        foreach (var texture in textures)
        {
            if (texture.height != height || texture.width != width)
            {
                return null;
            }
        }

        Texture2DArray texture2DArray = new Texture2DArray(textures[0].width, textures[0].height, textures.Count, format, false);

        for (int i = 0; i < textures.Count; i++)
        {
            // NOTE:
            // It is able to make a Texture2DArray with "Graphics.CopyTexture()".
            // However, it has a problem which is able to make Texture2DArray in Editor
            // without enabling read-write settings of texture.
            // And then, it causes some wrong result in build app.
            // So we should make a Texture2DArray with "SetPixels()".
            //
            // Graphics.CopyTexture(textures[i], 0, 0, texture2DArray, i, 0);
            var texture = textures[i];
            Debug.Assert(texture != null);
            if (texture == null) break;

            texture2DArray.SetPixels(textures[i].GetPixels(), i);
        }

        texture2DArray.Apply();
        return texture2DArray;
    }
}