using UnityEngine;
using System.Collections;

public class Ghost : MonoBehaviour
{
    [Header("Movement")]
    public float normalSpeed = 5f;

    [SerializeField] private float currentSpeed;

    [Header("Materials")]
    public Material normalMaterial;
    public Material frightenedMaterial;

    [Header("Sound")]
    public AudioClip movementClip;
    public AudioClip frightenedClip;

    private UnityEngine.AI.NavMeshAgent agent;
    private Player player;
    private AudioSource audioSource;
    private bool isActive = false;
    private bool isFrightened = false;
    private bool isRespawning = false;

    private Vector3 spawnPosition;
    private Quaternion spawnRotation;

    private MeshRenderer meshRenderer;
    private float frightenTimer = 0f;
    private const float frightenDuration = 4f;

    void Start()
    {
        agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        agent.speed = normalSpeed;
        currentSpeed = normalSpeed;

        agent.stoppingDistance = 0.3f;
        agent.autoBraking = false;

        player = FindAnyObjectByType<Player>();
        isActive = (player != null);

        spawnPosition = transform.position;
        spawnRotation = transform.rotation;

        meshRenderer = GetComponent<MeshRenderer>();

        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.loop = true;
        audioSource.playOnAwake = false;
        audioSource.volume = 0.6f;

        GameManager.OnPowerPelletEaten += EnterFrightenedMode;
    }

    void OnDestroy()
    {
        GameManager.OnPowerPelletEaten -= EnterFrightenedMode;
    }

    void Update()
    {
        if (!isActive || player == null || isRespawning || !agent.enabled) return;

        if (isFrightened)
        {
            frightenTimer -= Time.deltaTime;
            if (frightenTimer <= 0f)
            {
                ExitFrightenedMode();
                return;
            }

            Vector3 fleeDirection = (transform.position - player.transform.position).normalized;
            Vector3 fleeTarget = transform.position + fleeDirection * 10f;
            agent.SetDestination(fleeTarget);
        }
        else
        {
            agent.SetDestination(player.transform.position);
        }

        HandleMovementSound();
    }

    void EnterFrightenedMode()
    {
        if (isRespawning) return;

        isFrightened = true;
        frightenTimer = frightenDuration;

        float slowSpeed = normalSpeed / 3f;
        agent.speed = slowSpeed;
        currentSpeed = slowSpeed;          // visible in Inspector

        if (meshRenderer != null && frightenedMaterial != null)
            meshRenderer.material = frightenedMaterial;

        audioSource.Stop();
        if (frightenedClip != null)
        {
            audioSource.clip = frightenedClip;
            audioSource.Play();
        }
    }

    void ExitFrightenedMode()
    {
        isFrightened = false;
        frightenTimer = 0f;

        agent.speed = normalSpeed;
        currentSpeed = normalSpeed;        // visible in Inspector

        if (meshRenderer != null && normalMaterial != null)
            meshRenderer.material = normalMaterial;

        audioSource.Stop();
        audioSource.clip = movementClip;
    }

    void HandleMovementSound()
    {
        bool moving = agent.velocity.magnitude > 0.1f;
        if (moving && !audioSource.isPlaying) audioSource.Play();
        else if (!moving && audioSource.isPlaying) audioSource.Stop();
    }

    public void GetEaten()
    {
        isRespawning = true;
        ExitFrightenedMode();
        StartCoroutine(Respawn());
    }

    private IEnumerator Respawn()
    {
        meshRenderer.enabled = false;
        agent.enabled = false;
        transform.position = spawnPosition;
        transform.rotation = spawnRotation;

        yield return new WaitForSeconds(1f);

        agent.enabled = true;
        meshRenderer.enabled = true;

        isRespawning = false;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!isActive || isRespawning) return;

        if (collision.gameObject.CompareTag("Player"))
        {
            if (isFrightened)
            {
                collision.gameObject.GetComponent<Player>().EatGhost();
                GameManager.Instance.AddScore(200);
                GetEaten();
            }
            else
            {
                collision.gameObject.GetComponent<Player>().Die();
            }
        }
    }
}