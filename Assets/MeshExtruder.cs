using UnityEngine;
using System.Collections;

public class MeshExtruder : MonoBehaviour
{
    public int division = 800;
    Mesh mesh;

    void Start ()
    {
        mesh = new Mesh();
        mesh.MarkDynamic ();

        GetComponent<MeshFilter> ().sharedMesh = mesh;
    }
    
    void Update ()
    {
        var baseShape = new Vector3[4];
        
        baseShape [0] = new Vector3 (-1, -1, 0) * 0.3f;
        baseShape [1] = new Vector3 (+1, -1, 0) * 0.3f;
        baseShape [2] = new Vector3 (+1, +1, 0) * 0.3f;
        baseShape [3] = new Vector3 (-1, +1, 0) * 0.3f;

        var cvx = new AnimationCurve ();
        var cvy = new AnimationCurve ();
        var cvz = new AnimationCurve ();
        
        var pointCount = transform.childCount;
        var time = 0.0f;
        foreach (Transform point in transform) {
            cvx.AddKey(time, point.position.x);
            cvy.AddKey(time, point.position.y);
            cvz.AddKey(time, point.position.z);
            time += 1.0f / (pointCount - 1);
        }
        
        var vertices = new Vector3[baseShape.Length * division];
        var up = Vector3.up;
        var offs = 0;
        var prev = new Vector3 (cvx.Evaluate (-0.1f), cvy.Evaluate (-0.1f), cvz.Evaluate (-0.1f));
        var prevf = (new Vector3 (cvx.Evaluate (0), cvy.Evaluate (0), cvz.Evaluate (0)) - prev).normalized;
        
        for (var i = 0; i < division; i++) {
            time = (float)i / (division - 1);
            var p = new Vector3 (cvx.Evaluate(time), cvy.Evaluate(time), cvz.Evaluate(time));
            
            var f = (p - prev).normalized;
            
            up = Quaternion.FromToRotation(prevf, f) * up;
            
            var r = Vector3.Cross(up, f).normalized;
            
            foreach (var v in baseShape) {
                vertices[offs++] = p + r * v.x + up * v.y;
            }
            
            prev = p;
            prevf = f;
        }
        
        var lines = new int[baseShape.Length * division];
        for (var i = 0; i < lines.Length; i++) {
            lines[i] = i;
        }
        mesh.vertices = vertices;
        mesh.SetIndices (lines, MeshTopology.LineStrip, 0);
    }
}
