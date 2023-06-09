using UnityEngine;
using UnityEngine.UIElements;
using System;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;
using System.Threading;

using System.Collections;

public class MenuController : MonoBehaviour
{
    [SerializeField]
    UIDocument mainMenuDocument;
    public HeightMapSettings heightMapSettings;
    public MeshSettings meshSettings;
    public TextureData textureSettings;
    public GameObject meshGameObject;
    public Material meshMaterial;
    [SerializeField] public GameObject canvasButton;

    MeshRenderer meshRenderer;
    MapDisplay mapGen;
    Spining spin;

    //---

    //Element cho Noise
    public ScrollView _scrollviewHolder;
    public TextField _scaleInput;
    public TextField _octavesInput;
    public Slider _persistenceInput;
    public TextField _lacunarityInput;
    public TextField _seedInput;
    public TextField _heightMultiInput;

    public Button _saveNoise;
    public Button _loadNoise;
    public Label _assetNoise;

    public VisualElement saveNoiseElement;
    public VisualElement saveHolderNoiseElement;
    public VisualElement saveButtonHolderNoiseElement;
    public TextField _saveNoiseNameInput;
    public Button _cancelNoiseButton;
    public Button _rsaveNoise;

    public VisualElement loadNoiseElement;
    public VisualElement loadHolderNoiseElement;
    public VisualElement loadButtonHolderNoiseElement;
    public DropdownField _loadNoiseNameInput;
    public Button _cancelLoadNoiseButton;
    public Button _rloadNoise;
    //Elemnt cho Mesh
    public SliderInt _lodInput;
    public Toggle _useFallOff;
    public Slider _spinInput;
    public SliderInt _chunkSizeInput;

    //Biến hệ thống
    public DropdownField _drawMode;
    // public GameObject noisePlane;
    // public GameObject meshPlane;
    public Button _exitButton;
    public Button _saveMeshButton;
    public Button _saveMeshpopupButton;
    public Button _cancelMeshpopupButton;
    public Button _showLogButton;
    public TextField _meshnameInput;
    public Label _logLabel;
    public VisualElement _logContainer;
    private bool _islog = false;
    public VisualElement _mainContainer;
    public VisualElement _mainContainer1;
    public ProgressBar _saveProgress;


    // Biến cho texture
    public VisualElement childElement;

    public Button _saveTexture;
    public Button _loadTexture;
    public Label _assetTexture;

    public VisualElement saveTextureElement;
    public TextField _saveTextureNameInput;
    public Button _cancelTextureButton;
    public Button _rsaveTexture;
    public VisualElement saveHolderTextureElement;
    public VisualElement saveButtonHolderTextureElement;

    public VisualElement loadTextureElement;
    public DropdownField _loadTextureNameInput;
    public Button _cancelLoadTextureButton;
    public Button _rloadTexture;
    public VisualElement loadHolderTextureElement;
    public VisualElement loadButtonHolderTextureElement;

    public VisualElement[] subElement = new VisualElement[7];
    public VisualElement[] childsElement = new VisualElement[7];
    //public Slider[] _sliderTintStr = new Slider[7];
    public Slider[] _sliderStartHeight = new Slider[7];
    //public Slider[] _sliderBlend = new Slider[7];
    //public TextField[] _textureStrInput = new TextField[7];
    public Toggle[] _activeToggle = new Toggle[7];
    public Button[] _colorButton = new Button[7];
    public Button[] _setColorButton = new Button[7];
    public FlexibleColorPicker _fcp;
    public VisualElement _textureTitle;
    //---

    //Xử lý callback các biến texture
    void RegisterSliderArrayCallbacks(Slider[] sliderArray)
    {
        for (int i = 0; i < sliderArray.Length; i++)
        {
            Slider slider = sliderArray[i];

            slider.RegisterValueChangedCallback(evt => OnSliderValueChanged(slider, evt, sliderArray));
        }
    }
    /*void RegisterFieldArrayCallbacks(TextField[] fieldArray)
    {
        for (int i = 0; i < fieldArray.Length; i++)
        {
            TextField field = fieldArray[i];

            field.RegisterValueChangedCallback(evt => OnFieldArrayValueChanged(field, evt, fieldArray));
        }
    }*/

    void RegisterToggleArrayCallbacks(Toggle[] toggleArray)
    {
        for (int i = 0; i < toggleArray.Length; i++)
        {
            Toggle toggle = toggleArray[i];

            toggle.RegisterValueChangedCallback(evt => OnArrayToggleValueChanged(toggle, evt, toggleArray));
        }
    }

    void RegisterButtonArrayCallbacks(Button[] buttonArray, Button[] buttonArray2)
    {
        int activeButtonIndex = -1;

        for (int i = 0; i < buttonArray.Length; i++)
        {
            int buttonIndex = i;
            Button button = buttonArray[i];
            Button button2 = buttonArray2[i];
            button.clickable.clicked += () =>
            {
                if (activeButtonIndex == buttonIndex) // already active, do nothing
                {
                    return;
                }
                // hide the currently active button (if any)
                if (activeButtonIndex >= 0 && activeButtonIndex < buttonArray.Length)
                {
                    _setColorButton[activeButtonIndex].style.display = DisplayStyle.None;
                }
                int index = Array.IndexOf(buttonArray, button);
                string buttonName = button.name; // Get the name of the button that was clicked
                int indexactive = int.Parse(buttonName.Replace("color", "")); // Parse the index from the button name
                _setColorButton[indexactive].style.display = DisplayStyle.Flex;

                //Debug.Log("Buttton at " + index + "and indexactive: " + indexactive);
                _fcp.gameObject.SetActive(true);
                _fcp.color = textureSettings.layers[index].tint;
                // update the active button index
                activeButtonIndex = buttonIndex;
            };
            button2.clickable.clicked += () =>
            {
                int index = Array.IndexOf(buttonArray2, button2);
                string buttonName = button2.name; // Get the name of the button that was clicked
                int indexactive = int.Parse(buttonName.Replace("saveColor", "")); // Parse the index from the button name
                //Debug.Log("sub Buttton at " + index);
                textureSettings.layers[index].tint = _fcp.color;
                Color newColor = new Color(textureSettings.layers[index].tint.r, textureSettings.layers[index].tint.g, textureSettings.layers[index].tint.b, 1.0f);
                //Debug.Log("color" + textureSettings.layers[i].tint);
                _colorButton[index].style.backgroundColor = new StyleColor(newColor);
                _fcp.gameObject.SetActive(false);
                _setColorButton[indexactive].style.display = DisplayStyle.None;
                activeButtonIndex = -1;
                mapGen = GameObject.FindGameObjectWithTag("MapDisplay").GetComponent<MapDisplay>();
                mapGen.DrawMapInRuntime(getMode());

            };
        }

    }
    //---

