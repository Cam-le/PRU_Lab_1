using System.Collections;
using UnityEngine;

public class DiceRoller : MonoBehaviour
{
    public int numberOfSides = 12;
    private int lastRoll;

    private Quaternion originalRotation;

    void Start()
    {
        originalRotation = transform.rotation; 
    }
    public void RollDice()
    {
        lastRoll = Random.Range(1, numberOfSides + 1);
        Debug.Log("Result Roll Dice: " + lastRoll);
        AnimateDiceRoll();
    }

    private void AnimateDiceRoll()
    {
        float randomRotationX = Random.Range(0f, 360f);
        float randomRotationY = Random.Range(0f, 360f);
        float randomRotationZ = Random.Range(0f, 360f);

        transform.Rotate(randomRotationX, randomRotationY, randomRotationZ);
        StartCoroutine(RollCoroutine());
    }

    private IEnumerator RollCoroutine()
    {
        Vector3 originalPosition = transform.position;
        Vector3 targetPosition = originalPosition + new Vector3(0, 0.5f, 0);

        float duration = 0.5f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            transform.position = Vector3.Lerp(originalPosition, targetPosition, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        elapsed = 0f;

        while (elapsed < duration)
        {
            transform.position = Vector3.Lerp(targetPosition, originalPosition, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = originalPosition;
        transform.rotation = originalRotation;
    }

    public int GetLastRoll()
    {
        return lastRoll;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            RollDice();
        }
    }
}