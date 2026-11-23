using System.Collections;
using UnityEngine;

public class TrainerAnimator : MonoBehaviour
{
    public Transform armPivot;
    public Transform throwHand;
    public Transform heldBall;
    private Quaternion restRotation;
    private Coroutine current;

    private void Start()
    {
        if (armPivot != null) restRotation = armPivot.localRotation;
    }

    public void PlayThrow()
    {
        if (current != null) StopCoroutine(current);
        current = StartCoroutine(ThrowMotion());
    }

    private IEnumerator ThrowMotion()
    {
        float t = 0f;
        Quaternion raised = restRotation * Quaternion.Euler(-105f, 0, 0);
        while (t < 0.28f)
        {
            t += Time.deltaTime;
            armPivot.localRotation = Quaternion.Slerp(restRotation, raised, t / 0.28f);
            yield return null;
        }
        t = 0f;
        while (t < 0.22f)
        {
            t += Time.deltaTime;
            armPivot.localRotation = Quaternion.Slerp(raised, restRotation * Quaternion.Euler(35f,0,0), t / 0.22f);
            yield return null;
        }
        t = 0f;
        while (t < 0.35f)
        {
            t += Time.deltaTime;
            armPivot.localRotation = Quaternion.Slerp(restRotation * Quaternion.Euler(35f,0,0), restRotation, t / 0.35f);
            yield return null;
        }
        armPivot.localRotation = restRotation;
        current = null;
    }
}
