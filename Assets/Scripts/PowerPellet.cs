using UnityEngine;

public class PowerPellet : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            GameManager.Instance.AddScore(100);
            GameManager.Instance.PowerPelletEaten();
            Destroy(gameObject);
        }
    }
}