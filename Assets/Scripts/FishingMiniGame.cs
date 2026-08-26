using UnityEngine;

public class FishingMinigame : MonoBehaviour
{
    [Header("UI")]
    public RectTransform tensionBar;
    public RectTransform fishIndicator;

    [Header("Reel Zones")]
    public RectTransform redLeft;
    public RectTransform yellowLeft;
    public RectTransform greenZone;
    public RectTransform yellowRight;
    public RectTransform redRight;


    [Header("Fish Movement")]
    public float fishAcceleration = 0.5f;
    public float maxFishSpeed = 0.5f;
    public float directionChangeTime = 2f;

    [Header("Player Control")]
    public float playerStrength = 1.5f;
    public float playerFallSpeed = 1f;

    [Header("Reel Progress")]
    public float reelSpeed = 0.2f;
    public float catchRange = 0.15f;

    [Header("Reeling Speed")]
    public float redReelSpeed = 0.5f;
    public float yellowReelSpeed = 1.5f;
    public float greenReelSpeed = 4f;

    private float fishPosition = 0.5f;
    private float fishVelocity = 0f;

    private float fishDirection = 1f;
    private float directionTimer;

    private float nextDirectionChange;

    private float reelProgress = 0f;

    private bool isPlaying = false;


    void Update()
    {
        if (!isPlaying)
            return;

        UpdateFishMovement();
        UpdatePlayerControl();
        UpdateReelProgress();

        UpdateFishIndicator();

        CheckForWin();
    }

    public void StartMinigame()
    {
        if (isPlaying)
            return;

        gameObject.SetActive(true);

        isPlaying = true;

        // Start fish in center
        fishPosition = 0.5f;

        // Start with a small movement
        fishVelocity = Random.Range(-0.1f, 0.1f);

        fishDirection = Random.value > 0.5f ? 1f : -1f;
        nextDirectionChange = Random.Range(0.8f, 2.5f);

        directionTimer = directionChangeTime;

        reelProgress = 0f;

        UpdateFishIndicator();

        Debug.Log("Fishing minigame started!");
    }

    private void UpdateFishMovement()
    {
        directionTimer -= Time.deltaTime;

        if (directionTimer <= 0f)
        {
            fishDirection = Random.value > 0.5f ? 1f : -1f;

            directionTimer = Random.Range(0.8f, 2.5f);
        }

        // Fish pushes itself in its current direction
        fishVelocity += fishDirection *
                        fishAcceleration *
                        Time.deltaTime;

        // Limit fish speed
        fishVelocity = Mathf.Clamp(
            fishVelocity,
            -maxFishSpeed,
            maxFishSpeed);

        fishPosition += fishVelocity * Time.deltaTime;

        // Hit left edge
        if (fishPosition <= 0f)
        {
            fishPosition = 0f;
            fishVelocity = Mathf.Abs(fishVelocity);
            fishDirection = 1f;
        }

        // Hit right edge
        if (fishPosition >= 1f)
        {
            fishPosition = 1f;
            fishVelocity = -Mathf.Abs(fishVelocity);
            fishDirection = -1f;
        }
    }

    private void UpdatePlayerControl()
    {
        bool holding = Input.GetMouseButton(0);

        if (holding)
        {
            // Holding pushes the indicator RIGHT
            fishVelocity += playerStrength * Time.deltaTime;
        }
        else
        {
            // Releasing causes the indicator to move LEFT
            fishVelocity -= playerFallSpeed * Time.deltaTime;
        }
    }

    private void UpdateReelProgress()
    {
        float distanceFromCenter =
            Mathf.Abs(fishPosition - 0.5f);

        if (distanceFromCenter <= catchRange)
        {
            reelProgress += reelSpeed * Time.deltaTime;
        }
        else
        {
            reelProgress -= reelSpeed * 0.5f * Time.deltaTime;
        }

        reelProgress = Mathf.Clamp01(reelProgress);
    }

    private void UpdateFishIndicator()
    {
        float barWidth = tensionBar.rect.width;

        float x = Mathf.Lerp(
            -barWidth / 2f,
            barWidth / 2f,
            fishPosition);

        fishIndicator.anchoredPosition =
            new Vector2(
                x,
                fishIndicator.anchoredPosition.y);
    }

    public float GetCurrentReelSpeed()
    {
        float indicatorX = fishIndicator.position.x;

        if (IsInsideZone(greenZone, indicatorX))
        {
            return greenReelSpeed;
        }

        if (IsInsideZone(yellowLeft, indicatorX) ||
            IsInsideZone(yellowRight, indicatorX))
        {
            return yellowReelSpeed;
        }

        return redReelSpeed;
    }

    private bool IsInsideZone(RectTransform zone, float xPosition)
    {
        Vector3[] corners = new Vector3[4];

        zone.GetWorldCorners(corners);

        float left = corners[0].x;
        float right = corners[3].x;

        return xPosition >= left && xPosition <= right;
    }

    private void CheckForWin()
    {
        if (reelProgress >= 1f)
        {
            WinMinigame();
        }
    }
    private void WinMinigame()
    {
        isPlaying = false;

        Debug.Log("Fish successfully reeled in!");

        gameObject.SetActive(false);
    }

    public void StopMinigame()
    {
        isPlaying = false;
        gameObject.SetActive(false);
    }
}
