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

    private string customQueryMessage = "You are a sustainability consultant with a strong background in analyzing the environmental impact of products.\\n" +
    "Your expertise lies in assessing the carbon footprint and sustainability attributes of consumer items.\\n" +
    "You have in-depth knowledge of life cycle assessment methodologies and can accurately estimate the CO2 emissions associated with the production, transportation, and energy use of a product.\\n" +
    "You are skilled in identifying materials used in the construction of items and understanding their environmental implications.\\n" +
    "With your experience, you can generate a JSON-like output that aligns precisely with the specified schema, providing detailed information on the estimated CO2 emissions,\\n" +
    "recyclability score, expected lifespan, practical suggestions for reducing environmental impact, and ways to offset the carbon footprint of the product.\\n" +
    "Your ability to analyze and communicate complex environmental data makes you the ideal agent to handle this task effectively and efficiently.\\n\\n" +
    "Let's think step by step:\\n" +
    "1. Analyze the visible attributes of the product, such as the type of item, brand, model, and main materials used in its construction.\\n" +
    "2. Estimate the CO2 emissions associated with the production and transportation of the product based on the materials used and manufacturing processes.\\n" +
    "3. Calculate the estimated lifetime energy use emissions by assuming a lifespan of 4 years with 8 hours of use every day for electronic devices.\\n" +
    "4. Combine the estimated CO2 emissions from production, transportation, and energy use to determine the total carbon footprint of the product.\\n" +
    "5. Provide a comparative benchmark statement suggesting ways to reduce energy use or emissions related to the product for sustainability purposes.\\n" +
    "6. Evaluate the recyclability score of the product based on the materials used and their potential for recycling.\\n" +
    "7. Determine the expected lifespan of the product to understand its durability and longevity.\\n" +
    "8. Offer practical suggestions for users to reduce their environmental impact when using or disposing of the product.\\n" +
    "9. Suggest ways for users to offset the carbon footprint of the product through tree planting (return only the number of trees).\\n" +
    "10. Ensure that the JSON-like output aligns precisely with the specified schema to provide detailed information on the product's environmental impact and sustainability attributes.\\n" +
    "{\\n" +
