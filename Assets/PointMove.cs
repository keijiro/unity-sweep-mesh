using UnityEngine;
using System.Collections;

public class PointMove : MonoBehaviour {
    float seed;

    void Start () {
        seed = Random.value * 35.7f;
        transform.position = Random.insideUnitSphere * 3.0f;
    }

	void Update () {
        transform.position += transform.up * Time.deltaTime;

        transform.localRotation =
            Quaternion.AngleAxis (Perlin.Noise(transform.position) * Time.deltaTime * 600.0f, transform.right) *
            Quaternion.AngleAxis (Perlin.Noise(transform.position + Vector3.right) * Time.deltaTime * 600.0f, transform.forward) *
            transform.localRotation;
	}
}