    //Xử lý sự kiện Button
    void OnButtonClicked()
    {
#if UNITY_EDITOR
        // If we're in the Unity Editor, stop playing the game
        UnityEditor.EditorApplication.isPlaying = false;
#else
        // If we're running the game in a standalone build, quit the application
        Application.Quit();
#endif
    }
    void OnMeshPopup()
    {
        _saveMeshButton.style.display = DisplayStyle.Flex;
        _cancelMeshpopupButton.style.display = DisplayStyle.Flex;
        _meshnameInput.style.display = DisplayStyle.Flex;
    }
    void OnCancelPopup()
    {
        OnCancelExport();
        _saveMeshButton.style.display = DisplayStyle.None;
        _cancelMeshpopupButton.style.display = DisplayStyle.None;
        _meshnameInput.style.display = DisplayStyle.None;
    }
    void OnShowlog()
    {
        _islog = !_islog;
        _logContainer.style.display = _islog ? DisplayStyle.Flex : DisplayStyle.None;
    }
    private CancellationTokenSource cancellationTokenSource;
    void OnCancelExport()
    {
        // Cancel the mesh export if it is running
        if (cancellationTokenSource != null)
        {
            cancellationTokenSource.Cancel();
        }
    }
    async void OnMeshSave()
    {
        try
        {
            _saveMeshButton.SetEnabled(false);
            //_cancelMeshpopupButton.SetEnabled(false);

            MeshFilter targetMeshFilter = meshGameObject.GetComponent<MeshFilter>();
            string text = _meshnameInput.text;
            if (string.IsNullOrEmpty(text))
            {
                // Generate a random three-character string
                text = GenerateRandomString(3);
            }
            // Remove spaces and special characters
            text = RemoveSpacesAndSpecialCharacters(text);
            // Remove spaces and special characters
            text = RemoveSpacesAndSpecialCharacters(text);
            bool isReserved = IsReservedName(text);
            if (isReserved)
            {
                _logLabel.text = _logLabel.text + "\nThe entered file name is a reserved name. Please choose a different name.";
                return;
            }

            if (targetMeshFilter != null && targetMeshFilter.sharedMesh != null)
            {
                Mesh mesh = targetMeshFilter.sharedMesh;
                string filePath = Path.Combine(Application.persistentDataPath, text + "mesh.obj"); // Define the file path for the exported mesh

                if (File.Exists(filePath))
                {
                    Debug.LogError("File already exists: " + filePath);
                    // Show an error message to the user
                    // You can update the UI or display a popup to inform the user about the existing file
                    _logLabel.text = _logLabel.text + "\n Lỗi: File đã tồn tại";
                    _saveMeshButton.SetEnabled(true);
                    return; // Don't proceed with the export
                }

                mapGen = GameObject.FindGameObjectWithTag("MapDisplay").GetComponent<MapDisplay>();

                // Create a copy of the texture to be exported
                Texture2D originalTexture = mapGen.texturesave;
                Texture2D exportedTexture = new Texture2D(originalTexture.width, originalTexture.height);
                exportedTexture.SetPixels(originalTexture.GetPixels());
                exportedTexture.Apply();
                //string objData = ObjExporter.MeshToObj(mesh);
                //await ObjExporter.SaveMeshAsync(mesh, filePath);
                cancellationTokenSource = new CancellationTokenSource();
                CancellationToken cancellationToken = cancellationTokenSource.Token;
                Task exportTask = ExportMeshAsync(mesh, filePath, cancellationToken);
                while (!exportTask.IsCompleted)
                {
                    // Allow the UI to remain responsive
                    await Task.Yield();
                }

                // Check if the task completed successfully or was cancelled
                if (exportTask.IsCanceled)
                {
                    Debug.Log("Mesh export was cancelled.");
                    _saveMeshButton.style.display = DisplayStyle.None;
                    _saveMeshButton.SetEnabled(true);
                    //_cancelMeshpopupButton.SetEnabled(true);
                    _cancelMeshpopupButton.style.display = DisplayStyle.None;
                    _meshnameInput.style.display = DisplayStyle.None;
                    _logLabel.text = _logLabel.text + "\n Đã hủy lưu";
                    // Handle cancellation appropriately (e.g., update UI, show message)
                }
                else if (exportTask.IsFaulted)
                {
                    // Handle any errors that occurred during export
                    Debug.LogError("Error exporting mesh: " + exportTask.Exception);
                    _logLabel.text = _logLabel.text + "\n Lỗi:\n" + exportTask.Exception.Message;
                }
                else
                {
                    // Export material color
                    // Encode the Texture2D as a PNG file
                    byte[] pngBytes = exportedTexture.EncodeToPNG();
                    string filepngPath = Path.Combine(Application.persistentDataPath, text + "texture.png");
                    await WriteBytesToFileAsync(filepngPath, pngBytes);

                    Debug.Log("Mesh exported successfully to: " + filePath);
                    _logLabel.text = _logLabel.text + "\n Xuất thành công tới:\n" + Application.persistentDataPath + "/" + text + "mesh.obj";
                    _saveMeshButton.style.display = DisplayStyle.None;
                    _saveMeshButton.SetEnabled(true);
                    //_cancelMeshpopupButton.SetEnabled(true);
                    _cancelMeshpopupButton.style.display = DisplayStyle.None;
                    _meshnameInput.style.display = DisplayStyle.None;
                }
                // // Export material color
                // // Encode the Texture2D as a PNG file
                // byte[] pngBytes = exportedTexture.EncodeToPNG();
                // string filepngPath = Path.Combine(Application.persistentDataPath, text + "texture.png");

                // await WriteBytesToFileAsync(filepngPath, pngBytes);


                // Debug.Log("Mesh exported successfully to: " + filePath);
                // _logLabel.text = _logLabel.text + "\n Xuất thành công tới:\n" + Application.persistentDataPath + "/" + text + "mesh.obj";
                // _saveMeshButton.style.display = DisplayStyle.None;
                // _saveMeshButton.SetEnabled(true);
                // _cancelMeshpopupButton.SetEnabled(true);
                // _cancelMeshpopupButton.style.display = DisplayStyle.None;
                // _meshnameInput.style.display = DisplayStyle.None;
            }
            else
            {
                Debug.LogError("Failed to export mesh. Mesh filter or shared mesh is null.");
                _logLabel.text = _logLabel.text + "\n Lỗi Null";
            }
        }
        catch (Exception ex)
        {
            Debug.LogError("Error exporting mesh: " + ex.Message);
            _logLabel.text = _logLabel.text + "\n Lỗi:\n" + ex.Message;
        }
    }
    // Async file writing method for text
    private static async Task WriteTextToFileAsync(string filePath, string text)
    {
        using (StreamWriter writer = new StreamWriter(filePath))
        {
            await writer.WriteAsync(text);
        }
    }
    private async Task ExportMeshAsync(Mesh mesh, string filePath, CancellationToken cancellationToken)
    {
        ObjExporter.OnProgress += ExportProgressHandler; // Subscribe to the progress event
        _saveProgress.style.display = DisplayStyle.Flex;

        try
        {
            await ObjExporter.SaveMeshAsync(mesh, filePath, cancellationToken); // Start the export process
        }
        finally
        {
            ObjExporter.OnProgress -= ExportProgressHandler; // Unsubscribe from the progress event
            _saveProgress.style.display = DisplayStyle.None;
            _saveProgress.value = 0f;
            lastProgressUpdate = 0f;
        }

    }
    private float lastProgressUpdate = 0f;
    private const float ProgressUpdateInterval = 0.05f;
    private void ExportProgressHandler(float progress)
    {
        // Update your user interface with the progress value (e.g., progress bar)
        float scaleProgress = progress * 100f;
        if (progress - lastProgressUpdate >= ProgressUpdateInterval)
        {
            lastProgressUpdate = progress;
            Debug.Log("Progress is" + progress);
            // Update your UI elements with the progress value here
            // This could be updating a progress bar, text display, or any other UI element
            _saveProgress.value = (float)Math.Round(progress * 100, 2);
        }

    }


