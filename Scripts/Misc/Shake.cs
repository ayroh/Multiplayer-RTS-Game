using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shake : MonoBehaviour
{
    public AnimationCurve curve;
    public float duration = 1f;
    public int id;
    private void OnEnable()
    {
        EventManager.OnFireTriggeredAction += OnFireTriggered;
    }

    private void OnDisable()
    {
        EventManager.OnFireTriggeredAction -= OnFireTriggered;

    }
    private void OnFireTriggered(int id)
    {
        if(id == this.id)
        {
            StartCoroutine(Shaking());
        }
    }

    IEnumerator Shaking()
    {
        Vector3 startPosition = transform.position;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float strength = curve.Evaluate(elapsedTime / duration);
            transform.position = startPosition + Random.insideUnitSphere * strength;
            yield return null;
        }

        transform.position = startPosition;
    }
}
