using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Player : PhysicsObject
{
    private static Player instance;
    public static Player Instance
    {
        get
        {
            if (instance == null) instance = GameObject.FindObjectOfType<Player>();
            return instance;
        }
    }

    [Header("Attributes")]
    //[SerializeField] private float activeDuration;
    private int maxHealth = 100;
    [SerializeField] private float jumpPower;
    [SerializeField] private float speed;
    [SerializeField] private float runSpeed;
    public int attackPower;

    public float health;


    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            gameObject.name = "New Player";
        }
    }

    void Start()
    {
        health = maxHealth; // Initialize health to maxHealth at the start
        //UpdateUI();
        //SpawnPlayer();
    }

    void Update()
    {
        float currentSpeed = Input.GetKey(KeyCode.LeftShift) ? runSpeed : speed;
        targetVelocity = new Vector2(Input.GetAxis("Horizontal") * currentSpeed, 0);

        if (Input.GetButtonDown("Jump") && grounded)
        {
            velocity.y = jumpPower;
        }

        if (targetVelocity.x < -0.01)
        {
            transform.localScale = new Vector2(1f, 1f);
        }
        else if (targetVelocity.x > 0.01)
        {
            transform.localScale = new Vector2(-1f, 1f);
        }

        if (Input.GetButtonDown("Fire1"))
        {
            //ActiveAttack();
        }

        if (health <= 0)
        {
            //die();
        }
    }

    public void SpawnPlayer()
    {
        transform.position = GameObject.Find("playerSpawner").transform.position;
    }
}