using UnityEngine;
using System.Collections;

public class MeshExtruder : MonoBehaviour
{
    #region Public properties

    public int division = 800;
    public float baseScale = 1.0f;

    #endregion


    #region Private objects

    Mesh mesh;

    #endregion

    #region Base shape generator

    Vector3[] CreateSquareBase (float scale)
    {
        var array = new Vector3[4];
        array [0] = new Vector3 (-1, -1, 0) * scale;
        array [1] = new Vector3 (+1, -1, 0) * scale;
        array [2] = new Vector3 (+1, +1, 0) * scale;
        array [3] = new Vector3 (-1, +1, 0) * scale;
        return array;
    }

    #endregion

    #region Curve function

    AnimationCurve[] CreateCurveXYZ ()
    {
        var curves = new AnimationCurve[] {
            new AnimationCurve (),
            new AnimationCurve (),
            new AnimationCurve (),
            new AnimationCurve (),
            new AnimationCurve (),
            new AnimationCurve ()
        };

        var time = 0.0f;
        var delta = 1.0f / (transform.childCount - 1);

        foreach (Transform point in transform)
        {
            var position = point.localPosition;
            var up = point.localRotation * Vector3.up;
            curves [0].AddKey (time, position.x);
            curves [1].AddKey (time, position.y);
            curves [2].AddKey (time, position.z);
            curves [3].AddKey (time, up.x);
            curves [4].AddKey (time, up.y);
            curves [5].AddKey (time, up.z);
            time += delta;
        }

        return curves;
    }

    #endregion

    #region MonoBehaviour

    void Start ()
    {
        mesh = new Mesh ();
        mesh.MarkDynamic ();
        GetComponent<MeshFilter> ().sharedMesh = mesh;
    }
    
    void Update ()
    {
        var shape = CreateSquareBase (baseScale);
        var curves = CreateCurveXYZ ();

        var vertices = new Vector3[shape.Length * (division * 2 + 2)];
        var offset = 0;

        for (var i = 0; i < division; i++)
        {
            var t1 = (float)(i + 0) / division;
            var t2 = (float)(i + 1) / division;

            var p1 = new Vector3 (curves [0].Evaluate (t1), curves [1].Evaluate (t1), curves [2].Evaluate (t1));
            var p2 = new Vector3 (curves [0].Evaluate (t2), curves [1].Evaluate (t2), curves [2].Evaluate (t2));

            var ny = new Vector3 (curves [3].Evaluate (t1), curves [4].Evaluate (t1), curves [5].Evaluate (t1)).normalized;
            var nz = (p2 - p1).normalized;
            var nx = Vector3.Cross (ny, nz);

            foreach (var v in shape)
            {
                var p = p1 + nx * v.x + ny * v.y;
                vertices [offset++] = p;
                vertices [offset++] = p;
            }
        }

        // Head cap.
        for (var i = 0; i < shape.Length; i++)
        {
            vertices[offset++] = vertices[i * 2];
        }

        // Tail cap.
        {
            var bi = offset - shape.Length * 3;
            for (var i = 0; i < shape.Length; i++)
            {
                vertices[offset++] = vertices[bi + i * 2];
            }
        }

        var indices = new int[(division - 1) * shape.Length * 6 + (shape.Length - 2) * 6];
        offset = 0;

        for (var i = 0; i < division - 1; i++)
        {
            for (var i2 = 0; i2 < shape.Length; i2++)
            {
                var bi1 = 2 * (shape.Length * i + i2) + 1;
                var bi2 = 2 * (shape.Length * i + (i2 + 1) % shape.Length);

                indices [offset++] = bi1;
                indices [offset++] = bi2;
                indices [offset++] = bi2 + shape.Length * 2;

                indices [offset++] = bi1;
                indices [offset++] = bi2 + shape.Length * 2;
                indices [offset++] = bi1 + shape.Length * 2;
            }
        }

        // Make caps with a trignale fan.
        {
            var bi1 = vertices.Length - shape.Length * 2;
            var bi2 = vertices.Length - shape.Length;

            for (var i = 1; i < shape.Length - 1; i++)
            {
                indices [offset++] = bi1;
                indices [offset++] = bi1 + i + 1;
                indices [offset++] = bi1 + i;

                indices [offset++] = bi2;
                indices [offset++] = bi2 + i;
                indices [offset++] = bi2 + i + 1;
            }
        }

        mesh.vertices = vertices;
        mesh.SetIndices (indices, MeshTopology.Triangles, 0);
        mesh.RecalculateNormals ();
        mesh.RecalculateBounds ();
    }

    #endregion
}
