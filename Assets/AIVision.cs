using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.Text;
using System.Drawing;

public class AIVision : MonoBehaviour
{

    public Camera targetCamera; // Reference to the camera you want to render
    public RenderTexture renderTexture; // Reference to the RenderTexture to draw to


    [SerializeField] private string openAIUrl = "https://api.openai.com/v1/chat/completions";
    [SerializeField] private string apiKey = "YOUR_API_KEY";

    public string[] imageUrls = new string[]
    {
        "https://upload.wikimedia.org/wikipedia/commons/thumb/d/dd/Gfp-wisconsin-madison-the-nature-boardwalk.jpg/2560px-Gfp-wisconsin-madison-the-nature-boardwalk.jpg",
        "https://upload.wikimedia.org/wikipedia/commons/thumb/d/dd/Gfp-wisconsin-madison-the-nature-boardwalk.jpg/2560px-Gfp-wisconsin-madison-the-nature-boardwalk.jpg"
    };
    public string queryMessage = "What are in these images? Is there any difference between them?";

    void Start()
    {
        if (imageUrls.Length > 0)
        {
            StartCoroutine(PostImageQueryRequest(imageUrls));
        }
    }

    public void OnClickSend()
    {
        StartCoroutine(PostImageQueryRequest(imageUrls));
    }

    IEnumerator PostImageQueryRequest(string[] urls)
    {
        // Build the message content manually to ensure it's formatted correctly
        string contentList = "[{\"type\": \"text\", \"text\": \"" + queryMessage + "\"}";

        // Add image URLs to the content list if they exist
        if (urls.Length > 0)
        {
            contentList += ", {\"type\": \"image_url\", \"image_url\": {\"url\": \"" + urls[0] + "\"}}";
        }
        if (urls.Length > 1)
        {
            contentList += ", {\"type\": \"image_url\", \"image_url\": {\"url\": \"" + urls[1] + "\"}}";
        }
        contentList += "]";

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
        }
        else
        {
            Debug.LogWarning("Camera or RenderTexture is not assigned!");
        }
    }
}