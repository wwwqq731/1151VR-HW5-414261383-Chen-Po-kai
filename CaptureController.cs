using System.Collections;
using UnityEngine;

public class CaptureController : MonoBehaviour
{
    [HideInInspector] public Transform creature;
    private TrainerAnimator trainer;
    private Transform ball;
    private Vector3 creatureStartScale;
    private Vector3 creatureStartPosition;
    private bool busy;
    private bool captured;
    private GUIStyle titleStyle, helpStyle, statusStyle;

    private void Start()
    {
        trainer = FindObjectOfType<TrainerAnimator>();
        ball = GameObject.Find("Held_Pokeball")?.transform;
        if (creature != null)
        {
            creatureStartScale = creature.localScale;
            creatureStartPosition = creature.position;
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
        {
            if (!busy && !captured) StartCoroutine(ThrowAndCapture());
        }
        if (Input.GetKeyDown(KeyCode.R)) ResetScene();
    }

    private IEnumerator ThrowAndCapture()
    {
        busy = true;
        if (trainer != null) trainer.PlayThrow();
        if (ball == null) { busy = false; yield break; }

        ball.SetParent(null, true);
        Vector3 start = ball.position;
        Vector3 end = new Vector3(4.2f, 1.25f, 0.2f);
        float t = 0;
        float duration = 1.15f;
        while (t < duration)
        {
            t += Time.deltaTime;
            float p = Mathf.Clamp01(t / duration);
            Vector3 pos = Vector3.Lerp(start, end, p);
            pos.y += Mathf.Sin(p * Mathf.PI) * 3.0f;
            ball.position = pos;
            ball.Rotate(0, 720 * Time.deltaTime, 0, Space.World);
            yield return null;
        }

        // Flash and absorb the creature
        CreateBurst(end, new Color(1f,0.85f,0.25f));
        if (creature != null)
        {
            Vector3 original = creature.position;
            t = 0;
            while (t < 0.75f)
            {
                t += Time.deltaTime;
                float p = Mathf.Clamp01(t / 0.75f);
                creature.localScale = Vector3.Lerp(creatureStartScale, Vector3.zero, p);
                creature.position = Vector3.Lerp(original, end, p);
                yield return null;
            }
            creature.gameObject.SetActive(false);
        }

        // Ball closes, shakes, then confirms capture
        t = 0;
        while (t < 1.0f)
        {
            t += Time.deltaTime;
            ball.position = end + Vector3.up * (Mathf.Sin(t * 22f) * 0.12f);
            ball.Rotate(0, 360 * Time.deltaTime, 0, Space.World);
            yield return null;
        }
        CreateBurst(end, new Color(0.35f,1f,0.55f));
        captured = true;
        busy = false;
    }

    private void CreateBurst(Vector3 position, Color color)
    {
        var fx = new GameObject("Capture_Sparkle_Effect");
        fx.transform.position = position;
        var ps = fx.AddComponent<ParticleSystem>();
        var main = ps.main;
        main.startLifetime = 0.75f;
        main.startSpeed = 4.5f;
        main.startSize = 0.16f;
        main.startColor = color;
        main.maxParticles = 80;
        main.loop = false;
        var emission = ps.emission;
        emission.SetBursts(new ParticleSystem.Burst[] { new ParticleSystem.Burst(0f, 35) });
        var shape = ps.shape;
        shape.shapeType = ParticleSystemShapeType.Sphere;
        shape.radius = 0.35f;
        var renderer = ps.GetComponent<ParticleSystemRenderer>();
        renderer.material = new Material(Shader.Find("Sprites/Default"));
        Destroy(fx, 2.5f);
    }

    private void ResetScene()
    {
        StopAllCoroutines();
        if (creature != null)
        {
            creature.gameObject.SetActive(true);
            creature.localScale = creatureStartScale;
            creature.position = creatureStartPosition;
        }
        if (ball != null)
        {
            ball.SetParent(null, true);
            ball.position = new Vector3(-3.62f,1.0f,0.2f);
            ball.rotation = Quaternion.identity;
        }
        captured = false;
        busy = false;
    }

    private void OnGUI()
    {
        if (titleStyle == null)
        {
            titleStyle = new GUIStyle(GUI.skin.label) { fontSize = 25, fontStyle = FontStyle.Bold, alignment = TextAnchor.MiddleCenter };
            titleStyle.normal.textColor = Color.white;
            helpStyle = new GUIStyle(GUI.skin.label) { fontSize = 16, alignment = TextAnchor.MiddleCenter };
            helpStyle.normal.textColor = Color.white;
            statusStyle = new GUIStyle(GUI.skin.label) { fontSize = 22, fontStyle = FontStyle.Bold, alignment = TextAnchor.MiddleCenter };
            statusStyle.normal.textColor = new Color(1f,0.9f,0.35f);
        }
        GUI.Box(new Rect(Screen.width/2f-245, 12, 490, 92), "");
        GUI.Label(new Rect(Screen.width/2f-235, 17, 470, 34), "MEADOW CAPTURE ADVENTURE", titleStyle);
        GUI.Label(new Rect(Screen.width/2f-235, 52, 470, 28), "SPACE / Left Click: Throw Poké Ball     R: Reset", helpStyle);
        if (captured)
            GUI.Label(new Rect(Screen.width/2f-220, 112, 440, 40), "CAPTURE SUCCESS!", statusStyle);
        else if (busy)
            GUI.Label(new Rect(Screen.width/2f-220, 112, 440, 40), "GO, POKÉ BALL!", statusStyle);
        else
            GUI.Label(new Rect(Screen.width/2f-220, 112, 440, 40), "A wild creature appeared!", statusStyle);
    }
}
