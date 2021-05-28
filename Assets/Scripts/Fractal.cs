using UnityEngine;

public class Fractal : MonoBehaviour
{
    [SerializeField, Range(1, 8)]
    private int depth = 4;

    [SerializeField]
    private float offset = 1.0f;

    [SerializeField]
    private Mesh[] mesh;

    [SerializeField]
    private Material material;

    private MaterialPropertyBlock _propBlock;

    [ColorUsage(true, true)]
    public Color Color1, Color2;

    public float Speed = 1, Offset;

    private FractalPart[][] parts;
    public float rotspeed;

    private struct FractalPart
    {
        public Vector3 direction;
        public Quaternion rotation;
        public Transform transform;
    }

    private static Vector3[] directions = {
        Vector3.up, Vector3.right, Vector3.left, Vector3.forward, Vector3.back
    };

    private static Quaternion[] rotations = {
        Quaternion.identity,
        Quaternion.Euler(0f, 0f, -90f), Quaternion.Euler(0f, 0f, 90f),
        Quaternion.Euler(90f, 0f, 0f), Quaternion.Euler(-90f, 0f, 0f)
    };

    private void Awake()
    {
        parts = new FractalPart[depth][];

        for (int i = 0, length = 1; i < parts.Length; i++, length *= 5)
        {
            parts[i] = new FractalPart[length];//init all the sub arrays
        }
        float scale = 1f;//base scale
        parts[0][0] = CreatePart(0, 0, scale);//first one
        for (int l1 = 1; l1 < parts.Length; l1++)//generates in all 5 directions
        {
            scale *= 0.5f;
            FractalPart[] levelParts = parts[l1];
            for (int j = 0; j < levelParts.Length; j += 5)
            {
                for (int c = 0; c < 5; c++)
                {
                    levelParts[j + c] = CreatePart(l1, c, scale);
                }
            }
        }
    }

    private void Update()
    {
        Quaternion deltaRotation = Quaternion.Euler(0f, rotspeed * Time.deltaTime, 0f);
        FractalPart rootPart = parts[0][0];
        rootPart.rotation *= deltaRotation;
        rootPart.transform.localRotation = rootPart.rotation;
        parts[0][0] = rootPart;

        for (int l1 = 1; l1 < parts.Length; l1++)
        {
            FractalPart[] parentParts = parts[l1 - 1];
            FractalPart[] levelParts = parts[l1];
            for (int j = 0; j < levelParts.Length; j++)
            {
                Transform parentTransform = parentParts[j / 5].transform;
                FractalPart part = levelParts[j];
                part.rotation *= deltaRotation;
                //pos rot
                part.transform.localRotation = parentTransform.localRotation * part.rotation;//so it is local
                part.transform.localPosition =
                    parentTransform.localPosition +
                    parentTransform.localRotation *
                       (offset * part.transform.localScale.x * part.direction);
                levelParts[j] = part;
            }
        }
    }

    private FractalPart CreatePart(int levelIndex, int childIndex, float scale)
    {
        var go = new GameObject("Celula" + levelIndex + " C" + childIndex);
        go.transform.localScale = scale * Vector3.one;
        go.transform.SetParent(transform, false);

        go.AddComponent<MeshFilter>().mesh = mesh[Random.Range(0, mesh.Length)];

        MeshRenderer meshRenderer = go.AddComponent<MeshRenderer>();
        meshRenderer.material = material;
        //change color
        _propBlock = new MaterialPropertyBlock();
        meshRenderer.GetPropertyBlock(_propBlock);
        Speed += .5f;
        _propBlock.SetColor("_BaseColor", Color.Lerp(Color1, Color2, (Mathf.Sin(Speed + Offset) + 1) / 2f));
        meshRenderer.SetPropertyBlock(_propBlock);
        return new FractalPart
        {
            direction = directions[childIndex],
            rotation = rotations[childIndex],
            transform = go.transform
        };
    }
}