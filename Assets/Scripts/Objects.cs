using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Objects : MonoBehaviour
{
    void Start()
    {
        if (GetComponent<Collider>() == null)
        {
            //gameObject.AddComponent<BoxCollider>();
        }
    }

    void Update()
    {

    }

    void OnMouseEnter()
    {
        Debug.Log("Mouse entered the object!");
    }

    void OnMouseExit()
    {
        Debug.Log("Mouse exited the object!");
    }
}