    // Async file writing method for bytes
    private static async Task WriteBytesToFileAsync(string filePath, byte[] bytes)
    {
        using (FileStream fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None, bufferSize: 4096, useAsync: true))
        {
            await fileStream.WriteAsync(bytes, 0, bytes.Length);
        }
    }
    void OnNoiseSaveButton()
    {
        saveNoiseElement.style.display = DisplayStyle.Flex;
    }
    void OnNoiseCancelButton()
    {
        saveNoiseElement.style.display = DisplayStyle.None;
    }
    void OnNoiseRSaveButton()
    {
        // Create a new instance of the asset
        //HeightMapSettings newSettings = Instantiate(heightMapSettings);
        HeightMapSettingsDTO settingsDTO = new HeightMapSettingsDTO();
        settingsDTO.minHeight = heightMapSettings.minHeight;
        settingsDTO.maxHeight = heightMapSettings.maxHeight;
        settingsDTO.HeightMultiplier = heightMapSettings.HeightMultiplier;

        // Serialize AnimationCurve
        AnimationCurveData curveData = new AnimationCurveData(heightMapSettings.HeightCurve);
        settingsDTO.HeightCurve = curveData;

        // Serialize Vector2 fields
        settingsDTO.vectorData2 = heightMapSettings.noiseSettings.offset;

        // Serialize NoiseSettings
        NoiseSettingsDTO noiseSettingsDTO = new NoiseSettingsDTO();
        //noiseSettingsDTO.normallizeMode = (int)heightMapSettings.noiseSettings.normallizeMode;
        noiseSettingsDTO.scale = heightMapSettings.noiseSettings.scale;
        noiseSettingsDTO.octaves = heightMapSettings.noiseSettings.octaves;
        noiseSettingsDTO.persistence = heightMapSettings.noiseSettings.persistence;
        noiseSettingsDTO.lacunarity = heightMapSettings.noiseSettings.lacunarity;
        noiseSettingsDTO.seed = heightMapSettings.noiseSettings.seed;
        noiseSettingsDTO.offset = heightMapSettings.noiseSettings.offset;
        noiseSettingsDTO.ValidateValues();
        settingsDTO.noiseSettings = noiseSettingsDTO;

        string text = _saveNoiseNameInput.text;
        if (string.IsNullOrEmpty(text))
        {
            // Generate a random three-character string
            text = GenerateRandomString(3);
        }

        // Remove spaces and special characters
        text = RemoveSpacesAndSpecialCharacters(text);
        text = RemoveSpacesAndSpecialCharacters(text);
        bool isReserved = IsReservedName(text);
        if (isReserved)
        {
            _logLabel.text = _logLabel.text + "\nThe entered file name is a reserved name. Please choose a different name.";
            return;
        }
        // Modify the values of the new instance as needed

        // Save the modified asset as a binary file
        string filePath = Application.persistentDataPath + "/" + text + "noise" + ".dat";
        if (File.Exists(filePath))
        {
            Debug.LogWarning("A file already exists at path: " + filePath + ". Deleting the existing file.");
            File.Delete(filePath);
        }
        try
        {
            using (FileStream fileStream = new FileStream(filePath, FileMode.Create))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(fileStream, settingsDTO); // Serialize the newSettings object
            }

            Debug.Log("Asset saved successfully at path: " + filePath);
            _logLabel.text = _logLabel.text + "\n Lưu thành công tại đường dẫn:\n" + filePath;
            PopulateDropdown("noise", _loadNoiseNameInput);
            saveNoiseElement.style.display = DisplayStyle.None;
        }
        catch (IOException e)
        {
            Debug.LogError("Failed to save asset: " + e.Message);
            _logLabel.text = _logLabel.text + "\n Lỗi:\n" + e.Message;
        }
    }
    void OnNoiseLoadButton()
    {
        loadNoiseElement.style.display = DisplayStyle.Flex;
    }
    void OnNoiseLoadCancelButton()
    {
        loadNoiseElement.style.display = DisplayStyle.None;
    }

    void OnNoiseRLoadButton()
    {
        string name = _loadNoiseNameInput.value;
        string filePath = Application.persistentDataPath + "/" + name + ".dat";

        if (File.Exists(filePath))
        {
            try
            {
                using (FileStream fileStream = new FileStream(filePath, FileMode.Open))
                {
                    BinaryFormatter formatter = new BinaryFormatter();

                    // Deserialize the data
                    HeightMapSettingsDTO loadedDTO = (HeightMapSettingsDTO)formatter.Deserialize(fileStream);

                    // Convert the DTO to the appropriate class or struct instance
                    HeightMapSettings loadedSettings;


                    loadedSettings = ScriptableObject.CreateInstance<HeightMapSettings>();
                    loadedSettings.noiseSettings = loadedDTO.noiseSettings.ToNoiseSettings();
                    loadedSettings.useFallOff = loadedDTO.useFallOff;
                    loadedSettings.HeightMultiplier = loadedDTO.HeightMultiplier;
                    loadedSettings.HeightCurve = loadedDTO.HeightCurve.ToAnimationCurve();

                    // Assign the loaded settings to your desired class or struct instance
                    // For example:
                    mapGen = GameObject.FindGameObjectWithTag("MapDisplay").GetComponent<MapDisplay>();
                    //mapGen.heightMapSettings = ScriptableObject.CreateInstance<HeightMapSettings>();
                    mapGen.heightMapSettings.CopyFrom(loadedSettings);
                    generatedValue();
                    mapGen.DrawMapInRuntime(getMode());

                    Debug.Log("Asset loaded successfully from path: " + filePath);
                    _logLabel.text = _logLabel.text + "\n Lưu thành công tới đường dẫn:\n" + filePath;
                    loadNoiseElement.style.display = DisplayStyle.None;
                }
            }
            catch (IOException e)
            {
                Debug.LogError("Failed to load asset: " + e.Message + filePath);
                _logLabel.text = _logLabel.text + "\n Lỗi:\n" + e.Message;
            }
        }
        else
        {
            Debug.LogWarning("No saved asset found at path:\n" + filePath);
            _logLabel.text = _logLabel.text + "\n Lỗi không kiếm được:" + filePath;
        }
    }
    /*
    private NoiseSettings ReadNoiseSettings(BinaryReader reader)
    {
        NoiseSettings noiseSettings = new NoiseSettings();
        //noiseSettings.normallizeMode = Noise.NormallizeMode.Local;
        noiseSettings.scale = reader.ReadSingle();
        noiseSettings.octaves = reader.ReadInt32();
        noiseSettings.persistence = reader.ReadSingle();
        noiseSettings.lacunarity = reader.ReadSingle();
        noiseSettings.seed = reader.ReadInt32();
        noiseSettings.offset = new Vector2(reader.ReadSingle(), reader.ReadSingle());

        return noiseSettings;
    }

    private AnimationCurve ReadAnimationCurve(BinaryReader reader)
    {
        int keyCount = reader.ReadInt32();
        Keyframe[] keyframes = new Keyframe[keyCount];
        for (int i = 0; i < keyCount; i++)
        {
            float time = reader.ReadSingle();
            float value = reader.ReadSingle();
            float inTangent = reader.ReadSingle();
            float outTangent = reader.ReadSingle();
            keyframes[i] = new Keyframe(time, value, inTangent, outTangent);
        }

        AnimationCurve curve = new AnimationCurve(keyframes);
        curve.preWrapMode = (WrapMode)reader.ReadInt32();
        curve.postWrapMode = (WrapMode)reader.ReadInt32();

        return curve;
    }
    */

    void OnTextureSaveButton()
    {
        saveTextureElement.style.display = DisplayStyle.Flex;
    }
    void OnTextureCancelButton()
    {
        saveTextureElement.style.display = DisplayStyle.None;
    }

    void OnTextureRSaveButton()
    {
        string text = _saveTextureNameInput.text;
        if (string.IsNullOrEmpty(text))
        {
            // Generate a random three-character string
            text = GenerateRandomString(3);
        }

        // Remove spaces and special characters
        text = RemoveSpacesAndSpecialCharacters(text);
        bool isReserved = IsReservedName(text);

        if (isReserved)
        {
            _logLabel.text = _logLabel.text + "\nThe entered file name is a reserved name. Please choose a different name.";
        }
        else
        {
            // Save the modified asset as a binary file
            string filePath = Application.persistentDataPath + "/" + text + "texture" + ".dat";
            TextureDataSerializer.SaveTextureData(textureSettings, filePath, _logLabel);
            PopulateDropdown("texture", _loadTextureNameInput);
            saveTextureElement.style.display = DisplayStyle.None;
        }
    }
    void OnTextureLoadButton()
    {
        loadTextureElement.style.display = DisplayStyle.Flex;
    }
    void OnTextureLoadCancelButton()
    {
        loadTextureElement.style.display = DisplayStyle.None;
    }

    void OnTextureRLoadButton()
    {
        string name = _loadTextureNameInput.value;
        string filePath = Application.persistentDataPath + "/" + name + ".dat";
        TextureData loadedTextureData = TextureDataSerializer.LoadTextureData(filePath, textureSettings, _logLabel);
        if (loadedTextureData != null)
        {

            // Perform any necessary operations with the loaded textureData
            // Assign the loaded settings to your desired class or struct instance
            // For example:
            mapGen = GameObject.FindGameObjectWithTag("MapDisplay").GetComponent<MapDisplay>();
            //mapGen.heightMapSettings = ScriptableObject.CreateInstance<HeightMapSettings>();
            mapGen.textureData.CopyFrom(loadedTextureData);
            generatedValue();
            mapGen.DrawMapInRuntime(getMode());
            loadTextureElement.style.display = DisplayStyle.None;
        }
    }
    //----

    //Xử lý sự kiện cho texture
    void OnSliderValueChanged(Slider slider, ChangeEvent<float> evt, Slider[] targetArray)
    {
        int index = Array.IndexOf(targetArray, slider);

        /*if (evt.target == _sliderTintStr[index])
        {
            textureSettings.layers[index].tintStr = slider.value;
        }
        if (evt.target == _sliderBlend[index])
        {
            //textureSettings.layers[index].blendStr = slider.value;
        }*/
        if (evt.target == _sliderStartHeight[index])
        {
            textureSettings.layers[index].startHeight = slider.value;
        }

        mapGen = GameObject.FindGameObjectWithTag("MapDisplay").GetComponent<MapDisplay>();
        mapGen.DrawMapInRuntime(getMode());
    }
    /*void OnFieldArrayValueChanged(TextField field, ChangeEvent<string> evt, TextField[] targetArray)
    {
        int index = Array.IndexOf(targetArray, field);

        if (evt.target == _textureStrInput[index])
        {
            //textureSettings.layers[index].textureScale = float.Parse(field.value);
        }
        mapGen = GameObject.FindGameObjectWithTag("MapDisplay").GetComponent<MapDisplay>();
        mapGen.DrawMapInRuntime(getMode());
    }*/
    void OnArrayToggleValueChanged(Toggle field, ChangeEvent<bool> evt, Toggle[] targetArray)
    {
        int index = Array.IndexOf(targetArray, field);

        if (evt.target == _activeToggle[index])
        {
            if (field.value)
            {
                textureSettings.layers[index].active = 1;
            }
            else
            {
                textureSettings.layers[index].active = 0;
            }
        }
        mapGen = GameObject.FindGameObjectWithTag("MapDisplay").GetComponent<MapDisplay>();
        mapGen.DrawMapInRuntime(getMode());
    }
    //----

    //Xử lý sự kiện cho các element bình thường
    void OnValuesChange(ChangeEvent<string> evt)
    {
        if (evt.target == _scaleInput)
        {
            bool success = float.TryParse(_scaleInput.value, out heightMapSettings.noiseSettings.scale);
            //heightMapSettings.noiseSettings.scale = float.Parse(_scaleInput.value);
            if (!success)
                heightMapSettings.noiseSettings.scale = 1;
        }
        /*if (evt.target == _octavesInput)
        {
            bool success = int.TryParse(_octavesInput.value, out heightMapSettings.noiseSettings.octaves);
            //heightMapSettings.noiseSettings.octaves = int.Parse(_octavesInput.value);
            if (!success)
            {
                heightMapSettings.noiseSettings.octaves = 1;
            }
            else if (heightMapSettings.noiseSettings.octaves > 20)
            {
                heightMapSettings.noiseSettings.octaves = 20;
                _octavesInput.value = heightMapSettings.noiseSettings.octaves.ToString();
            }

        }*/
        if (evt.target == _octavesInput)
        {
            if (int.TryParse(_octavesInput.value, out int octavesValue) && octavesValue >= 0)
            {
                if (octavesValue > 20)
                {
                    octavesValue = 20;
                    _octavesInput.value = octavesValue.ToString();
                }

                heightMapSettings.noiseSettings.octaves = octavesValue;
            }
            else
            {
                // If the value is negative or parsing fails, set octaves to 1
                heightMapSettings.noiseSettings.octaves = 1;
            }
        }
        if (evt.target == _lacunarityInput)
        {
            bool success = float.TryParse(_lacunarityInput.value, out heightMapSettings.noiseSettings.lacunarity);
            //heightMapSettings.noiseSettings.lacunarity = float.Parse(_lacunarityInput.value);
            if (!success)
                heightMapSettings.noiseSettings.lacunarity = 1;
        }
        if (evt.target == _seedInput)
        {
            bool success = int.TryParse(_seedInput.value, out heightMapSettings.noiseSettings.seed);
            heightMapSettings.noiseSettings.seed = int.Parse(_seedInput.value);
            if (!success)
                heightMapSettings.noiseSettings.seed = 1;
        }
        if (evt.target == _heightMultiInput)
        {
            bool success = float.TryParse(_heightMultiInput.value, out heightMapSettings.HeightMultiplier);
            heightMapSettings.HeightMultiplier = float.Parse(_heightMultiInput.value);
            if (!success)
                heightMapSettings.noiseSettings.seed = 1;
        }
        if (evt.target == _drawMode)
        {
            showSettings();
        }
        mapGen = GameObject.FindGameObjectWithTag("MapDisplay").GetComponent<MapDisplay>();
        mapGen.DrawMapInRuntime(getMode());
        //Debug.Log("changed");
    }

    public string getMode()
    {
        var rootElement = mainMenuDocument.rootVisualElement;
        _drawMode = rootElement.Q<DropdownField>("mode");
        return _drawMode.value;
    }

    void OnFloatValuesChange(ChangeEvent<float> evt)
    {
        if (evt.target == _persistenceInput)
        {
            heightMapSettings.noiseSettings.persistence = _persistenceInput.value;
        }
        if (evt.target == _spinInput)
        {
            spin = GameObject.FindGameObjectWithTag("Holder").GetComponent<Spining>();
            spin.speed = _spinInput.value;
        }
        mapGen = GameObject.FindGameObjectWithTag("MapDisplay").GetComponent<MapDisplay>();
        mapGen.DrawMapInRuntime(getMode());
        //Debug.Log("changed");
    }

    void OnIntValuesChange(ChangeEvent<int> evt)
    {
        if (evt.target == _lodInput)
        {
            mapGen.levelOfDetail = _lodInput.value;
        }
        if (evt.target == _chunkSizeInput)
        {
            meshSettings.chunkSizeIndex = _chunkSizeInput.value;
        }
        mapGen = GameObject.FindGameObjectWithTag("MapDisplay").GetComponent<MapDisplay>();
        mapGen.DrawMapInRuntime(getMode());
        //Debug.Log("changed");
    }

    void OnToggleValuesChange(ChangeEvent<bool> evt)
    {
        heightMapSettings.useFallOff = _useFallOff.value;
        mapGen = GameObject.FindGameObjectWithTag("MapDisplay").GetComponent<MapDisplay>();
        mapGen.DrawMapInRuntime(getMode());
        //Debug.Log("changed");
    }
    //---

    //Khởi tạo giá trị cho các Element Texture
    void SliderArraysStart()
    {
        for (int i = 0; i < textureSettings.layers.Length; i++)
        {
            //_sliderTintStr[i].value = textureSettings.layers[i].tintStr;
            _sliderStartHeight[i].value = textureSettings.layers[i].startHeight;
            //_sliderBlend[i].value = textureSettings.layers[i].blendStr;
            //_textureStrInput[i].value = textureSettings.layers[i].textureScale.ToString();
            if (textureSettings.layers[i].active == 1)
                _activeToggle[i].value = true;
            else
                _activeToggle[i].value = false;
            Color newColor = new Color(textureSettings.layers[i].tint.r, textureSettings.layers[i].tint.g, textureSettings.layers[i].tint.b, 1.0f);
            //Debug.Log("color" + textureSettings.layers[i].tint);
            _colorButton[i].style.backgroundColor = new StyleColor(newColor);

        }
    }
    //Query lấy element cho các biến Texture
    void queryTextureElement(ScrollView _scrollviewHolder)
    {
        var rootElement = mainMenuDocument.rootVisualElement;
        //_scrollviewHolder = rootElement.Q<ScrollView>("ScrollView");
        for (int i = 0; i < 7; i++)
        {
            subElement[i] = rootElement.Q<VisualElement>("v" + i);
            childsElement[i] = subElement[i].Q<VisualElement>("sub" + i);
            //_sliderTintStr[i] = subElement[i].Q<Slider>("TintStr" + i);
            _sliderStartHeight[i] = subElement[i].Q<Slider>("StartHeight" + i);
            //_sliderBlend[i] = subElement[i].Q<Slider>("Blend" + i);
            //_textureStrInput[i] = subElement[i].Q<TextField>("TextureStr" + i);
            _activeToggle[i] = subElement[i].Q<Toggle>("setColor" + i);
            _colorButton[i] = subElement[i].Q<Button>("color" + i);
            _setColorButton[i] = subElement[i].Q<Button>("saveColor" + i);

        }
    }
    //Query các Element khác
    void queryElement()
    {
        var rootElement = mainMenuDocument.rootVisualElement; // lấy root
        _mainContainer = rootElement.Q<VisualElement>("mainContainer");
        _mainContainer1 = _mainContainer.Q<VisualElement>("Settings");
        _scrollviewHolder = _mainContainer1.Q<ScrollView>("ScrollViewmain"); // Lấy scroll View

        queryTextureElement(_scrollviewHolder); // query các phần tử Texture
        _saveTexture = _scrollviewHolder.Q<Button>("saveTexture");
        _loadTexture = _scrollviewHolder.Q<Button>("loadTexture");
        _assetTexture = _scrollviewHolder.Q<Label>("assetTexture");
        _textureTitle = _scrollviewHolder.Q<VisualElement>("textureTitle");

        saveTextureElement = _scrollviewHolder.Q<VisualElement>("saveNameTexture");
        saveHolderTextureElement = saveTextureElement.Q<VisualElement>("saveTextureHolder");
        _saveTextureNameInput = saveHolderTextureElement.Q<TextField>("textureSaveName");
        saveButtonHolderTextureElement = saveTextureElement.Q<VisualElement>("saveButtonTextureHolder");
        _cancelTextureButton = saveButtonHolderTextureElement.Q<Button>("cancelTexture");
        _rsaveTexture = saveButtonHolderTextureElement.Q<Button>("rsaveTexture");

        loadTextureElement = _scrollviewHolder.Q<VisualElement>("loadNameTexture");
        loadHolderTextureElement = loadTextureElement.Q<VisualElement>("loadTextureHolder");
        _loadTextureNameInput = loadHolderTextureElement.Q<DropdownField>("loadlistTexture");
        loadButtonHolderTextureElement = loadTextureElement.Q<VisualElement>("loadButtonTextureHolder");
        _cancelLoadTextureButton = loadButtonHolderTextureElement.Q<Button>("cancelLoadTexture");
        _rloadTexture = loadButtonHolderTextureElement.Q<Button>("rloadTexture");


        //Noise element
        _scaleInput = _scrollviewHolder.Q<TextField>("noiseScale");
        _octavesInput = _scrollviewHolder.Q<TextField>("octaves");
        _persistenceInput = _scrollviewHolder.Q<Slider>("persistance");
        _lacunarityInput = _scrollviewHolder.Q<TextField>("lacunarity");
        _seedInput = _scrollviewHolder.Q<TextField>("seed");
        _heightMultiInput = _scrollviewHolder.Q<TextField>("heightMultiplier");
        _useFallOff = _scrollviewHolder.Q<Toggle>("useFalloff");

        _saveNoise = _scrollviewHolder.Q<Button>("saveNoise");
        _loadNoise = _scrollviewHolder.Q<Button>("loadNoise");
        _assetNoise = _scrollviewHolder.Q<Label>("assetNoise");

        saveNoiseElement = _scrollviewHolder.Q<VisualElement>("saveNameNoise");
        saveHolderNoiseElement = saveNoiseElement.Q<VisualElement>("saveNoiseHolder");
        _saveNoiseNameInput = saveHolderNoiseElement.Q<TextField>("noiseSaveName");
        saveButtonHolderNoiseElement = saveNoiseElement.Q<VisualElement>("saveButtonNoiseHolder");
        _cancelNoiseButton = saveButtonHolderNoiseElement.Q<Button>("cancelNoise");
        _rsaveNoise = saveButtonHolderNoiseElement.Q<Button>("rsaveNoise");

        loadNoiseElement = _scrollviewHolder.Q<VisualElement>("loadNameNoise");
        loadHolderNoiseElement = loadNoiseElement.Q<VisualElement>("loadNoiseHolder");
        _loadNoiseNameInput = loadHolderNoiseElement.Q<DropdownField>("loadlistNoise");
        loadButtonHolderNoiseElement = loadNoiseElement.Q<VisualElement>("loadButtonNoiseHolder");
        _cancelLoadNoiseButton = loadButtonHolderNoiseElement.Q<Button>("cancelLoadNoise");
        _rloadNoise = loadButtonHolderNoiseElement.Q<Button>("rloadNoise");

        //Mesh Element
        _lodInput = _scrollviewHolder.Q<SliderInt>("lod");
        _chunkSizeInput = _scrollviewHolder.Q<SliderInt>("chunkSize");

        //Hệ thống
        _spinInput = _scrollviewHolder.Q<Slider>("spin");
        _drawMode = _scrollviewHolder.Q<DropdownField>("mode");
        _exitButton = rootElement.Q<Button>("exit");
        _saveMeshButton = rootElement.Q<Button>("saveMesh");
        _saveMeshpopupButton = rootElement.Q<Button>("saveMeshpopup");
        _cancelMeshpopupButton = rootElement.Q<Button>("cancelMeshpopup");
        _showLogButton = rootElement.Q<Button>("showlog");
        _meshnameInput = rootElement.Q<TextField>("meshName");
        _logLabel = rootElement.Q<Label>("log");
        _logContainer = rootElement.Q<VisualElement>("logContainer");
        _saveProgress = rootElement.Q<ProgressBar>("saveprogress");

    }
    //khoi tao dropdown
    public void PopulateDropdown(string prefix, DropdownField dropdown)
    {
        string[] fileNames = Directory.GetFiles(Application.persistentDataPath, "*.dat")
            .Select(Path.GetFileNameWithoutExtension)
            .Where(name => name.EndsWith(prefix))
            .ToArray();

        dropdown.choices = fileNames.ToList();
    }

    //Khởi tạo giá trị ban đầu
    void generatedValue()
    {
        mapGen = GameObject.FindGameObjectWithTag("MapDisplay").GetComponent<MapDisplay>();
        var rangeAttribute = (RangeAttribute)System.Attribute.GetCustomAttribute(
            typeof(MapDisplay).GetField("levelOfDetail"), typeof(RangeAttribute));
        int maxLOD = (int)rangeAttribute.max;
        //Debug.Log(maxLOD);
        queryElement();


        PopulateDropdown("noise", _loadNoiseNameInput);
        PopulateDropdown("texture", _loadTextureNameInput);
        _scaleInput.value = heightMapSettings.noiseSettings.scale.ToString();
        _octavesInput.value = heightMapSettings.noiseSettings.octaves.ToString();
        _persistenceInput.value = heightMapSettings.noiseSettings.persistence;
        _lacunarityInput.value = heightMapSettings.noiseSettings.lacunarity.ToString();
        _seedInput.value = heightMapSettings.noiseSettings.seed.ToString();
        _heightMultiInput.value = heightMapSettings.HeightMultiplier.ToString();
        _lodInput.value = mapGen.levelOfDetail;
        _useFallOff.value = heightMapSettings.useFallOff;
        _chunkSizeInput.value = meshSettings.chunkSizeIndex;
        _meshnameInput.value = "";


        SliderArraysStart();
    }
    //Khi hoạt động
    void OnEnable()
    {
        queryElement();

        _scaleInput.RegisterValueChangedCallback(OnValuesChange);
        _octavesInput.RegisterValueChangedCallback(OnValuesChange);
        _persistenceInput.RegisterValueChangedCallback(OnFloatValuesChange);
        _lacunarityInput.RegisterValueChangedCallback(OnValuesChange);
        _seedInput.RegisterValueChangedCallback(OnValuesChange);
        _heightMultiInput.RegisterValueChangedCallback(OnValuesChange);

        _saveNoise.clickable.clicked += OnNoiseSaveButton;
        _cancelNoiseButton.clickable.clicked += OnNoiseCancelButton;
        _rsaveNoise.clickable.clicked += OnNoiseRSaveButton;

        _loadNoise.clickable.clicked += OnNoiseLoadButton;
        _cancelLoadNoiseButton.clickable.clicked += OnNoiseLoadCancelButton;
        _rloadNoise.clickable.clicked += OnNoiseRLoadButton;

        _lodInput.RegisterValueChangedCallback(OnIntValuesChange);
        _chunkSizeInput.RegisterValueChangedCallback(OnIntValuesChange);
        _useFallOff.RegisterValueChangedCallback(OnToggleValuesChange);
        _spinInput.RegisterValueChangedCallback(OnFloatValuesChange);

        //RegisterSliderArrayCallbacks(_sliderTintStr);

        RegisterSliderArrayCallbacks(_sliderStartHeight);

        //RegisterSliderArrayCallbacks(_sliderBlend);

        //RegisterFieldArrayCallbacks(_textureStrInput);

        RegisterToggleArrayCallbacks(_activeToggle);

        RegisterButtonArrayCallbacks(_colorButton, _setColorButton);

        _saveTexture.clickable.clicked += OnTextureSaveButton;
        _cancelTextureButton.clickable.clicked += OnTextureCancelButton;
        _rsaveTexture.clickable.clicked += OnTextureRSaveButton;

        _loadTexture.clickable.clicked += OnTextureLoadButton;
        _cancelLoadTextureButton.clickable.clicked += OnTextureLoadCancelButton;
        _rloadTexture.clickable.clicked += OnTextureRLoadButton;

        _drawMode.RegisterValueChangedCallback(OnValuesChange);
        _exitButton.clickable.clicked += OnButtonClicked;
        _saveMeshpopupButton.clickable.clicked += OnMeshPopup;
        _cancelMeshpopupButton.clickable.clicked += OnCancelPopup;
        _saveMeshButton.clickable.clicked += OnMeshSave;
        _showLogButton.clickable.clicked += OnShowlog;
    }
    private void OnDisable()
    {
        _saveNoise.clickable.clicked -= OnNoiseSaveButton;
        _cancelNoiseButton.clickable.clicked -= OnNoiseCancelButton;
        _rsaveNoise.clickable.clicked -= OnNoiseRSaveButton;

        _loadNoise.clickable.clicked -= OnNoiseLoadButton;
        _cancelLoadNoiseButton.clickable.clicked -= OnNoiseLoadCancelButton;
        _rloadNoise.clickable.clicked -= OnNoiseRLoadButton;

        _saveTexture.clickable.clicked -= OnTextureSaveButton;
        _cancelTextureButton.clickable.clicked -= OnTextureCancelButton;
        _rsaveTexture.clickable.clicked -= OnTextureRSaveButton;

        _loadTexture.clickable.clicked -= OnTextureLoadButton;
        _cancelLoadTextureButton.clickable.clicked -= OnTextureLoadCancelButton;
        _rloadTexture.clickable.clicked -= OnTextureRLoadButton;

        _exitButton.clickable.clicked -= OnButtonClicked;
        _saveMeshpopupButton.clickable.clicked -= OnMeshPopup;
        _cancelMeshpopupButton.clickable.clicked -= OnCancelPopup;
        _saveMeshButton.clickable.clicked -= OnMeshSave;
        _showLogButton.clickable.clicked -= OnShowlog;
    }
    void Start()
    {
        generatedValue();
        showSettings();
        mapGen.DrawMapInRuntime(getMode());
    }

    //chuan hoa text
    private string RemoveSpacesAndSpecialCharacters(string input)
    {
        // Remove spaces and special characters using regular expressions
        string processedText = System.Text.RegularExpressions.Regex.Replace(input, "[^a-zA-Z0-9]", "");
        return processedText;
    }

    private bool IsReservedName(string fileName)
    {
        string[] reservedNames = GetReservedNames(); // Custom list of reserved names

        return reservedNames.Any(name => string.Equals(name, fileName, StringComparison.OrdinalIgnoreCase));
    }

    private string[] GetReservedNames()
    {
        // Add the reserved names specific to your target operating system
        // This example includes some reserved names in Windows
        return new string[]
        {
            "CON", "PRN", "AUX", "NUL",
            "COM1", "COM2", "COM3", "COM4", "COM5", "COM6", "COM7", "COM8", "COM9",
            "LPT1", "LPT2", "LPT3", "LPT4", "LPT5", "LPT6", "LPT7", "LPT8", "LPT9"
        };
    }

    private string GenerateRandomString(int length)
    {
        const string characters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        string randomString = "";

        for (int i = 0; i < length; i++)
        {
            int randomIndex = UnityEngine.Random.Range(0, characters.Length);
            randomString += characters[randomIndex];
        }

        return randomString;
    }
    //---

    //Hiển thị các thông số cần
    void showSettings()
    {
        if (_drawMode.value == "Noise" || _drawMode.value == "Raw Noise")
        {
            for (int i = 0; i < 7; i++)
            {
                subElement[i].style.display = DisplayStyle.None;
            }
            _spinInput.style.display = DisplayStyle.None;
            _lodInput.style.display = DisplayStyle.None;
            _textureTitle.style.display = DisplayStyle.None;
            _saveMeshpopupButton.style.display = DisplayStyle.None;
            canvasButton.gameObject.SetActive(false);
        }
        if (_drawMode.value == "Mesh")
        {
            for (int i = 0; i < 7; i++)
            {
                subElement[i].style.display = DisplayStyle.Flex;
            }
            _spinInput.style.display = DisplayStyle.Flex;
            _lodInput.style.display = DisplayStyle.Flex;
            _textureTitle.style.display = DisplayStyle.Flex;
            _saveMeshpopupButton.style.display = DisplayStyle.Flex;
            canvasButton.gameObject.SetActive(true);
        }
        if (_drawMode.value == "Color Map")
        {
            for (int i = 0; i < 7; i++)
            {
                subElement[i].style.display = DisplayStyle.Flex;
            }
            _spinInput.style.display = DisplayStyle.None;
            _lodInput.style.display = DisplayStyle.None;
            _textureTitle.style.display = DisplayStyle.Flex;
            _saveMeshpopupButton.style.display = DisplayStyle.None;
            canvasButton.gameObject.SetActive(false);
        }
    }

    //---
}
