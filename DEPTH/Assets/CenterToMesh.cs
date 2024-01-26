using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CenterToMesh : MonoBehaviour
{

    [SerializeField]
    GameObject immersiveView;


    // Update is called once per frame
    void Update()
    {
        Vector3 immersiveViewCenter = immersiveView.GetComponent<MeshRenderer>().bounds.center;
        this.transform.position = new Vector3(immersiveViewCenter.x, immersiveViewCenter.y, this.transform.position.z);
    }

}
