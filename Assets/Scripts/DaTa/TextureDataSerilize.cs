using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine.UIElements;

public static class TextureDataSerializer
{
    public static void SaveTextureData(TextureData textureData, string filePath, Label label)
    {
        try
        {
            TextureDataDTO textureDataDTO = new TextureDataDTO(textureData);
            BinaryFormatter formatter = new BinaryFormatter();
            using (FileStream fileStream = new FileStream(filePath, FileMode.Create))
            {
                formatter.Serialize(fileStream, textureDataDTO);
            }

            Debug.Log("TextureData saved successfully at path: " + filePath);
            label.text = label.text + "\nLưu thành công tại đường dẫn:\n " + filePath;
        }
        catch (IOException ex)
        {
            Debug.LogError("Error saving TextureData: " + ex.Message);

            label.text = label.text + "\nSave failed: " + ex.Message;

        }
    }

    public static TextureData LoadTextureData(string filePath, TextureData defaultTexture, Label label)
    {
        try
        {
            if (File.Exists(filePath))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                using (FileStream fileStream = new FileStream(filePath, FileMode.Open))
                {
                    TextureDataDTO textureDataDTO = (TextureDataDTO)formatter.Deserialize(fileStream);
                    TextureData textureData = textureDataDTO.ToTextureData(defaultTexture);

                    Debug.Log("TextureData loaded successfully from path:" + filePath);
                    label.text = label.text + "\nDữ liệu tải thành công tại:\n" + filePath;
                    return textureData;
                }
            }
            else
            {
                Debug.LogWarning("No saved TextureData found at path: " + filePath);
                label.text = label.text + "\nKhông tìm thấy dữ liệu tại:\n" + filePath;
                return null;
            }
        }
        catch (IOException ex)
        {
            Debug.LogError("Error loading TextureData: " + ex.Message);

            label.text = label.text + "\nLoad failed:\n" + ex.Message;

            return null;
        }
    }
}
