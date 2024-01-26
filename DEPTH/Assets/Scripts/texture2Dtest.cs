using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class texture2Dtest : MonoBehaviour
{
    void Start()
    {
        // Set the dimensions of the texture
        int width = 256;
        int height = 256;

        // Create a new Texture2D with the specified dimensions
        Texture2D texture = new Texture2D(width, height);

        // Set some pixels in the texture (for demonstration purposes)
        Color[] colors = new Color[width * height];
        for (int i = 0; i < colors.Length; i++)
        {
            colors[i] = Color.green; // Set all pixels to green
        }

        // Apply the color array to the texture
        texture.SetPixels(colors);

        // Apply changes to the texture
        texture.Apply();

        // Assign the texture to a material for rendering
        GetComponent<Renderer>().material.mainTexture = texture;
    }
}
