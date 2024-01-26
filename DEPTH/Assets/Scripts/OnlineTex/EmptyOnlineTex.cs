using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine.Networking;

using WebSocketSharp;
using System.Security.Policy;
using System.Collections.Concurrent;
using System.Threading.Tasks;

public class EmptyOnlineTex : OnlineTex
{
    public bool Supported { get; private set; } = true;
    public float LastTime { get; private set; } = 0;

    private string _url;
    private CanRunCoroutine _behav;

    private WebSocket ws;

    private bool _isWaiting = false;
    private float _startingTime;
    private Texture2D _currentTex;
    //private ConcurrentQueue<string> queue = new ConcurrentQueue<string>();

    public EmptyOnlineTex(string url)
    {
        _url = url;
        _behav = Utils.GetDummyBehavior();
        //Supported = (_behav != null);

        //ws = new WebSocket(_url);
        //ws.OnMessage += OnMessage;
        //ws.Connect();

        //_currentTex = StaticGOs.PlaceholderTexture;

        //Debug.Log($"Connecting to {url}");
        //UITextSet.StatusText.text = "Connecting...";

    }

  

    ////  is called whenever a message is received from the WebSocket server
    //private void OnMessage(object sender, MessageEventArgs e)
    //{

    //    //if (queue.Count > 8)
    //    //{
    //    //    queue.TryDequeue(out string removed);
    //    //}
    //    //queue.Enqueue(e.Data);
    //    string message = e.Data;

    //    //MainBehavior.instance.get_texInputs().UpdateTex();

    //    Task.Run(() => LoadTextureFromBase64(message));
    //}


    //public void LoadTextureFromBase64(string base64Image)
    //{

    //    int w = 720;
    //    int h = 480;

    //    byte[] imageData = Convert.FromBase64String(base64Image);

    //    Texture2D temp = new Texture2D(w, h);
    //    temp.LoadImage(imageData); // Load the image data into the texture
    //    MainBehavior.instance.get_texInputs().UpdateTexCustom(temp);

    //}

    public void StartRendering() { }



    //private void Get()
    //{
    //    //_startingTime = Time.time;
    //    queue.TryDequeue(out string message);
    //    if (message.Equals(null)) return;
    //    _startingTime = Time.time;
    //    LoadTextureFromBase64(message);
    //    MainBehavior.instance.get_texInputs().UpdateTexCustom(_currentTex);
        
    //    LastTime = Time.time;
    //    UITextSet.StatusText.text = $"fps: {(int)(1 / (LastTime - _startingTime))}";
    //    Debug.Log(queue.Count);
    //}

    public void Dispose()
    {
        UITextSet.StatusText.text = "Disconnecting.";
        Debug.Log($"Disconnecting from {_url}");

        //_isWaiting = true;

        //OnDestroy();
    }

    public Texture2D GetTex()
    {
        throw new NotImplementedException();
    }
}