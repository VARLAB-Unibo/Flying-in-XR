using System;
using System.Collections;
using System.Collections.Concurrent;
using UnityEngine;
using WebSocketSharp;

public class WebSocketClient : MonoBehaviour
{
    private WebSocket ws;
    public static WebSocketClient instance;

    [SerializeField]
    private GameObject textured;

    // gestione dei messaggi WebSocket in un thread separato
    ConcurrentQueue<string> queue = new ConcurrentQueue<string>();
    ConcurrentQueue<float> times = new ConcurrentQueue<float>();

    private float lastTime;

    void Start()
    {
        instance = this;
        // Connect to the WebSocket server
        ws = new WebSocket("ws://localhost:8765");
        ws.OnMessage += OnMessage;
        ws.Connect();

        Debug.Log("WebSocket connected.");
        Debug.Log("WebSocket server started on port 8080");
    }
    void Update()
    {

        //TODO: synchronize with current FPS rate from server
        while (queue.TryDequeue(out string message))
        {
            print("Received message: " + message);

            //float time;
            //times.TryDequeue(out float time);
            LoadTextureFromBase64(message);

        }

    }

    IEnumerator loadedTexture(string base64, float time)
    {
        yield return new WaitForSeconds(time);

        LoadTextureFromBase64(base64);
    }

    void OnMessage(object sender, MessageEventArgs e)
    {

        // Handle the received base64-encoded image data
        string imgBase64 = e.Data;
        queue.Enqueue(imgBase64);  
    }


 
    public void LoadTextureFromBase64(String base64Image)
    {
        Debug.Log(base64Image);
        int w = 1920;
        int h = 1080;
        byte[] imageData = Convert.FromBase64String(base64Image);

        Texture2D texture = new Texture2D(w, h);
        texture.LoadImage(imageData); // Load the image data into the texture
        textured.GetComponent<Renderer>().material.mainTexture = texture;

    }
}
