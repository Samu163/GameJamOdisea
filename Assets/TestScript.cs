using UnityEngine;

public class TestScript : MonoBehaviour
{

    Transform _transform;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _transform = GetComponent<Transform>();
    }

    // Update is called once per frame
    void Update()
    {
        _transform.position = new Vector3(0, 0, 0);
    }

    private void OnDisable()
    {
        
    }

    private void OnDestroy()
    {
        
    }

    void PrintHi()
    {
        print("hi");
    }

}
