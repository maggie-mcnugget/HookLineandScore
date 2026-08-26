using System.Collections.Generic;
using UnityEngine;

public class FishSpawner : MonoBehaviour
{
    [Header("Fish Prefabs")]
    public GameObject smallFishPrefab;
    public GameObject mediumFishPrefab;
    public GameObject largeFishPrefab;

    [Header("Spawn Chances")]
    [Range(0, 100)]
    public int smallChance = 60;

    [Range(0, 100)]
    public int mediumChance = 30;

    [Range(0, 100)]
    public int largeChance = 10;

    [Header("Spawn Settings")]
    public BoxCollider2D spawnArea;

    public int maxFish = 5;

    public float spawnInterval = 2f;

    // Keeps track of all the fish currently spawned
    private List<GameObject> spawnedFish = new List<GameObject>();

    private FishMovement caughtFish;
    private Transform currentBobber;
    private bool fishHasBeenCaught = false;

    private GameManager gameManager;

    private void Start()
    {
        gameManager = FindObjectOfType<GameManager>();

        // Calls TrySpawnFish immediately, then every spawnInterval seconds
        InvokeRepeating(
            nameof(TrySpawnFish),
            0f,
            spawnInterval);
    }

    private void TrySpawnFish()
    {
        // Remove destroyed fish from our list
        spawnedFish.RemoveAll(fish => fish == null);

        if (spawnedFish.Count >= maxFish)
            return;

        SpawnFish();
    }

    private void SpawnFish()
    {
        GameObject fishPrefab = ChooseFishPrefab();

        if (fishPrefab == null)
            return;

        Vector2 spawnPosition = GetRandomSpawnPosition();

        GameObject newFish = Instantiate(
            fishPrefab,
            spawnPosition,
            Quaternion.identity);

        spawnedFish.Add(newFish);
    }

    private GameObject ChooseFishPrefab()
    {
        int totalChance = smallChance + mediumChance + largeChance;

        if (totalChance <= 0)
            return null;

        // Pick a random number within the total chance range
        int roll = Random.Range(0, totalChance);

        if (roll < smallChance)
        {
            return smallFishPrefab;
        }
        else if (roll < smallChance + mediumChance)
        {
            return mediumFishPrefab;
        }
        else
        {
            return largeFishPrefab;
        }
    }

    private Vector2 GetRandomSpawnPosition()
    {
        Bounds bounds = spawnArea.bounds;

        // Pick a random position inside the spawn box
        float randomX = Random.Range(
            bounds.min.x,
            bounds.max.x);

        float randomY = Random.Range(
            bounds.min.y,
            bounds.max.y);

        return new Vector2(randomX, randomY);
    }

    public void StartFishChase(Transform bobber)
    {
        spawnedFish.RemoveAll(fish => fish == null);

        currentBobber = bobber;
        fishHasBeenCaught = false;
        caughtFish = null;

        // Tell every currently spawned fish to chase the bobber
        foreach (GameObject fish in spawnedFish)
        {
            FishMovement movement = fish.GetComponent<FishMovement>();

            if (movement != null)
            {
                movement.ChaseBobber(bobber, this);
            }
        }
    }

    public void FishReachedBobber(FishMovement winningFish)
    {
        // Prevent multiple fish from being counted as caught
        if (fishHasBeenCaught)
            return;

        fishHasBeenCaught = true;
        caughtFish = winningFish;

        Debug.Log("Fish reached the bobber: " + winningFish.name);

        // Attach the winning fish to the bobber
        winningFish.AttachToBobber(currentBobber);

        // Tell all the other fish to stop chasing
        foreach (GameObject fish in spawnedFish)
        {
            if (fish == null)
                continue;

            FishMovement movement = fish.GetComponent<FishMovement>();

            if (movement != null && movement != winningFish)
            {
                movement.StopChasing();
            }
        }
    }

    public FishMovement GetCaughtFish()
    {
        return caughtFish;
    }
}