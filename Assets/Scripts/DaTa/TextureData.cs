using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
[CreateAssetMenu()]
public class TextureData : UpdateData
{
    const int textureSize = 512;
    const TextureFormat textureFormat = TextureFormat.RGB565;
    public Layer[] layers;
    // public Color[] baseColor;
    // [Range(0,1)]
    // public float[] baseStartHeight;
    // [Range(0,1)]
    // public float[] baseBlends;

    float savedMinHeight;
    float savedMaxHeight;
    /*public void ApplytoMaterial(Material material)
    {

        // gán cho các biến bên script bên shader
        material.SetInt("layerCount", layers.Length); //đếm xem có bao nhiêu màu thì size của hai mảng kia to bấy nhiêu
        material.SetColorArray("baseColor", layers.Select(x => x.tint).ToArray());
        material.SetFloatArray("baseStartHeight", layers.Select(x => x.startHeight).ToArray());
        material.SetFloatArray("baseBlends", layers.Select(x => x.blendStr).ToArray());
        material.SetFloatArray("baseColorStr", layers.Select(x => x.tintStr).ToArray());
        material.SetFloatArray("baseTextureScale", layers.Select(x => x.textureScale).ToArray());
        material.SetFloatArray("baseTextureActive", layers.Select(x => x.active).ToArray());
        Texture2DArray textureArray = GenerateTextureArray(layers.Select(x => x.texture).ToArray());
        material.SetTexture("baseTexture", textureArray);

        UpdateMeshHeight(material, savedMinHeight, savedMaxHeight);
    }*/
    //Lưu lại height tránh bị trôi
    public void UpdateMeshHeight(Material material, float minHeight, float maxHeight)
    {
        savedMaxHeight = maxHeight;
        savedMinHeight = minHeight;
        material.SetFloat("minHeight", minHeight);
        material.SetFloat("maxHeight", maxHeight);
    }

    public void CopyFrom(TextureData other)
    {
        layers = new Layer[other.layers.Length];
        for (int i = 0; i < layers.Length; i++)
        {
            layers[i] = new Layer();
            //layers[i].texture = other.layers[i].texture;
            layers[i].tint = other.layers[i].tint;
            //layers[i].tintStr = other.layers[i].tintStr;
            layers[i].startHeight = other.layers[i].startHeight;
            //layers[i].blendStr = other.layers[i].blendStr;
            //layers[i].textureScale = other.layers[i].textureScale;
            layers[i].active = other.layers[i].active;
        }
        savedMinHeight = other.savedMinHeight;
        savedMaxHeight = other.savedMaxHeight;
    }

    // tạo mảng texture
    Texture2DArray GenerateTextureArray(Texture2D[] textures)
    {
        Texture2DArray textureArray = new Texture2DArray(textureSize, textureSize, textures.Length, textureFormat, true);
        for (int i = 0; i < textures.Length; i++)
        {
            textureArray.SetPixels(textures[i].GetPixels(), i);

        }
        textureArray.Apply();
        return textureArray;
    }
    [System.Serializable]
    public class Layer
    {
        //public Texture2D texture;
        public Color tint;
        //[Range(0, 1)]
        //public float tintStr;
        [Range(0, 1)]
        public float startHeight;
        //[Range(0, 1)]
        //public float blendStr;
        //public float textureScale;

        public float active;

    }
}
