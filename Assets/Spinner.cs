using UnityEngine;
using System.Collections;

public class Spinner : MonoBehaviour
{
    float rx;
    float ry;

    void Awake()
    {
        rx = Random.Range (10.0f, 100.0f);
        ry = Random.Range (10.0f, 100.0f);
    }

    void Update ()
    {
        transform.localRotation =
            Quaternion.AngleAxis (Time.time * rx, Vector3.right) *
            Quaternion.AngleAxis (Time.time * ry, Vector3.up);
    }
}
