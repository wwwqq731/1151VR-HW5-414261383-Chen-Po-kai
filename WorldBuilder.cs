using UnityEngine;
using UnityEngine.SceneManagement;

public static class WorldBuilder
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void BuildWorld()
    {
        if (Object.FindObjectOfType<CaptureController>() != null) return;
        var root = new GameObject("PokemonCaptureWorld");
        root.AddComponent<CaptureController>();
        CreateEnvironment(root.transform);
        CreateTrainer(root.transform);
        CreateCreature(root.transform);
        CreateCameraAndLight();
    }

    private static Material Mat(Color c, float metallic = 0f, float smooth = 0.25f)
    {
        Shader shader = Shader.Find("Universal Render Pipeline/Lit");
        if (shader == null) shader = Shader.Find("Standard");
        var m = new Material(shader);
        m.color = c;
        if (m.HasProperty("_Metallic")) m.SetFloat("_Metallic", metallic);
        if (m.HasProperty("_Smoothness")) m.SetFloat("_Smoothness", smooth);
        return m;
    }

    private static GameObject Primitive(PrimitiveType type, string name, Transform parent, Vector3 pos, Vector3 scale, Material mat)
    {
        var o = GameObject.CreatePrimitive(type);
        o.name = name;
        o.transform.SetParent(parent);
        o.transform.position = pos;
        o.transform.localScale = scale;
        var r = o.GetComponent<Renderer>();
        if (r != null) r.material = mat;
        return o;
    }

    private static void CreateEnvironment(Transform root)
    {
        var grass = Mat(new Color(0.27f, 0.56f, 0.25f));
        var grassLight = Mat(new Color(0.42f, 0.69f, 0.31f));
        var trunk = Mat(new Color(0.30f, 0.17f, 0.09f));
        var leaves = Mat(new Color(0.12f, 0.39f, 0.18f));
        var leaves2 = Mat(new Color(0.20f, 0.50f, 0.23f));
        var mountain = Mat(new Color(0.38f, 0.48f, 0.43f));
        var rock = Mat(new Color(0.48f, 0.54f, 0.49f));

        Primitive(PrimitiveType.Plane, "Meadow_Ground", root, new Vector3(0, -0.12f, 0), new Vector3(3.8f, 1, 2.8f), grass);
        // Soft rolling hills in the distance
        Vector3[] hillPos = {
            new Vector3(-17, 1.0f, 18), new Vector3(-7, 1.3f, 21),
            new Vector3(5, 1.0f, 20), new Vector3(16, 1.5f, 18),
            new Vector3(22, 0.7f, 10), new Vector3(-22, 0.8f, 10)
        };
        for (int i = 0; i < hillPos.Length; i++)
        {
            var h = Primitive(PrimitiveType.Sphere, "Rolling_Hill_" + i, root, hillPos[i], new Vector3(15, 5 + (i%2), 9), i%2==0 ? mountain : rock);
            h.transform.rotation = Quaternion.Euler(0, i*17, 0);
        }

        // Trees around the clearing, leaving the centre open for the action
        Vector3[] treePos = {
            new Vector3(-13,0,3), new Vector3(-16,0,10), new Vector3(-9,0,13),
            new Vector3(12,0,12), new Vector3(16,0,6), new Vector3(13,0,-4),
            new Vector3(-15,0,-6), new Vector3(-10,0,-12), new Vector3(17,0,-11)
        };
        for (int i = 0; i < treePos.Length; i++)
        {
            var t = new GameObject("Tree_" + (i+1));
            t.transform.SetParent(root);
            t.transform.position = treePos[i];
            Primitive(PrimitiveType.Cylinder, "Trunk", t.transform, treePos[i] + new Vector3(0,1.25f,0), new Vector3(0.42f,2.5f,0.42f), trunk);
            Primitive(PrimitiveType.Sphere, "Canopy_Lower", t.transform, treePos[i] + new Vector3(0,3.1f,0), new Vector3(3.0f,2.5f,3.0f), leaves);
            Primitive(PrimitiveType.Sphere, "Canopy_Upper", t.transform, treePos[i] + new Vector3(0,4.25f,0), new Vector3(2.25f,2.0f,2.25f), leaves2);
        }

        // Small grass tufts as decorative clumps
        var tuftMat = Mat(new Color(0.56f, 0.75f, 0.30f));
        for (int i = 0; i < 45; i++)
        {
            float x = Mathf.Sin(i * 12.37f) * 13f;
            float z = Mathf.Cos(i * 7.91f) * 10f;
            if (Mathf.Abs(x) < 4.5f && Mathf.Abs(z) < 3.5f) continue;
            var tuft = Primitive(PrimitiveType.Sphere, "Grass_Tuft", root, new Vector3(x,0.12f,z), new Vector3(0.22f,0.42f,0.18f), tuftMat);
            tuft.transform.rotation = Quaternion.Euler(0, i*31, 18);
        }
    }

    private static void CreateTrainer(Transform root)
    {
        var skin = Mat(new Color(0.93f, 0.68f, 0.48f));
        var shirt = Mat(new Color(0.12f, 0.36f, 0.82f));
        var dark = Mat(new Color(0.10f, 0.13f, 0.20f));
        var red = Mat(new Color(0.85f, 0.12f, 0.16f));
        var white = Mat(Color.white);
        var hair = Mat(new Color(0.12f, 0.08f, 0.06f));

        var trainer = new GameObject("Trainer_Stationary");
        trainer.transform.SetParent(root);
        trainer.transform.position = new Vector3(-4.0f, 0, 0.2f);
        trainer.transform.rotation = Quaternion.Euler(0, 90, 0);

        Primitive(PrimitiveType.Capsule, "Body", trainer.transform, new Vector3(-4,1.35f,0.2f), new Vector3(0.72f,0.85f,0.42f), shirt);
        Primitive(PrimitiveType.Sphere, "Head", trainer.transform, new Vector3(-4,2.55f,0.2f), new Vector3(0.72f,0.78f,0.68f), skin);
        Primitive(PrimitiveType.Sphere, "Hair", trainer.transform, new Vector3(-4,2.87f,0.16f), new Vector3(0.78f,0.34f,0.72f), hair);
        Primitive(PrimitiveType.Cube, "Cap_Brim", trainer.transform, new Vector3(-4,2.98f,0.47f), new Vector3(0.92f,0.10f,0.42f), red);
        Primitive(PrimitiveType.Sphere, "Cap_Top", trainer.transform, new Vector3(-4,3.12f,0.15f), new Vector3(0.72f,0.28f,0.65f), red);
        // Legs
        Primitive(PrimitiveType.Capsule, "Left_Leg", trainer.transform, new Vector3(-4.22f,0.43f,0.2f), new Vector3(0.28f,0.62f,0.30f), dark);
        Primitive(PrimitiveType.Capsule, "Right_Leg", trainer.transform, new Vector3(-3.78f,0.43f,0.2f), new Vector3(0.28f,0.62f,0.30f), dark);
        Primitive(PrimitiveType.Cube, "Left_Shoe", trainer.transform, new Vector3(-4.22f,0.12f,0.38f), new Vector3(0.42f,0.20f,0.58f), dark);
        Primitive(PrimitiveType.Cube, "Right_Shoe", trainer.transform, new Vector3(-3.78f,0.12f,0.38f), new Vector3(0.42f,0.20f,0.58f), dark);

        // Stationary left arm and animated throwing arm
        Primitive(PrimitiveType.Capsule, "Left_Arm", trainer.transform, new Vector3(-4.53f,1.48f,0.2f), new Vector3(0.22f,0.57f,0.24f), shirt);
        var pivot = new GameObject("Throwing_Arm_Pivot");
        pivot.transform.SetParent(trainer.transform);
        pivot.transform.position = new Vector3(-3.62f,1.88f,0.2f);
        var arm = Primitive(PrimitiveType.Capsule, "Right_Arm", pivot.transform, new Vector3(-3.62f,1.42f,0.2f), new Vector3(0.22f,0.58f,0.24f), shirt);
        arm.transform.localPosition = new Vector3(0,-0.46f,0);
        var hand = Primitive(PrimitiveType.Sphere, "Throwing_Hand", pivot.transform, new Vector3(-3.62f,0.98f,0.2f), new Vector3(0.25f,0.25f,0.25f), skin);
        hand.transform.localPosition = new Vector3(0,-0.92f,0);
        var anim = trainer.AddComponent<TrainerAnimator>();
        anim.armPivot = pivot.transform;
        anim.throwHand = hand.transform;
        var ball = CreateBall(root, "Held_Pokeball", new Vector3(-3.62f,1.00f,0.2f), 0.34f);
        ball.transform.SetParent(hand.transform, true);
        ball.transform.localPosition = new Vector3(0, -0.05f, 0.20f);
        anim.heldBall = ball.transform;
    }

    private static GameObject CreateBall(Transform root, string name, Vector3 pos, float size)
    {
        var red = Mat(new Color(0.88f,0.08f,0.12f), 0.1f, 0.55f);
        var white = Mat(new Color(0.93f,0.95f,0.98f), 0.05f, 0.4f);
        var black = Mat(new Color(0.06f,0.07f,0.09f), 0.1f, 0.5f);
        var ball = new GameObject(name);
        ball.transform.SetParent(root);
        ball.transform.position = pos;
        Primitive(PrimitiveType.Sphere, "Ball_White_Half", ball.transform, pos, Vector3.one*size, white);
        var top = Primitive(PrimitiveType.Sphere, "Ball_Red_Half", ball.transform, pos + Vector3.up*size*0.24f, new Vector3(size,size*0.52f,size), red);
        top.transform.localPosition = new Vector3(0,size*0.24f,0);
        var band = Primitive(PrimitiveType.Cylinder, "Ball_Black_Band", ball.transform, pos, new Vector3(size*0.53f,size*0.055f,size*0.53f), black);
        band.transform.localPosition = Vector3.zero;
        var button = Primitive(PrimitiveType.Sphere, "Ball_Button", ball.transform, pos + Vector3.forward*size*0.49f, Vector3.one*size*0.22f, white);
        button.transform.localPosition = new Vector3(0,0,size*0.49f);
        foreach (var c in ball.GetComponentsInChildren<Collider>()) Object.Destroy(c);
        return ball;
    }

    private static void CreateCreature(Transform root)
    {
        var purple = Mat(new Color(0.55f,0.28f,0.78f));
        var light = Mat(new Color(0.76f,0.58f,0.91f));
        var dark = Mat(new Color(0.12f,0.08f,0.18f));
        var eye = Mat(new Color(1f,0.86f,0.18f),0,0.7f);
        var creature = new GameObject("Wild_Creature");
        creature.transform.SetParent(root);
        creature.transform.position = new Vector3(4.2f,0.95f,0.2f);
        Primitive(PrimitiveType.Sphere, "Round_Body", creature.transform, creature.transform.position, new Vector3(1.55f,1.55f,1.35f), purple);
        Primitive(PrimitiveType.Sphere, "Belly", creature.transform, new Vector3(4.2f,0.65f,0.84f), new Vector3(0.95f,0.82f,0.34f), light);
        var ear1 = Primitive(PrimitiveType.Capsule, "Left_Ear", creature.transform, new Vector3(3.65f,2.0f,0.15f), new Vector3(0.48f,1.0f,0.42f), purple);
        var ear2 = Primitive(PrimitiveType.Capsule, "Right_Ear", creature.transform, new Vector3(4.75f,2.0f,0.15f), new Vector3(0.48f,1.0f,0.42f), purple);
        ear1.transform.rotation = Quaternion.Euler(0,0,-18);
        ear2.transform.rotation = Quaternion.Euler(0,0,18);
        Primitive(PrimitiveType.Sphere, "Left_Eye", creature.transform, new Vector3(3.88f,1.22f,0.79f), new Vector3(0.20f,0.27f,0.12f), eye);
        Primitive(PrimitiveType.Sphere, "Right_Eye", creature.transform, new Vector3(4.52f,1.22f,0.79f), new Vector3(0.20f,0.27f,0.12f), eye);
        Primitive(PrimitiveType.Sphere, "Left_Pupil", creature.transform, new Vector3(3.91f,1.20f,0.89f), new Vector3(0.08f,0.13f,0.05f), dark);
        Primitive(PrimitiveType.Sphere, "Right_Pupil", creature.transform, new Vector3(4.55f,1.20f,0.89f), new Vector3(0.08f,0.13f,0.05f), dark);
        Primitive(PrimitiveType.Sphere, "Mouth", creature.transform, new Vector3(4.2f,0.91f,0.94f), new Vector3(0.22f,0.08f,0.05f), dark);
        Primitive(PrimitiveType.Sphere, "Tail", creature.transform, new Vector3(5.0f,0.7f,-0.48f), new Vector3(0.7f,0.28f,0.3f), light);
        var controller = root.GetComponent<CaptureController>();
        controller.creature = creature.transform;
    }

    private static void CreateCameraAndLight()
    {
        var camObj = new GameObject("Main Camera");
        camObj.tag = "MainCamera";
        var cam = camObj.AddComponent<Camera>();
        cam.fieldOfView = 48;
        camObj.transform.position = new Vector3(0,7.5f,-19.5f);
        camObj.transform.rotation = Quaternion.Euler(16,0,0);
        cam.clearFlags = CameraClearFlags.Skybox;
        cam.backgroundColor = new Color(0.53f,0.78f,0.96f);
        camObj.AddComponent<AudioListener>();

        var lightObj = new GameObject("Sun_Light");
        var light = lightObj.AddComponent<Light>();
        light.type = LightType.Directional;
        light.intensity = 1.15f;
        lightObj.transform.rotation = Quaternion.Euler(48,-32,0);
        RenderSettings.ambientLight = new Color(0.65f,0.70f,0.76f);
        RenderSettings.fog = true;
        RenderSettings.fogColor = new Color(0.68f,0.82f,0.91f);
        RenderSettings.fogMode = FogMode.Exponential;
        RenderSettings.fogDensity = 0.008f;
    }
}
