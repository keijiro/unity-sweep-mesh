using UnityEngine;
using System.Collections;

public class SimpleFollow : MonoBehaviour
{
    public GameObject target;

    GameObject head;

    void Awake ()
    {
        head = transform.FindChild ("head").gameObject;
    }

    void FixedUpdate ()
    {
        head.rigidbody.MovePosition (target.transform.position);
        head.rigidbody.MoveRotation (target.transform.rotation);
    }
}
