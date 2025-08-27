using Cinemachine;
using UnityEngine;

public class sceneCameraController : MonoBehaviour
{
    private static sceneCameraController instance;
    public static sceneCameraController Instance
    {
        get
        {
            if (instance == null) instance = GameObject.FindObjectOfType<sceneCameraController>();
            return instance;
        }
    }
    [SerializeField] public CinemachineVirtualCamera CinemachineVirtualCamera;

    
    // Start is called before the first frame update
    void Start()
    {
        CinemachineVirtualCamera.Follow = PlayerManager.Instance.transform;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
