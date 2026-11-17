using System.Collections;
using UnityEngine;

public class OfferingOrb : MonoBehaviour
{
    private Vector3 startPosition;

    public void StartOrbMovement(float timeToTarget, Vector3 targetPosition)
    {
        StartCoroutine(MoveToTarget(timeToTarget, targetPosition));
    }

    private IEnumerator MoveToTarget(float timeToTarget, Vector3 targetPosition)
    {
        float timePassed = 0f;

        //Apply movement
        while (timePassed < timeToTarget)
        {
            yield return null;

            timePassed += Time.deltaTime;
            float moveProgression = Mathf.Clamp01(timePassed / timeToTarget);
            float easedMoveProgression = moveProgression < 0.5f ? 2f * moveProgression * moveProgression : 1f - Mathf.Pow(-2f * moveProgression + 2f, 2f) / 2f;

            transform.position = Vector3.Lerp(startPosition, targetPosition, easedMoveProgression);
        }
        Destroy(gameObject);
    }

    private void Awake()
    {
        //Set variables
        startPosition = transform.position;
    }
}