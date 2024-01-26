using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoWebSocketConnection : MonoBehaviour
{
    [SerializeField]
    private string url;


    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(checkforMain());
    }


    public IEnumerator checkforMain()
    {
        while (!MainBehavior.instance)
        {
            yield return new WaitForSeconds(0.5f);
        }

        MainBehavior.instance.SocketOnlineTexStart(url);
    }


}
