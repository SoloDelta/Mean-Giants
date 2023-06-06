using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerController : MonoBehaviour, IDamage
{
    [Header("----- Components -----")]
    [SerializeField] CharacterController controller;

    [Header("----- Player Stats -----")]
    [SerializeField] int health;
    [Range(0, 10)][SerializeField] float playerSpeed;
    [Range(8, 25)][SerializeField] float jumpHeight;
    [Range(10, 50)][SerializeField] float gravityValue;
    [Range(1, 3)][SerializeField] int jumpMax;
    [Range(2, 5)][SerializeField] int sprint;

    [Header("----- Gun Stats -----")]
    [Range(0.1f, 3)][SerializeField] float shootRate;
    [Range(1, 10)][SerializeField] int shootDamage;
    [Range(1, 1000)][SerializeField] int shootDistance;
    [SerializeField] GameObject hitEffect;

    private int jumpedTimes;
    [SerializeField] private Vector3 playerVelocity;
    private bool groundedPlayer;
    private Vector3 move;
    bool isShooting;
    private Animator animate;

    private void Start()
    {
        animate = GetComponentInChildren<Animator>();
    }

    private void Update()
    {
        Movement();

        if (Input.GetButton("Shoot") && !isShooting)
        {
            StartCoroutine(shoot());
        }
    }

    void Movement()
    {
        groundedPlayer = controller.isGrounded;
        if (groundedPlayer && playerVelocity.y <= 0)
        {
            playerVelocity.y = 0;
            jumpedTimes = 0;
        }

        move = (transform.right * Input.GetAxis("Horizontal")) + (transform.forward * Input.GetAxis("Vertical"));
        controller.Move(playerSpeed * Time.deltaTime * move);

        // Changes the height position of the player
        if (Input.GetButtonDown("Jump") && jumpedTimes < jumpMax)
        {
            jumpedTimes++;
            playerVelocity.y = jumpHeight;
        }

        else if (Input.GetButtonDown("Sprint"))
        {
            Run();
        }
        else if (Input.GetButtonUp("Sprint"))
        {
            Walk();
        }
        else if(playerVelocity.z <= 0)
        {
            Idle();
        }

        playerVelocity.y -= gravityValue * Time.deltaTime;
        controller.Move(playerVelocity * Time.deltaTime);
    }
    IEnumerator shoot()
    {
        isShooting = true;
        RaycastHit hit;
        if (Physics.Raycast(Camera.main.ViewportPointToRay(new Vector2(0.5f, 0.5f)), out hit, shootDistance))
        {
            IDamage damageable = hit.collider.GetComponent<IDamage>();
            if (damageable != null)
            {
                damageable.TakeDamage(shootDamage);
            }
        }
        yield return new WaitForSeconds(shootRate);
        isShooting = false;
    }

    public void TakeDamage(int damage)
    {
        health -= damage;
    }

    private void Idle()
    {
        animate.SetFloat("Speed", 0, 0.1f, Time.deltaTime);
    }

    private void Walk() 
    {
        playerSpeed -= sprint;
        animate.SetFloat("Speed", 0.5f, 0.1f, Time.deltaTime);
    }

    private void Run()
    {
        playerSpeed += sprint;
        animate.SetFloat("Speed", 1, 0.1f, Time.deltaTime);
    }
}
