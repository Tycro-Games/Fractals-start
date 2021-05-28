using UnityEngine;

public class Fractal : MonoBehaviour
{
    [SerializeField, Range(1, 8)]
    private int depth = 4;

    [SerializeField]
    private float offset = 1.0f;

    private void Start()
    {
        name = "Celula " + depth;
        if (depth <= 1)
        {
            return;
        }
        Fractal childA = CreateChild(Vector3.up, Quaternion.identity);
        Fractal childB = CreateChild(Vector3.right, Quaternion.Euler(0f, 0f, -90f));
        Fractal childC = CreateChild(Vector3.left, Quaternion.Euler(0f, 0f, 90f));
        Fractal childD = CreateChild(Vector3.forward, Quaternion.Euler(90f, 0f, 0f));
        Fractal childE = CreateChild(Vector3.back, Quaternion.Euler(-90f, 0f, 0f));

        childA.transform.SetParent(transform, false);
        childB.transform.SetParent(transform, false);
        childC.transform.SetParent(transform, false);
        childD.transform.SetParent(transform, false);
        childE.transform.SetParent(transform, false);
    }

    private void Update()
    {
        //   transform.Rotate(0f, 22.5f * Time.deltaTime, 0f);
    }

    private Fractal CreateChild(Vector3 direction, Quaternion rotation)//constructor
    {
        Fractal child = Instantiate(this); //make this
        child.depth = depth - 1; //substract so it's not infinite
        child.transform.localPosition = direction * offset;
        child.transform.localRotation = rotation;
        child.transform.localScale = 0.5f * Vector3.one; //yep it is constant(cause in hierarchy a parent dictates the world scale
        return child;
    }
}