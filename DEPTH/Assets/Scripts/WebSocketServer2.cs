using System;
using UnityEngine;
using WebSocketSharp;
using WebSocketSharp.Server;
using System.Collections.Concurrent;

public class WebSocketServer2 : MonoBehaviour
{
    // gestione dei messaggi WebSocket in un thread separato
    ConcurrentQueue<string> queue = new ConcurrentQueue<string>();

    private WebSocketServer wssv;
    public static WebSocketServer2 instance;

    [SerializeField]
    private GameObject textured;


    void Start()
    {
        instance = this;
        // Start the WebSocket server on port 8080
        wssv = new WebSocketServer(8080);
        wssv.AddWebSocketService<Echo>("/echo");
        wssv.Start();
        //this.LoadTextureFromBase64("mammina");
        Debug.Log("WebSocket server started on port 8080");
    }

    //void OnDestroy()
    //{
    //    if (wssv != null && wssv.IsListening)
    //        wssv.Stop();
    //}


    void Update()
    {
        while (queue.TryDequeue(out string message))
        {
            print("Received message: " + message);
            LoadTextureFromBase64(message);
        }
    }

    // invia i messaggi alla coda (queue) dei messaggi dei WebSocket, da un thread diverso a quello principale      
    public void SendMessageFromAnotherThread(string message)
    {
        queue.Enqueue(message);
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


        // Set some pixels in the texture (for demonstration purposes)
        //Color[] colors = new Color[256 * 256];
        //Color chosen = new Color (UnityEngine.Random.Range(0.0f, 1.0f), UnityEngine.Random.Range(0.0f, 1.0f), UnityEngine.Random.Range(0.0f, 1.0f));

        //for (int i = 0; i < colors.Length; i++)
        //{
        //    colors[i] = chosen; // Set all pixels to green
        //}

        //// Apply the color array to the texture
        //texture.SetPixels(colors);

        //// Apply changes to the texture
        //texture.Apply();
        ////Debug.Log("Texturezzato!");
        //texture.LoadImage(imageData); // Load the image data into the texture
        //textured.GetComponent<Renderer>().material.mainTexture = texture;
    }

}

public class Echo : WebSocketBehavior
    {


        protected override void OnMessage(MessageEventArgs e)
        {
            // base.OnMessage(e);
            Debug.Log($"Received message: {e.Data}");
            //WebSocketServer2.instance.LoadTextureFromBase64(e.Data);
            WebSocketServer2.instance.SendMessageFromAnotherThread(e.Data.ToString());


            Send("Server received your message: " + "OK");
            

            // Process the received message as needed
            // Send a response back to the client
            
           
        }

    }