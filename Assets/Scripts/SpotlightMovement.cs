using UnityEngine;

public class SpotlightMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float minX = -5f; // Minimum X position
    public float maxX = 5f;  // Maximum X position
    public float minY = -1f; // Minimum Y position
    public float maxY = 1f;  // Maximum Y position
    public float moveDurationMin = 2f; // Minimum time for movement
    public float moveDurationMax = 5f; // Maximum time for movement

    [Header("Pause Settings")]
    public float minPauseDuration = 0.5f; // Minimum pause duration
    public float maxPauseDuration = 2f;  // Maximum pause duration
    public float initialPauseDuration = 2f; // Pause duration before first move

    private Vector3 initialPosition; // Store the initial position of the spotlight

    void Start()
    {
         // Store the initial position
        initialPosition = transform.position;

        // Pause for the initial duration before starting the first move
        Invoke(nameof(MoveSpotlight), initialPauseDuration);
    }

    void MoveSpotlight()
    {
        // Generate a random target position within the defined range
        Vector3 targetPosition = new Vector3(
            initialPosition.x + Random.Range(minX, maxX),
            initialPosition.y + Random.Range(minY, maxY),
            transform.position.z
        );

        // Generate a random duration for the movement
        float moveDuration = Random.Range(moveDurationMin, moveDurationMax);

        // Move the spotlight to the target position
        LeanTween.move(gameObject, targetPosition, moveDuration)
            .setEase(LeanTweenType.easeInOutSine)
            .setOnComplete(() =>
            {
                // Pause before the next movement
                float pauseDuration = Random.Range(minPauseDuration, maxPauseDuration);
                Invoke(nameof(MoveSpotlight), pauseDuration);
            });
    }
}
