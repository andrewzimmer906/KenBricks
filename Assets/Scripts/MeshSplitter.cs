using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// Let's just convert this to a directed graph for data computation.  Seems easier
/// to me anyway :D
struct Vertex {
    public Vector3 pos;
    public Vector2 uv;
    public List<Triangle> edges;
}

struct Triangle {
    public Vertex a;
    public Vertex b;
    public Vertex c;
}

public class MeshSplitter {

    public static Mesh[] splitMesh(Mesh filter) {
        Vertex[] allVerts = ConvertToDirectedGraph(filter);
        Mesh backToFilter = ConvertFromDirectedGraph(allVerts);
        Mesh[] filters = { backToFilter };
        return filters;
    }

    private static Mesh ConvertFromDirectedGraph(Vertex[] allVerts) {
        Vector3[] verticies = new Vector3[allVerts.Length];
        Vector2[] uv = new Vector2[allVerts.Length];

        for (int i = 0; i < allVerts.Length; i++) {
            verticies[i] = allVerts[i].pos;
            uv[i] = allVerts[i].uv;
        }

        List<int> triangles = new List<int>();

        for (int i = 0; i < allVerts.Length; i++) {
            for(int j = 0; j < allVerts[i].edges.Count; j++) {
                Triangle tri = allVerts[i].edges[j];

                triangles.Add(i);
                triangles.Add(Array.FindIndex(allVerts, s => s.pos == tri.b.pos));
                triangles.Add(Array.FindIndex(allVerts, s => s.pos == tri.c.pos));
            }
        }

        Mesh mesh = new Mesh();
        mesh.SetVertices(verticies);
        mesh.SetUVs(0, uv);
        mesh.SetTriangles(triangles, 0);

        return mesh;
    }

    private static Vertex[] ConvertToDirectedGraph(Mesh mesh) {
        Vector3[] verticies = mesh.vertices;
        Vector2[] uvs = mesh.uv;
        int[] triangles = mesh.triangles;

        Vertex[] vertexGraph = new Vertex[verticies.Length];

        for (int i = 0; i < verticies.Length; i++) {
            vertexGraph[i].pos = verticies[i];
            vertexGraph[i].uv = uvs[i];
            vertexGraph[i].edges = new List<Triangle>();

        }

        for (int i = 0; i < triangles.Length-2; i+=3) {
            int first = triangles[i];
            int second = triangles[i + 1];
            int third = triangles[i + 2];

            Triangle tri = new Triangle();
            tri.a = vertexGraph[first];
            tri.b = vertexGraph[second];
            tri.c = vertexGraph[third];

            vertexGraph[first].edges.Add(tri);
        }

        return vertexGraph;
    }
}
