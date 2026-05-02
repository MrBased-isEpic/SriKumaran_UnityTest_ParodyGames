using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointCube : MonoBehaviour
{
    private PointManager manager;


    private void Start()
    {
        manager = GetComponentInParent<PointManager>();
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        
        manager.AddPoint();
        gameObject.SetActive(false);
    }
}
