using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    [Header("Movement")]
    public float movementSpeed = 10f;
    public float rotationSpeed = 180f;

    [Header("Camera")]
    public Transform cameraTransform;

    [Header("Sounds")]
    public AudioClip chompClip;
    public AudioClip deathClip;
    public AudioClip eatGhostClip;

    private AudioSource chompSource;
    private AudioSource sfxSource;
    private Rigidbody rb;
    private Vector2 movementInput;
    private bool isDead = false;

    void Awake()
    {
        if (SceneManager.GetActiveScene().name != "Game")
        {
            enabled = false;
            return;
        }
    }

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        chompSource = gameObject.AddComponent<AudioSource>();
        chompSource.clip = chompClip;
        chompSource.loop = true;
        chompSource.playOnAwake = false;
        chompSource.volume = 0.9f;

        sfxSource = gameObject.AddComponent<AudioSource>();
        sfxSource.playOnAwake = false;
        sfxSource.loop = false;

        if (cameraTransform == null && Camera.main != null)
            cameraTransform = Camera.main.transform;
    }

    void Update()
    {
        if (isDead) return;

        var kb = Keyboard.current;
        if (kb == null) return;

        float h = 0f, v = 0f;
        if (kb.leftArrowKey.isPressed  || kb.aKey.isPressed) h = -1f;
        if (kb.rightArrowKey.isPressed || kb.dKey.isPressed) h =  1f;
        if (kb.downArrowKey.isPressed  || kb.sKey.isPressed) v = -1f;
        if (kb.upArrowKey.isPressed    || kb.wKey.isPressed) v =  1f;
        movementInput = new Vector2(h, v);

        bool isMoving = rb.linearVelocity.magnitude > 0.1f;
        if (isMoving && !chompSource.isPlaying) chompSource.Play();
        else if (!isMoving && chompSource.isPlaying) chompSource.Stop();
    }

    void FixedUpdate()
    {
        if (isDead) return;
        MovePlayer();
    }

    void MovePlayer()
    {
        if (movementInput.sqrMagnitude < 0.01f)
        {
            rb.linearVelocity = new Vector3(0f, rb.linearVelocity.y, 0f);
            return;
        }

        Vector3 camForward = cameraTransform.forward;
        Vector3 camRight   = cameraTransform.right;
        camForward.y = 0f;
        camRight.y   = 0f;
        camForward.Normalize();
        camRight.Normalize();

        Vector3 moveDirection = (camForward * movementInput.y + camRight * movementInput.x).normalized;

        rb.linearVelocity = new Vector3(
            moveDirection.x * movementSpeed,
            rb.linearVelocity.y,
            moveDirection.z * movementSpeed
        );

        Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
        transform.rotation = Quaternion.RotateTowards(
            transform.rotation,
            targetRotation,
            rotationSpeed * Time.fixedDeltaTime
        );
    }

    public void EatGhost()
    {
        sfxSource.PlayOneShot(eatGhostClip);
    }

    public void Die()
    {
        if (isDead) return;
        isDead = true;

        movementInput = Vector2.zero;
        rb.linearVelocity = Vector3.zero;
        //GetComponent<Collider>().enabled = false;
        chompSource.Stop();

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        sfxSource.PlayOneShot(deathClip);
        Invoke(nameof(LoadMainMenu), 1.5f);
    }

    void LoadMainMenu()
    {
        SceneManager.LoadScene(0);
    }
}