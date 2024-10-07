using UnityEngine;
using UnityEditor;
using System.Reflection;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.XR.ARFoundation;
#if UNITY_EDITOR

public class ARSetup : MonoBehaviour
{
    [MenuItem("Tech Art Library/AR/Check Player Settings")]
    private static void CheckPlayerSettings()
    {
        if (((int)PlayerSettings.Android.minSdkVersion) < 24)
            Debug.LogError("Minimum API level must be at least 24 for AR");
        BuildTargetGroup buildTargetGroup = EditorUserBuildSettings.selectedBuildTargetGroup;
        ScriptingImplementation scriptingBackend = PlayerSettings.GetScriptingBackend(buildTargetGroup);
        if (scriptingBackend.ToString() != "IL2CPP")
            Debug.LogError("The recommended scripting backend for AR is IL2CPP");
        if (PlayerSettings.colorSpace.ToString() != "Linear")
            Debug.LogWarning("The recommended color space for AR is Linear");
        if (PlayerSettings.Android.targetArchitectures.ToString() != "ARM64")
            Debug.LogError("ARM64 must be targetted for AR");
        if (EditorUserBuildSettings.androidBuildSubtarget.ToString() != "ETC2")
            Debug.LogWarning("The recommended compression format for mobile is ETC2");
        // Get the current render pipeline asset
        RenderPipelineAsset renderPipelineAsset = GraphicsSettings.currentRenderPipeline;
        if (renderPipelineAsset is UniversalRenderPipelineAsset urpAsset)
        Debug.Log("");
        CheckUrpARFeature();
        Debug.Log("Checked");
    }

    private static void CheckUrpARFeature()
    {
        // Get the current render pipeline asset
        RenderPipelineAsset renderPipelineAsset = GraphicsSettings.currentRenderPipeline;

        // Check if the current render pipeline asset is a UniversalRenderPipelineAsset
        if (renderPipelineAsset is UniversalRenderPipelineAsset urpAsset)
        {
            // Use reflection to access the private m_RendererDataList field
            FieldInfo rendererDataListField = typeof(UniversalRenderPipelineAsset).GetField("m_RendererDataList", BindingFlags.NonPublic | BindingFlags.Instance);
            ScriptableRendererData[] rendererDataList = (ScriptableRendererData[])rendererDataListField.GetValue(urpAsset);

            // Loop through the renderer data list
            foreach (var rendererData in rendererDataList)
            {
                if (rendererData is UniversalRendererData universalRendererData)
                {
                    // Check each renderer feature
                    foreach (var rendererFeature in universalRendererData.rendererFeatures)
                    {
                        if (rendererFeature is ARBackgroundRendererFeature)
                        {
                            Debug.Log("AR Background Renderer Feature is added.");
                            return;
                        }
                    }
                }
            }

            Debug.LogError("AR Background Renderer Feature is not added.");
        }
        else
        {
            Debug.LogError("The current render pipeline is not a UniversalRenderPipelineAsset.");
        }
    }

}
#endif