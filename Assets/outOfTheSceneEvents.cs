using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutOfTheSceneEvents : MonoBehaviour
{
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }
}
