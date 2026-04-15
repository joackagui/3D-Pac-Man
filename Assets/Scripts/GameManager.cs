using UnityEngine;
using UnityEngine.SceneManagement;
using System;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public static event Action OnPowerPelletEaten;
    private int score = 0;
    private int totalPellets = 0;
    private int collectedPellets = 0;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else { Destroy(gameObject); return; }
    }

    void Start()
    {
        PelletManager pm = FindAnyObjectByType<PelletManager>();
        if (pm != null)
            totalPellets = pm.GetPelletsCount();

        Debug.Log("Total pellets found: " + totalPellets);
        UIManager.Instance?.UpdateScore(score);
    }

    public void AddScore(int points)
    {
        score += points;
        collectedPellets++;

        Debug.Log($"Collected {collectedPellets}/{totalPellets}");
        UIManager.Instance?.UpdateScore(score);

        if (totalPellets > 0 && collectedPellets >= totalPellets)
            Win();
    }

    public void PowerPelletEaten()
    {
        OnPowerPelletEaten?.Invoke();
    }

    void Win()
    {
        Invoke(nameof(LoadMainMenu), 0.5f);
    }

    void LoadMainMenu()
    {
        SceneManager.LoadScene(0);
    }
}