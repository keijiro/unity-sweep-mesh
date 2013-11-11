using UnityEngine;
using System.Collections;

public class MeshExtruder : MonoBehaviour
{
    public int division = 800;
    public float baseScale = 1.0f;
    Mesh mesh;

    void Start ()
    {
        mesh = new Mesh ();
        mesh.MarkDynamic ();
        GetComponent<MeshFilter> ().sharedMesh = mesh;
    }
    
    void Update ()
    {
        var baseShape = new Vector3[4];
        
        baseShape [0] = new Vector3 (-1, -1, 0) * baseScale;
        baseShape [1] = new Vector3 (+1, -1, 0) * baseScale;
        baseShape [2] = new Vector3 (+1, +1, 0) * baseScale;
        baseShape [3] = new Vector3 (-1, +1, 0) * baseScale;

        var cvx = new AnimationCurve ();
        var cvy = new AnimationCurve ();
        var cvz = new AnimationCurve ();
        
        var pointCount = transform.childCount;
        var time = 0.0f;
        foreach (Transform point in transform) {
            cvx.AddKey (time, point.position.x);
            cvy.AddKey (time, point.position.y);
            cvz.AddKey (time, point.position.z);
            time += 1.0f / (pointCount - 1);
        }
        
        var vertices = new Vector3[2 * baseShape.Length * division];
        var up = Vector3.forward;
        var offs = 0;
        var prev = new Vector3 (cvx.Evaluate (-0.1f), cvy.Evaluate (-0.1f), cvz.Evaluate (-0.1f));
        var prevf = (new Vector3 (cvx.Evaluate (0), cvy.Evaluate (0), cvz.Evaluate (0)) - prev).normalized;
        
        for (var i = 0; i < division; i++) {
            time = (float)i / (division - 1);
            var p = new Vector3 (cvx.Evaluate (time), cvy.Evaluate (time), cvz.Evaluate (time));
            
            var f = (p - prev).normalized;
            
            up = Quaternion.FromToRotation (prevf, f) * up;
            
            var r = Vector3.Cross (up, f).normalized;
            
            foreach (var v in baseShape) {
                var temp = p + r * v.x + up * v.y;
                vertices [offs++] = temp;
                vertices [offs++] = temp;
            }
            
            prev = p;
            prevf = f;
        }

        var indices = new int[baseShape.Length * (division - 1) * 24];
        offs = 0;
        for (var i = 0; i < division - 1; i++) {
            for (var j = 0; j < baseShape.Length; j++) {
                var bi1 = baseShape.Length * 2 * i + 2 * j + 1;
                var bi2 = baseShape.Length * 2 * i + 2 * ((j + 1) % baseShape.Length);

                indices[offs++] = bi1;
                indices[offs++] = bi2;
                indices[offs++] = bi2 + baseShape.Length * 2;

                indices[offs++] = bi1;
                indices[offs++] = bi2 + baseShape.Length * 2;
                indices[offs++] = bi1 + baseShape.Length * 2;
            }
        }

        mesh.vertices = vertices;
        mesh.SetIndices (indices, MeshTopology.Triangles, 0);
        mesh.RecalculateNormals ();
    }
}
