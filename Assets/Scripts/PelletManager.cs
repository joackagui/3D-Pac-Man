using UnityEngine;

public class PelletManager : MonoBehaviour
{
    public static PelletManager Instance { get; private set; }

    private int pelletsCount = 0;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else { Destroy(gameObject); return; }

        // Count here in Awake so GameManager.Start() always gets the correct number
        pelletsCount = transform.childCount;
    }

    public int GetPelletsCount()
    {
        return pelletsCount;
    }
}