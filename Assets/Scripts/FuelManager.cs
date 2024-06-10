using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class FuelManager : MonoBehaviour
{
    private Transform[] fuels;
    
    // Start is called before the first frame update
    void Start()
    {
        fuels = GetComponentsInChildren<Transform>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void FuelOn()
    {
        foreach (var fuel in fuels)
        {
            fuel.gameObject.SetActive(true);
        }
    }
}
