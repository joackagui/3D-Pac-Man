using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    public float movementSpeed = 10f;

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

        chompSource = gameObject.AddComponent<AudioSource>();
        chompSource.clip = chompClip;
        chompSource.loop = true;
        chompSource.playOnAwake = false;
        chompSource.volume = 0.9f;

        sfxSource = gameObject.AddComponent<AudioSource>();
        sfxSource.playOnAwake = false;
        sfxSource.loop = false;
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
        Vector3 direction = new Vector3(movementInput.x, 0f, movementInput.y);
        rb.linearVelocity = new Vector3(
            direction.x * movementSpeed,
            rb.linearVelocity.y,
            direction.z * movementSpeed
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
        GetComponent<Collider>().enabled = false;
        chompSource.Stop();

        sfxSource.PlayOneShot(deathClip);
        Invoke(nameof(LoadMainMenu), 1.5f);
    }

    void LoadMainMenu()
    {
        SceneManager.LoadScene(0);
    }
}