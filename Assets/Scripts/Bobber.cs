using UnityEngine;
using System.Collections;

public class Bobber : MonoBehaviour
{
    private Vector2 startPosition;

    private float launchAngle;
    private float launchSpeed;

    private float timer;

    public float gravity = 9.81f;

    public float waterHeight = 2f;

    private bool isReeling = false;
    private bool hasLanded = false;
    private bool waitingToLand = false;

    public float landingSlideSpeed = 15f;

    public float reelSpeed = 3f;
    public float landingDelay = 0.3f;

    public Transform reelTarget;

    private GameManager gameManager;

    public FishingMinigame fishingMinigame;

    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
    }

    public void Launch(Vector2 start, float angle, float speed)
    {
        startPosition = start;

        // Unity's trig functions use radians, so convert the angle from degrees before using Sin/Cos
        launchAngle = angle * Mathf.Deg2Rad;

        launchSpeed = speed;

        timer = 0;

        hasLanded = false;
        waitingToLand = false;
        isReeling = false;
    }

    public void StartReeling(Transform target)
    {
        isReeling = true;
        reelTarget = target;
    }

    void Update()
    {
        // REELING
        if (isReeling)
        {
            Vector2 targetPosition = new Vector2(
                reelTarget.position.x,
                transform.position.y);

            // Start with the normal reel speed
            float currentReelSpeed = reelSpeed;

            // The minigame can change this speed depending on whether the indicator is in red, yellow, or green
            if (fishingMinigame != null)
            {
                currentReelSpeed =
                    fishingMinigame.GetCurrentReelSpeed();
            }

            transform.position = Vector2.MoveTowards(
                transform.position,
                targetPosition,
                currentReelSpeed * Time.deltaTime);

            // Once the bobber gets close enough to the player, the fishing is finished
            if (Mathf.Abs(transform.position.x - reelTarget.position.x) < 0.1f)
            {
                gameManager.BobberReturned();
                Destroy(gameObject);
            }

            return;
        }

        // Once bobber has landed, don't continue the projectile math
        if (hasLanded)
        {
            return;
        }

        // Small movement after hitting the water to make landing look a little less abrupt
        if (waitingToLand)
        {
            transform.position += Vector3.right *
                landingSlideSpeed * Time.deltaTime;

            landingSlideSpeed = Mathf.MoveTowards(
                landingSlideSpeed,
                0f,
                15f * Time.deltaTime);

            return;
        }

        // PROJECTILE MOTION
        timer += Time.deltaTime;

        // Horizontal projectile equation: x = v*cos(angle)*t
        float x = launchSpeed * Mathf.Cos(launchAngle) * timer;

        // Vertical projectile equation: y = v*sin(angle)*t - 1/2*g*t²
        float y = launchSpeed * Mathf.Sin(launchAngle) * timer
                - 0.5f * gravity * timer * timer;

        transform.position = startPosition + new Vector2(x, y);

        // Check if the bobber has reached the water
        if (transform.position.y <= waterHeight)
        {
            // Keep it exactly on the water instead of letting it continue falling underneath it
            transform.position = new Vector3(
                transform.position.x,
                waterHeight,
                transform.position.z);

            // Calculate the horizontal part of the launch velocity so we can use it for the little landing slide
            landingSlideSpeed = launchSpeed * Mathf.Cos(launchAngle);

            waitingToLand = true;

            StartCoroutine(LandAfterDelay());
        }
    }

    private IEnumerator LandAfterDelay()
    {
        // Wait a tiny amount of time before officially landing
        yield return new WaitForSeconds(landingDelay);

        waitingToLand = false;
        hasLanded = true;

        gameManager.BobberLanded(transform);
    }
}