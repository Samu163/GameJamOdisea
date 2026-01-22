using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[ExecuteAlways]
public class Billboard : MonoBehaviour
{

    private Camera camera;

    void Awake()
    {
        camera = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        transform.rotation = Quaternion.LookRotation(transform.position - camera.transform.position);
    }
}
