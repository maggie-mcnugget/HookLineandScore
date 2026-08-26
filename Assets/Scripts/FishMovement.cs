using UnityEngine;

public class FishMovement : MonoBehaviour
{
    public float moveSpeed = 1f;
    public float moveDistance = 2f;

    public float chaseSpeed = 2f;
    public float bobberReachDistance = 0.2f;

    private Vector3 startPosition;
    private Vector3 targetPosition;

    private bool movingRight = true;
    private bool chasingBobber = false;

    private bool returningHome = false;
    private bool attachedToBobber = false;

    private Transform bobberTarget;
    private FishSpawner fishSpawner;

    private SpriteRenderer spriteRenderer;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        startPosition = transform.position;

        // Randomize these so every fish doesn't move exactly the same
        moveSpeed = Random.Range(0.5f, 1.2f);
        moveDistance = Random.Range(1.5f, 3f);

        targetPosition = startPosition + Vector3.right * moveDistance;

        spriteRenderer.flipX = true;
    }

    void Update()
    {
        // Once caught, the fish is attached to the bobber, so don't run any normal movement
        if (attachedToBobber)
        {
            return;
        }

        // Fish is going back to where it originally spawned
        if (returningHome)
        {
            ReturnHome();
            return;
        }

        // Fish is currently chasing the bobber
        if (chasingBobber)
        {
            ChaseBobber();
            return;
        }

        // Otherwise, just swim normally
        SwimBackAndForth();
    }

    private void SwimBackAndForth()
    {
        transform.position = Vector3.MoveTowards(
            transform.position,
            targetPosition,
            moveSpeed * Time.deltaTime);

        if (Vector3.Distance(transform.position, targetPosition) < 0.01f)
        {
            if (movingRight)
            {
                targetPosition = startPosition - Vector3.right * moveDistance;
                movingRight = false;

                spriteRenderer.flipX = false;
            }
            else
            {
                targetPosition = startPosition + Vector3.right * moveDistance;
                movingRight = true;

                spriteRenderer.flipX = true;
            }
        }
    }

    private void ChaseBobber()
    {
        Vector2 direction = bobberTarget.position - transform.position;

        transform.position = Vector2.MoveTowards(
            transform.position,
            bobberTarget.position,
            chaseSpeed * Time.deltaTime);

        // Flip the sprite so it faces the direction it is moving
        if (direction.x > 0)
        {
            spriteRenderer.flipX = true;
        }
        else if (direction.x < 0)
        {
            spriteRenderer.flipX = false;
        }

        if (Vector2.Distance(transform.position, bobberTarget.position) <= bobberReachDistance)
        {
            fishSpawner.FishReachedBobber(this);
        }
    }

    public void ChaseBobber(Transform bobber, FishSpawner spawner)
    {
        bobberTarget = bobber;
        fishSpawner = spawner;

        chasingBobber = true;
    }

    public void StopChasing()
    {
        chasingBobber = false;
        bobberTarget = null;

        // Other fish go back to where they spawned
        returningHome = true;

        if (movingRight)
        {
            targetPosition = startPosition + Vector3.right * moveDistance;
            spriteRenderer.flipX = true;
        }
        else
        {
            targetPosition = startPosition - Vector3.right * moveDistance;
            spriteRenderer.flipX = false;
        }
    }

    public void AttachToBobber(Transform bobber)
    {
        chasingBobber = false;
        attachedToBobber = true;

        bobberTarget = bobber;

        // Make the fish a child of the bobber so it follows it around
        transform.SetParent(bobber);

        transform.localPosition = new Vector3(
            0f,
            -1.2f,
            0f);

        spriteRenderer.flipX = false;
    }

    private void ReturnHome()
    {
        Vector2 direction = startPosition - transform.position;

        transform.position = Vector3.MoveTowards(
            transform.position,
            startPosition,
            chaseSpeed * Time.deltaTime);

        if (direction.x > 0)
        {
            spriteRenderer.flipX = true;
        }
        else if (direction.x < 0)
        {
            spriteRenderer.flipX = false;
        }

        // Once the fish reaches its starting position, let it go back to normal swimming
        if (Vector2.Distance(transform.position, startPosition) < 0.01f)
        {
            transform.position = startPosition;

            returningHome = false;

            if (movingRight)
            {
                targetPosition = startPosition + Vector3.right * moveDistance;
                spriteRenderer.flipX = true;
            }
            else
            {
                targetPosition = startPosition - Vector3.right * moveDistance;
                spriteRenderer.flipX = false;
            }
        }
    }
}