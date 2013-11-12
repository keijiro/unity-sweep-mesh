using UnityEngine;
using System.Collections;

public class SimpleMove : MonoBehaviour
{
    Vector3[] seeds;
    Vector3 initialPosition;
    public float range;
    public float speed;

    void Start ()
    {
        seeds = new Vector3[] {
            Random.insideUnitSphere,
            Random.insideUnitSphere,
            Random.insideUnitSphere
        };
        initialPosition = transform.position;
    }

    void FixedUpdate ()
    {
        var position = new Vector3 (
            Perlin.Fbm (seeds[0] + Vector3.right * Time.time * speed, 3),
            0,
            Perlin.Fbm (seeds[2] + Vector3.forward * Time.time * speed, 3)
        );

        rigidbody.MovePosition (initialPosition + position * range);

        rigidbody.MoveRotation (Quaternion.AngleAxis(Mathf.Sin (Time.time) * 180.0f, Vector3.forward));
    }
}
