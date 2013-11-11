using UnityEngine;
using System.Collections;

public class RopeBuilder : MonoBehaviour
{
    #region Public properties

    public int nodeNum = 10;
    public float interval = 1.0f;
    public float mass = 1.0f;
    public float drag = 0.1f;
    public float angularDrag = 0.1f;

    #endregion

    #region Physics setup

    void AddRigidbody (GameObject node, bool isFixed)
    {
        var rb = node.AddComponent<Rigidbody> ();
        rb.mass = mass;
        rb.drag = drag;
        rb.angularDrag = angularDrag;
        rb.isKinematic = isFixed;
    }
    
    void AddJoint (GameObject node, GameObject boundTo)
    {
        var joint = node.AddComponent<ConfigurableJoint> ();
        joint.connectedBody = boundTo.rigidbody;
        
        var limit = new SoftJointLimit ();
        limit.limit = 0.1f;
        limit.spring = 40.0f;
        joint.linearLimit = limit;

        /*
        limit.limit = 10.0f;
        joint.angularYLimit = limit;
        joint.angularZLimit = limit;
        joint.highAngularXLimit = limit;
        joint.lowAngularXLimit = limit;
        */
        
        joint.xMotion = ConfigurableJointMotion.Locked;
        joint.yMotion = ConfigurableJointMotion.Locked;
        joint.zMotion = ConfigurableJointMotion.Locked;
        joint.angularXMotion = ConfigurableJointMotion.Free;
        joint.angularYMotion = ConfigurableJointMotion.Free;
        joint.angularZMotion = ConfigurableJointMotion.Free;
    }

    void BuildChain(GameObject root)
    {
        // Make the first node.
        var node = new GameObject ("first node");

        node.transform.parent = root.transform;
        node.transform.localPosition = transform.position;

        AddRigidbody (node, true);

        // Make the chain of nodes.
        for (var i = 0; i < nodeNum; i++) {
            var newNode = new GameObject ("node " + i);

            newNode.transform.parent = root.transform;
            newNode.transform.position = node.transform.position + root.transform.forward * interval;

            AddRigidbody (newNode, false);
            AddJoint (node, newNode);

            node = newNode;
        }
    }

    #endregion

    #region MonoBehaviour
    
    void Awake ()
    {
        BuildChain (gameObject);
    }

    #endregion
}
