using UnityEngine;
using System.Collections;

public class SimpleMove : MonoBehaviour
{
    void Start ()
    {
    
    }
    
    void FixedUpdate ()
    {
        var position = new Vector3 (
            Perlin.Fbm (Vector3.right * Time.time, 3),
            Perlin.Fbm (Vector3.up * Time.time, 3),
            Perlin.Fbm (Vector3.forward * Time.time, 3)
        );

        rigidbody.MovePosition (position * 4.0f);

        rigidbody.MoveRotation (Quaternion.AngleAxis(Mathf.Sin (Time.time) * 180.0f, Vector3.forward));
    }
}
