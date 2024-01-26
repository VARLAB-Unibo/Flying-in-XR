using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowValue : MonoBehaviour
{
    [SerializeField]
    public GameObject value;

    private float offset = 0.25f; 

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        this.value.transform.position = new Vector3(this.transform.position.x, this.transform.position.y + offset, this.transform.position.z);
    }
}
