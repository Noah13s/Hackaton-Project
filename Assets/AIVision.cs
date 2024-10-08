using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.Text;
using TMPro;
using System.Net;


public class AIVision : MonoBehaviour
{
    public Camera targetCamera; // Reference to the camera you want to render
    public RenderTexture renderTexture; // Reference to the RenderTexture to draw to
    public TextMeshProUGUI outputText;
    public Master_Counter2 counter;

    [SerializeField] private string openAIUrl = "https://api.openai.com/v1/chat/completions";
    [SerializeField] private string apiKey = "YOUR_API_KEY";

    [TextArea(15, 20)]
    public string queryMessage = "What’s in this image?";

    public void OnClickSend()
    {
        CaptureImage();
        StartCoroutine(PostImageQueryRequest());
    }

    IEnumerator PostImageQueryRequest()
    {
        // Capture the image and convert to Base64
        string base64Image = ConvertRenderTextureToBase64(renderTexture);

        // Build the message content manually to ensure it's formatted correctly
        string contentList = "[{\"type\": \"text\", \"text\": \"" + queryMessage + "\"}," +
                             "{\"type\": \"image_url\", \"image_url\": {\"url\": \"data:image/jpeg;base64," + base64Image + "\"}}]";

        // Manually construct the request body
        string json = "{\"model\": \"gpt-4o-mini\", \"messages\": [{\"role\": \"user\", \"content\": " + contentList + "}], \"max_tokens\": 300}";

        // Log the JSON to verify it's correct
        Debug.Log("Request JSON: " + json);

        using (UnityWebRequest webRequest = new UnityWebRequest(openAIUrl, "POST"))
        {
            byte[] jsonToSend = new UTF8Encoding().GetBytes(json);
            webRequest.uploadHandler = new UploadHandlerRaw(jsonToSend);
            webRequest.downloadHandler = new DownloadHandlerBuffer();
            webRequest.SetRequestHeader("Content-Type", "application/json");
            webRequest.SetRequestHeader("Authorization", "Bearer " + apiKey);

            yield return webRequest.SendWebRequest();

            // Log the raw response
            if (webRequest.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Error: " + webRequest.error);
                Debug.LogError("Response Text: " + webRequest.downloadHandler.text);  // Log the error response for more info
            }
            else
            {
                Debug.Log("Response: " + webRequest.downloadHandler.text);
                
                outputText.text = ParseJson(webRequest.downloadHandler.text, "content").Split(",")[1];
                counter.SetCounterValue(counter.GetCounterValue()+int.Parse(ParseJson(webRequest.downloadHandler.text, "content").Split(",")[0]));
            }
        }
    }

    public void CaptureImage()
    {
        if (targetCamera != null && renderTexture != null)
        {
            // Set the target texture of the camera to the RenderTexture
            targetCamera.targetTexture = renderTexture;

            // Render the camera's view to the RenderTexture
            targetCamera.Render();

            // Reset the target texture
            targetCamera.targetTexture = null;

            // Optional: Call PostImageQueryRequest here if you want to send the request immediately after capturing
            // StartCoroutine(PostImageQueryRequest());
        }
        else
        {
            Debug.LogWarning("Camera or RenderTexture is not assigned!");
        }
    }

    public string ConvertRenderTextureToBase64(RenderTexture rt)
    {
        // Set the RenderTexture as the active one
        RenderTexture.active = rt;

        // Create a new Texture2D with the same dimensions as the RenderTexture
        Texture2D texture = new Texture2D(rt.width, rt.height, TextureFormat.RGB24, false);

        // Read the pixels from the RenderTexture into the Texture2D
        texture.ReadPixels(new Rect(0, 0, rt.width, rt.height), 0, 0);
        texture.Apply();

        // Encode the texture to PNG
        byte[] bytes = texture.EncodeToPNG();  // Use EncodeToPNG or ImageConversion.EncodeToPNG

        // Clean up
        RenderTexture.active = null; // Reset the active RenderTexture
        Destroy(texture); // Optional: Destroy the texture to free memory

        // Convert the byte array to a Base64 string
        return System.Convert.ToBase64String(bytes);
    }

    public string ParseJson(string json, string key)
    {
        string jsonResponse = json;

        // Ensure the key exists in the JSON (e.g., "object", "CO2_emission")
        int startIndex = jsonResponse.IndexOf("\"" + key + "\"");

        if (startIndex != -1)
        {
            // Move past the key and find the colon
            startIndex = jsonResponse.IndexOf(':', startIndex) + 1;

            // Trim spaces and quotation marks to handle JSON formatting
            string trimmedResponse = jsonResponse.Substring(startIndex).Trim();

            // Check if the value is surrounded by quotes (for strings like "object")
            if (trimmedResponse[0] == '"')
            {
                // Find the end quote for the string value
                int endIndex = trimmedResponse.IndexOf('"', 1);
                string contentValue = trimmedResponse.Substring(1, endIndex - 1); // Extract the value
                Debug.Log(key + ": " + contentValue);
                return contentValue;
            }
            else
            {
                // If it's not a string, extract until the next comma or closing brace
                int endIndex = trimmedResponse.IndexOfAny(new char[] { ',', '}' });
                string contentValue = trimmedResponse.Substring(0, endIndex).Trim();
                Debug.Log(key + ": " + contentValue);
                return contentValue;
            }
        }
        else
        {
            Debug.Log(key + " key not found.");
            return null;
        }
    }

}
