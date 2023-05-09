using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public static class TextureDataSerializer
{
    public static void SaveTextureData(TextureData textureData, string filePath)
    {
        TextureDataDTO textureDataDTO = new TextureDataDTO(textureData);
        BinaryFormatter formatter = new BinaryFormatter();
        using (FileStream fileStream = new FileStream(filePath, FileMode.Create))
        {
            formatter.Serialize(fileStream, textureDataDTO);
        }

        Debug.Log("TextureData saved successfully at path: " + filePath);
    }

    public static TextureData LoadTextureData(string filePath, TextureData defaultTexture)
    {
        if (File.Exists(filePath))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            using (FileStream fileStream = new FileStream(filePath, FileMode.Open))
            {
                TextureDataDTO textureDataDTO = (TextureDataDTO)formatter.Deserialize(fileStream);
                TextureData textureData = textureDataDTO.ToTextureData(defaultTexture);

                Debug.Log("TextureData loaded successfully from path: " + filePath);
                return textureData;
            }
        }
        else
        {
            Debug.LogWarning("No saved TextureData found at path: " + filePath);
            return null;
        }
    }
}
