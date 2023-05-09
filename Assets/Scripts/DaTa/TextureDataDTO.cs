using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

[Serializable]
public class TextureDataDTO
{
    public LayerDTO[] layers;

    public TextureDataDTO(TextureData textureData)
    {
        layers = textureData.layers.Select(layer => new LayerDTO(layer)).ToArray();
    }

    public TextureData ToTextureData(TextureData defaultTextureData)
    {
        TextureData textureData = ScriptableObject.CreateInstance<TextureData>();
        textureData.layers = layers.Select(layerDTO => layerDTO.ToLayer()).ToArray();

        for (int i = 0; i < 7; i++){
            if (textureData.layers[i].texture == null)
                textureData.layers[i].texture = defaultTextureData.layers[i].texture;
        }
            return textureData;
    }
}

[Serializable]
public class LayerDTO
{
    //public Texture2D texture;
    public ColorData tint;
    public float tintStr;
    public float startHeight;
    public float blendStr;
    public float textureScale;
    public float active;

    public LayerDTO(TextureData.Layer layer)
    {
        //texture = layer.texture;
        tint = new ColorData(layer.tint);
        tintStr = layer.tintStr;
        startHeight = layer.startHeight;
        blendStr = layer.blendStr;
        textureScale = layer.textureScale;
        active = layer.active;
    }

    public TextureData.Layer ToLayer()
    {
        TextureData.Layer layer = new TextureData.Layer();
        //layer.texture = texture;
        layer.tint = tint.ToColor();
        layer.tintStr = tintStr;
        layer.startHeight = startHeight;
        layer.blendStr = blendStr;
        layer.textureScale = textureScale;
        layer.active = active;
        return layer;
    }
}
[Serializable]
public class ColorData
{
    public float r;
    public float g;
    public float b;
    public float a;

    public ColorData(Color color)
    {
        r = color.r;
        g = color.g;
        b = color.b;
        a = color.a;
    }

    public Color ToColor()
    {
        return new Color(r, g, b, a);
    }
}
