using UnityEngine;

public class PowerPellet : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            GameManager.Instance.AddScore(100);
            Destroy(this.gameObject);
        }
    }
}