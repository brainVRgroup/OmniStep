using System;
using System.Collections;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

[RequireComponent(typeof(RobotTextAndAudioManager))]
public class RobotController : MonoBehaviour
{
    [SerializeField] NavMeshAgent agent;
    [SerializeField] Animator animator;
    [SerializeField] Canvas robotText;
    [SerializeField] Camera playersView;

    public UnityEvent OnTargetReached;
    public Animator Animator { get => animator; }
    private RobotTextAndAudioManager RobotTextAndAudioManager { get; set; }

    [Range(0, 1f)]
    public float StartAnimTime = 0.25f;
    [Range(0, 1f)]
    public float StopAnimTime = 0.05f;

    private const float ROTATION_SPEED = 5f;

    private bool onTheMove = false;
    private bool facePlayer = true;
    private bool showText = true;

    private float distanceFromObject = 0.45f;

    private void Awake()
    {
        RobotTextAndAudioManager = GetComponent<RobotTextAndAudioManager>();
    }

    void Update()
    {
        if (agent != null && onTheMove)
        {
            // Ensure the agent is not still calculating the path
            if (!agent.pathPending)
            {
                HandleAnimation();

                // Check if the agent has reached the destination
                if (agent.remainingDistance <= agent.stoppingDistance)
                {


                    // Check if the agent is close enough to the destination and has stopped
                    if (!agent.hasPath || agent.velocity.sqrMagnitude == 0f)
                    {
                        Debug.Log("Agent has reached the destination.");
                        onTheMove = false;

                        if (facePlayer)
                            FacePlayer();
                        else
                            OnTargetReached?.Invoke();
                    }
                }
            }
        }
    }

    public async Task SetInstructionsAsync(string key, CancellationToken cancellationToken, bool facePlayer = true)
    {
        // Robot turn to face the player for better text visibility
        if (facePlayer)
            FacePlayer();

        await RobotTextAndAudioManager.SetEntryAsync(key, cancellationToken: cancellationToken);
    }

    public void FacePlayer()
    {
        if (playersView != null)
        {
            StopAllCoroutines(); // Stop any existing rotations to prevent conflicts
            StartCoroutine(RotateTowardsPlayer());
        }
    }

    private IEnumerator RotateTowardsPlayer()
    {
        //animator.transform.eulerAngles = Vector3.zero;

        // Calculate the direction from the NPC to the player
        Vector3 direction = playersView.transform.position - transform.position;
        direction.y = 0; // Keep the rotation only in the horizontal plane

        if (direction.sqrMagnitude > 0.01f) // Ensure direction is not zero to avoid errors
        {
            // Calculate the rotation needed to face the player
            Quaternion targetRotation = Quaternion.LookRotation(direction);

            // Smoothly rotate towards the player
            while (Quaternion.Angle(transform.rotation, targetRotation) > 0.01f)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * ROTATION_SPEED);
                yield return null;
            }

            // Ensure the final rotation is exactly the target rotation
            transform.rotation = targetRotation;
        }

        if (showText)
            robotText.gameObject.SetActive(true);

        OnTargetReached?.Invoke();
        yield return null;
    }

    public void RotateSidewaysToPlayer()
    {
        if (playersView != null)
        {
            StopAllCoroutines(); // Stop any existing rotations to prevent conflicts
            StartCoroutine(RotatePerpendicularToPlayer());
        }
    }

    private IEnumerator RotatePerpendicularToPlayer()
    {
        // Calculate the direction from the NPC to the player
        Vector3 directionToPlayer = playersView.transform.position - transform.position;
        directionToPlayer.y = 0; // Keep the rotation only in the horizontal plane

        if (directionToPlayer.sqrMagnitude > 0.01f) // Ensure direction is not zero to avoid errors
        {
            // Calculate the rotation needed to face perpendicular to the player
            Quaternion lookRotation = Quaternion.LookRotation(directionToPlayer);
            Quaternion perpendicularRotation = lookRotation * Quaternion.Euler(0, 90, 0); // Rotate 90 degrees on the Y axis

            // Smoothly rotate towards the perpendicular direction
            while (Quaternion.Angle(animator.transform.rotation, perpendicularRotation) > 0.01f)
            {
                animator.transform.rotation = Quaternion.Slerp(animator.transform.rotation, perpendicularRotation, Time.deltaTime * ROTATION_SPEED);
                yield return null;
            }

            // Ensure the final rotation is exactly the target rotation
            animator.transform.rotation = perpendicularRotation;
        }
    }

    public void RotateToPlayer()
    {
        if (playersView != null)
        {
            StopAllCoroutines(); // Stop any existing rotations to prevent conflicts
            StartCoroutine(RotateToPlayerCourutine());
        }
    }

    private IEnumerator RotateToPlayerCourutine()
    {
        // Calculate the direction from the NPC to the player
        Vector3 directionToPlayer = playersView.transform.position - transform.position;
        directionToPlayer.y = 0; // Keep the rotation only in the horizontal plane

        if (directionToPlayer.sqrMagnitude > 0.01f) // Ensure direction is not zero to avoid errors
        {
            // Calculate the rotation needed to face perpendicular to the player
            Quaternion lookRotation = Quaternion.LookRotation(directionToPlayer);
            Quaternion perpendicularRotation = lookRotation * Quaternion.Euler(0, 0, 0); // Rotate 90 degrees on the Y axis

            // Smoothly rotate towards the perpendicular direction
            while (Quaternion.Angle(animator.transform.rotation, perpendicularRotation) > 0.01f)
            {
                animator.transform.rotation = Quaternion.Slerp(animator.transform.rotation, perpendicularRotation, Time.deltaTime * ROTATION_SPEED);
                yield return null;
            }

            // Ensure the final rotation is exactly the target rotation
            animator.transform.rotation = perpendicularRotation;
        }
    }

    public void MoveNPCToSpot(Vector3 target, bool showTextOnReach = true, bool facePlayerOnReach = true)
    {
        facePlayer = facePlayerOnReach;
        showText = showTextOnReach;
        agent.SetDestination(target);
        onTheMove = true;
        robotText.gameObject.SetActive(false);

    }

    public void MoveNPCToObject(Vector3 targetObject)
    {
        Vector3 directionToObject = (targetObject - transform.position).normalized;
        Vector3 targetPosition = targetObject - directionToObject * distanceFromObject;

        // Move the NPC to the calculated target position
        facePlayer = false;
        showText = false;
        agent.SetDestination(targetPosition);
        onTheMove = true;
        robotText.gameObject.SetActive(false);

    }

    private void HandleAnimation()
    {
        if (agent.velocity.magnitude > 0)
        {
            animator.SetFloat("Blend", agent.speed, StartAnimTime, Time.deltaTime);
        }
        else if (agent.velocity.magnitude <= 0.001)
        {
            animator.SetFloat("Blend", 0);
        }
    }

}
