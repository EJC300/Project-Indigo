using UnityEngine;

public class Mover : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.right * 22 * Time.deltaTime + Vector3.up * 5 * Time.deltaTime + Vector3. forward * 5 * Time.deltaTime);
    }
}