"    \\\\\\\"title\\\\\\\": \\\\\\\"Product CO2 Emissions Schema\\\\\\\",\\n" +
"    \\\\\\\"type\\\\\\\": \\\\\\\"object\\\\\\\",\\n" +
"    \\\\\\\"properties\\\\\\\": {\\n" +
"        \\\\\\\"object\\\\\\\": {\\n" +
"            \\\\\\\"type\\\\\\\": \\\\\\\"string\\\\\\\",\\n" +
"            \\\\\\\"description\\\\\\\": \\\\\\\"Type of the item (e.g., laptop, bottle).\\\\\\\"\\n" +
"        },\\n" +
"        \\\\\\\"brand\\\\\\\": {\\n" +
"            \\\\\\\"type\\\\\\\": \\\\\\\"string\\\\\\\",\\n" +
"            \\\\\\\"description\\\\\\\": \\\\\\\"Brand name of the product.\\\\\\\"\\n" +
"        },\\n" +
"        \\\\\\\"model\\\\\\\": {\\n" +
"            \\\\\\\"type\\\\\\\": \\\\\\\"string\\\\\\\",\\n" +
"            \\\\\\\"description\\\\\\\": \\\\\\\"Model name or number (if applicable).\\\\\\\"\\n" +
"        },\\n" +
"        \\\\\\\"material\\\\\\\": {\\n" +
"            \\\\\\\"type\\\\\\\": \\\\\\\"string\\\\\\\",\\n" +
"            \\\\\\\"description\\\\\\\": \\\\\\\"Main materials used in the product’s construction.\\\\\\\"\\n" +
"        },\\n" +
"        \\\\\\\"estimated_CO2_emissions\\\\\\\": {\\n" +
"            \\\\\\\"type\\\\\\\": \\\\\\\"object\\\\\\\",\\n" +
"            \\\\\\\"properties\\\\\\\": {\\n" +
"                \\\\\\\"production\\\\\\\": {\\n" +
"                    \\\\\\\"type\\\\\\\": \\\\\\\"string\\\\\\\",\\n" +
"                    \\\\\\\"description\\\\\\\": \\\\\\\"Estimated CO2 emissions from production (in kg).\\\\\\\"\\n" +
"                },\\n" +
"                \\\\\\\"transportation\\\\\\\": {\\n" +
"                    \\\\\\\"type\\\\\\\": \\\\\\\"string\\\\\\\",\\n" +
"                    \\\\\\\"description\\\\\\\": \\\\\\\"Estimated CO2 emissions from transportation (in kg).\\\\\\\"\\n" +
"                },\\n" +
"                \\\\\\\"energy_use\\\\\\\": {\\n" +
"                    \\\\\\\"type\\\\\\\": \\\\\\\"string\\\\\\\",\\n" +
"                    \\\\\\\"description\\\\\\\": \\\\\\\"Estimated lifetime energy use emissions (in kg).\\\\\\\"\\n" +
"                },\\n" +
"                \\\\\\\"total\\\\\\\": {\\n" +
"                    \\\\\\\"type\\\\\\\": \\\\\\\"string\\\\\\\",\\n" +
"                    \\\\\\\"description\\\\\\\": \\\\\\\"Total estimated CO2 emissions combining all the above.\\\\\\\"\\n" +
"                }\\n" +
"            },\\n" +
"            \\\\\\\"required\\\\\\\": [\\\\\\\"production\\\\\\\", \\\\\\\"transportation\\\\\\\", \\\\\\\"energy_use\\\\\\\", \\\\\\\"total\\\\\\\"]\\n" +
"        },\\n" +
"        \\\\\\\"comparative_benchmark\\\\\\\": {\\n" +
"            \\\\\\\"type\\\\\\\": \\\\\\\"string\\\\\\\",\\n" +
"            \\\\\\\"description\\\\\\\": \\\\\\\"A statement suggesting ways to reduce energy use or emissions related to the product.\\\\\\\"\\n" +
"        },\\n" +
"        \\\\\\\"recyclability_score\\\\\\\": {\\n" +
"            \\\\\\\"type\\\\\\\": \\\\\\\"string\\\\\\\",\\n" +
"            \\\\\\\"description\\\\\\\": \\\\\\\"Estimated recyclability percentage of the product.\\\\\\\"\\n" +
"        },\\n" +
"        \\\\\\\"estimated_lifespan\\\\\\\": {\\n" +
"            \\\\\\\"type\\\\\\\": \\\\\\\"string\\\\\\\",\\n" +
"            \\\\\\\"description\\\\\\\": \\\\\\\"Expected lifespan of the product (in years).\\\\\\\"\\n" +
"        },\\n" +
"        \\\\\\\"suggestions\\\\\\\": {\\n" +
"            \\\\\\\"type\\\\\\\": \\\\\\\"array\\\\\\\",\\n" +
"            \\\\\\\"items\\\\\\\": {\\n" +
"                \\\\\\\"type\\\\\\\": \\\\\\\"string\\\\\\\"\\n" +
"            },\\n" +
"            \\\\\\\"description\\\\\\\": \\\\\\\"A list of practical suggestions for users to reduce their environmental impact.\\\\\\\"\\n" +
"        },\\n" +
"        \\\\\\\"carbon_offset\\\\\\\": {\\n" +
"            \\\\\\\"type\\\\\\\": \\\\\\\"int\\\\\\\",\\n" +
"            \\\\\\\"description\\\\\\\": \\\\\\\"A statement on how users can offset the carbon footprint of the product.\\\\\\\"\\n" +
"        }\\n" +
"    },\\n" +
"    \\\\\\\"required\\\\\\\": [\\n" +
"        \\\\\\\"object\\\\\\\",\\n" +
"        \\\\\\\"brand\\\\\\\",\\n" +
"        \\\\\\\"model\\\\\\\",\\n" +
"        \\\\\\\"material\\\\\\\",\\n" +
"        \\\\\\\"estimated_CO2_emissions\\\\\\\",\\n" +
"        \\\\\\\"comparative_benchmark\\\\\\\",\\n" +
"        \\\\\\\"recyclability_score\\\\\\\",\\n" +
"        \\\\\\\"estimated_lifespan\\\\\\\",\\n" +
"        \\\\\\\"suggestions\\\\\\\",\\n" +
"        \\\\\\\"carbon_offset\\\\\\\"\\n" +
"    ]\\n" +
"}";

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
