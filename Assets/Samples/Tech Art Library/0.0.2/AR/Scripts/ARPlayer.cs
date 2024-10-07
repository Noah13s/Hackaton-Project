using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARCore;
using UnityEngine.XR.ARFoundation;

public class ARPlayer : MonoBehaviour
{
    public GameObject indicator;
    
    [NonSerialized]
    public Camera camera;
    private GameObject indicatorInstance;
    private ARSession session;
    private Material material;
    private bool planesVisible = true; // Track if planes are visible or hidden
    private Coroutine showHideCoroutine;


    // Start is called before the first frame update
    void Start()
    {
        camera = GetComponentInChildren<Camera>();
        indicatorInstance = Instantiate(indicator);
        session = FindObjectOfType<ARSession>();
        material = GetComponent<ARPlaneManager>().planePrefab.gameObject.GetComponent<MeshRenderer>().sharedMaterial;

    }

    // Update is called once per frame
    void Update()
    {
        targetIndicator();
        if (!planesVisible)// Makes sure newly created planes are hidden
        {
            ARPlane[] planes = FindObjectsOfType<ARPlane>();
            foreach (ARPlane plane in planes)
            {
                plane.GetComponent<MeshRenderer>().material.SetColor("_TexTintColor", new Color(1, 1, 1, 0));
            }
        }
    }

    private void targetIndicator()
    {
        RaycastHit hit;
        if (Physics.Raycast(camera.transform.position, camera.transform.forward, out hit))
        {
            indicatorInstance.SetActive(true);
            indicatorInstance.transform.position = hit.point;
            indicatorInstance.transform.rotation = Quaternion.FromToRotation(transform.up, hit.normal);
        } else
        {
            indicatorInstance.SetActive(false);
        }

    }

    public void resetARSession()
    {
        session.Reset();
    }

    public void showHideARPlane()
    {
        Color visible = new Color(1, 1, 1, 1);
        Color hidden = new Color(1, 1, 1, 0);
        if (showHideCoroutine != null)
        {
            StopCoroutine(showHideCoroutine);
        }
        showHideCoroutine = StartCoroutine(LerpPlaneVisibility(planesVisible ? visible : hidden, planesVisible ? hidden : visible, 1f));
        planesVisible = !planesVisible;
    }

    private IEnumerator LerpPlaneVisibility(Color startColor, Color endColor, float duration)
    {
        ARPlane[] planes = FindObjectsOfType<ARPlane>();
        float elapsed = 0f;
        while (elapsed < duration)
        {
            float t = elapsed / duration;
            Color currentColor = Color.Lerp(startColor, endColor, t);
            foreach (ARPlane plane in planes)
            {
                plane.GetComponent<MeshRenderer>().material.SetColor("_TexTintColor", currentColor);
            }
            elapsed += Time.deltaTime;
            yield return null;
        }
        // Ensure final color is set
        foreach (ARPlane plane in planes)
        {
            plane.GetComponent<MeshRenderer>().material.SetColor("_TexTintColor", endColor);
        }
    }
}

