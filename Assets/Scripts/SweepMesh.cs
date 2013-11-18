using UnityEngine;
using System.Collections;

public class SweepMesh : MonoBehaviour
{
    #region Public properties

    public int division = 100;
    public int points = 4;
    public float thickness = 1.0f;
    public float noiseLevel = 0.0f;
    public float noiseFreq = 0.1f;
    public int noiseFractal = 0;

    #endregion


    #region Private objects

    Mesh mesh;

    #endregion

    #region Profile generator

    Vector3[] CreateRegularPolygonProfile (int points)
    {
        var profile = new Vector3[points];
        for (int i = 0; i < points; i++)
        {
            var theta = Mathf.PI * 2 / points * i;
            profile [i] = new Vector3 (Mathf.Cos (theta), Mathf.Sin (theta), 0);
        }
        return profile;
    }

    #endregion

    #region Path generator

    AnimationCurve[] CreatePathFromChildren ()
    {
        var path = new AnimationCurve[] {
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
            path [0].AddKey (time, position.x);
            path [1].AddKey (time, position.y);
            path [2].AddKey (time, position.z);
            path [3].AddKey (time, up.x);
            path [4].AddKey (time, up.y);
            path [5].AddKey (time, up.z);
            time += delta;
        }

        return path;
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
        var profile = CreateRegularPolygonProfile (points);
        var path = CreatePathFromChildren ();

        var vertices = new Vector3[profile.Length * (division * 2 + 2)];
        var offset = 0;

        for (var i = 0; i < division; i++)
        {
            var t1 = (float)(i + 0) / division;
            var t2 = (float)(i + 1) / division;

            var p1 = new Vector3 (path [0].Evaluate (t1), path [1].Evaluate (t1), path [2].Evaluate (t1));
            var p2 = new Vector3 (path [0].Evaluate (t2), path [1].Evaluate (t2), path [2].Evaluate (t2));

            var ny = new Vector3 (path [3].Evaluate (t1), path [4].Evaluate (t1), path [5].Evaluate (t1)).normalized;
            var nz = (p2 - p1).normalized;
            var nx = Vector3.Cross (ny, nz);

            foreach (var v in profile)
            {
                var t = thickness + Perlin.Fbm (p1 * noiseFreq, noiseFractal) * noiseLevel;
                var p = p1 + nx * (v.x * t) + ny * (v.y * t);
                vertices [offset++] = p;
                vertices [offset++] = p;
            }
        }

        // Head cap.
        for (var i = 0; i < profile.Length; i++)
        {
            vertices [offset++] = vertices [i * 2];
        }

        // Tail cap.
        {
            var bi = offset - profile.Length * 3;
            for (var i = 0; i < profile.Length; i++)
            {
                vertices [offset++] = vertices [bi + i * 2];
            }
        }

        var indices = new int[(division - 1) * profile.Length * 6 + (profile.Length - 2) * 6];
        offset = 0;

        for (var i = 0; i < division - 1; i++)
        {
            for (var i2 = 0; i2 < profile.Length; i2++)
            {
                var bi1 = 2 * (profile.Length * i + i2) + 1;
                var bi2 = 2 * (profile.Length * i + (i2 + 1) % profile.Length);

                indices [offset++] = bi1;
                indices [offset++] = bi2;
                indices [offset++] = bi2 + profile.Length * 2;

                indices [offset++] = bi1;
                indices [offset++] = bi2 + profile.Length * 2;
                indices [offset++] = bi1 + profile.Length * 2;
            }
        }

        // Make caps with a trignale fan.
        {
            var bi1 = vertices.Length - profile.Length * 2;
            var bi2 = vertices.Length - profile.Length;

            for (var i = 1; i < profile.Length - 1; i++)
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
