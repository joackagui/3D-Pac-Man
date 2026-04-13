using UnityEngine;

public class Ghost : MonoBehaviour
{
    public float speed = 5f;

    [Header("Sound")]
    public AudioClip movementClip;

    private UnityEngine.AI.NavMeshAgent agent;
    private Player player;
    private AudioSource audioSource;
    private bool isActive = false;

    void Start()
    {
        agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        agent.speed = speed;
        agent.stoppingDistance = 0.3f;
        agent.autoBraking = false;

        player = FindAnyObjectByType<Player>();
        isActive = (player != null);

        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.clip = movementClip;
        audioSource.loop = true;
        audioSource.playOnAwake = false;
        audioSource.volume = 0.6f;
    }

    void Update()
    {
        if (!isActive || player == null) return;

        agent.SetDestination(player.transform.position);

        bool moving = agent.velocity.magnitude > 0.1f;
        if (moving && !audioSource.isPlaying) audioSource.Play();
        else if (!moving && audioSource.isPlaying) audioSource.Stop();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!isActive) return;
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.gameObject.GetComponent<Player>().Die();
        }
    }
}