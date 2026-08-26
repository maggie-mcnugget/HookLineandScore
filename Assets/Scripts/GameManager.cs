using UnityEngine;
using UnityEngine.EventSystems;

//game manager controls overall game flow, including casting, reeling, and starting the fishing minigame

public class GameManager : MonoBehaviour
{
    public CameraFollow cameraFollow;

    public FishSpawner fishSpawner;
    public GameObject reelButton;

    public ScoreManager scoreManager;

    public FishingMinigame fishingMinigame;

    //where bobber starts its cast from
    public Transform castPoint;
    public GameObject bobberPrefab;

    //strength and speed of cast
    public float power = 8;
    public float angle = 45;

    public LineRenderer trajectoryLine;

    //number of points used to draw the trajectory line and time between each point
    public int trajectoryPoints = 30;
    public float timeBetweenPoints = 0.1f;

    public float gravity = 9.81f;

    //min and max casting power
    public float minPower = 2f;
    public float maxPower = 15f;

    //screen positions for casting power, used to calculate power based on mouse position
    public float powerStartX = 0f;
    public float powerEndX = 1920f;


    //were used for tracking drag position, was scrapped but might be used again in the future if drag system is expanded
    private Vector2 dragStart;
    private Vector2 dragCurrent;

    private bool isDragging = false;
    private bool waitingForCast = false;
    private bool hasBobberInWater = false;

    private GameObject currentBobber;


    private void Start()
    {
        // Don't show the Reel button until a bobber has been cast
        reelButton.SetActive(false);

        // Don't show the trajectory until the player starts aiming
        trajectoryLine.enabled = false;
    }

    void Update()
    {
        if (isDragging)
        {
            UpdateAim(Input.mousePosition);

            // Wait for the player to click to actually cast, The EventSystem check stops clicks on UI buttons from casting
            if (waitingForCast &&
                Input.GetMouseButtonDown(0) &&
                !EventSystem.current.IsPointerOverGameObject())
            {
                waitingForCast = false;
                FinishAim();
            }
        }

        if (trajectoryLine.enabled)
        {
            DrawTrajectory();
        }
    }

    public void Cast()
    {
        Debug.Log("Cast!");

        // Create a new bobber at the cast point
        currentBobber = Instantiate(
            bobberPrefab,
            castPoint.position,
            Quaternion.identity);

        cameraFollow.StartFollowing(currentBobber.transform);

        Bobber bobberScript = currentBobber.GetComponent<Bobber>();

        // Give the bobber access to the minigame so it can use the minigame's current reeling speed
        bobberScript.fishingMinigame = fishingMinigame;

        bobberScript.gravity = gravity;
        bobberScript.Launch(castPoint.position, angle, power);

        hasBobberInWater = true;

        // The player can now try to reel in
        reelButton.SetActive(true);
    }

    public void Reel()
    {
        Debug.Log("Reel!");

        if (currentBobber == null)
        {
            Debug.Log("No bobber!");
            return;
        }

        // Don't start the reeling minigame if a fish hasn't reached the bobber
        if (fishSpawner.GetCaughtFish() == null)
        {
            Debug.Log("No fish hooked!");
            return;
        }

        StartFishingMinigame();

        Bobber bobberScript =
            currentBobber.GetComponent<Bobber>();

        bobberScript.StartReeling(castPoint);
    }

    public void BeginAim()
    {
        // Can't cast another bobber while the current one is still out
        if (hasBobberInWater)
            return;

        isDragging = true;
        waitingForCast = true;

        trajectoryLine.enabled = true;
    }

    public void UpdateAim(Vector2 fingerPosition)
    {
        // Convert the mouse/finger position from the screen into a position in the game world
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(fingerPosition);

        worldPosition.z = 0;

        Vector2 direction = worldPosition - castPoint.position;

        // POWER
        // Converts the player's horizontal position into a 0-1 value that we can use to calculate the cast power
        float powerAmount = Mathf.InverseLerp(
            powerEndX,
            powerStartX,
            fingerPosition.x);

        // Converts 0-1 value into  actual min/max power
        power = Mathf.Lerp(
            minPower,
            maxPower,
            powerAmount);

        // ANGLE
        // Atan2 gives us the angle between the cast point and the mouse
        angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        // Prevent the player from aiming too low or too high
        angle = Mathf.Clamp(angle, 10f, 80f);
    }

    public void FinishAim()
    {
        isDragging = false;

        trajectoryLine.enabled = false;

        Cast();
    }

    public void BobberLanded(Transform bobber)
    {
        Debug.Log("Bobber landed! Fish are chasing!");

        // Once the bobber lands, tell the fish to start chasing it
        fishSpawner.StartFishChase(bobber);
    }

    public void StartFishingMinigame()
    {
        Debug.Log("Starting fishing minigame!");

        fishingMinigame.StartMinigame();
    }

    public void BobberReturned()
    {
        // Make sure the reeling UI disappears when the bobber gets back
        fishingMinigame.StopMinigame();

        cameraFollow.StopFollowing();

        // Get the fish that was caught
        FishMovement caughtFish = fishSpawner.GetCaughtFish();

        if (caughtFish != null)
        {
            FishScore fishScore = caughtFish.GetComponent<FishScore>();

            if (fishScore != null)
            {
                // Add the caught fish's score to the player's total
                scoreManager.AddScore(fishScore.Score);

                Debug.Log("Fish caught! +" + fishScore.Score + " points");
            }
        }

        // Reset everything so another cast can happen
        currentBobber = null;
        hasBobberInWater = false;

        reelButton.SetActive(false);

        Debug.Log("Bobber returned!");
    }

    void DrawTrajectory()
    {
        trajectoryLine.positionCount = trajectoryPoints;

        // Convert degrees to radians because Mathf.Sin/Cos use radians
        float angleRad = angle * Mathf.Deg2Rad;

        for (int i = 0; i < trajectoryPoints; i++)
        {
            float t = i * timeBetweenPoints;

            // Projectile motion equations:
            // x = v*cos(angle)*t
            // y = v*sin(angle)*t - 1/2*g*t²
            float x = power * Mathf.Cos(angleRad) * t;

            float y = power * Mathf.Sin(angleRad) * t
                    - 0.5f * gravity * t * t;

            Vector3 point = castPoint.position + new Vector3(x, y, 0);

            trajectoryLine.SetPosition(i, point);
        }
    }
}
