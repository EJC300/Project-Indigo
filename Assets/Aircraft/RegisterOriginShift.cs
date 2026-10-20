using UnityEngine;
using GameInfrastructure;
public class RegisterOriginShift : MonoBehaviour
{
    public OriginShift originShift;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        originShift.AddToShifter(this.transform);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
