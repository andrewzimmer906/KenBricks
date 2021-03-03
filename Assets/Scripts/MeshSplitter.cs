using System;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

/// Let's just convert this to a directed graph for data computation.  Seems easier
/// to me anyway :D

class MeshGraph {
    public Vertex[] allVerts;
    public List<Triangle> edges;

    public Triangle[] edgesForVertex(Vertex v) {
        List<Triangle> activeEdges = new List<Triangle>();
        foreach(Triangle edge in edges) {
            if (edge.a.isEqual(v)) {
                activeEdges.Add(edge);
            }
        }

        return activeEdges.ToArray();
    }

    public Triangle[] edgesConnectedToVertex(Vertex v) {
        List<Triangle> activeEdges = new List<Triangle>();
        foreach (Triangle edge in edges) {
            if (edge.a.isEqual(v) ||
                edge.b.isEqual(v) ||
                edge.c.isEqual(v)) {
                activeEdges.Add(edge);
            }
        }

        return activeEdges.ToArray();
    }

    public bool isEmpty {
        get {
            return edges.Count == 0;
        }
    }

    public bool sharesVertex(MeshGraph other) {
        foreach(Vertex v in allVerts) {
            foreach(Vertex v2 in other.allVerts) {
                if (v.pos == v2.pos) {
                    return true;
                }
            }
        }

        return false;
    }

    public void AddGraph(MeshGraph other) {
        edges.AddRange(other.edges);
        List<Vertex> verts = new List<Vertex>(allVerts);
        verts.AddRange(other.allVerts);
        allVerts = verts.ToArray();
    }

    public Vertex[] verticesForEdges(Triangle[] edges) {
        List<Vertex> verts = new List<Vertex>();
        foreach(Triangle edge in edges) {
            foreach(Vertex vert in allVerts) {
                if (vert.pos == edge.a.pos ||
                   vert.pos == edge.b.pos ||
                   vert.pos == edge.c.pos) {
                    verts.Add(vert);
                }
            }
        }

        List<Vertex> vertsDeduped = new List<Vertex>();
        foreach (Vertex vert in verts) {
            if (vertsDeduped.Find(v => v.isEqual(vert)) == null) {
                vertsDeduped.Add(vert);
            }
        }

        return vertsDeduped.ToArray();
    }

    public void RemoveDuplicateVertices() {
        List<Vertex> verts = new List<Vertex>();
        foreach(Vertex vert in allVerts) {
            if(verts.Find(v => v.isEqual(vert)) == null) {
                verts.Add(vert);
            }
        }

        allVerts = verts.ToArray();
    }
}

class Vertex {
    public Vector3 pos;
    public Vector3 normal;
    public Vector4 tangent;
    public Vector2 uv;

    public bool isWalked;

    public bool isEqual(Vertex other) {
        return pos == other.pos &&
               normal == other.normal &&
               tangent == other.tangent &&
               uv == other.uv;
    }
}

class Triangle {
    public Vertex a;
    public Vertex b;
    public Vertex c;
}

public struct MeshWithCenter {
    public Vector3 center;
    public Mesh mesh;
}

public class MeshSplitter {
    public static MeshWithCenter[] splitMesh(Mesh mesh) {
        MeshGraph graph = ConvertToDirectedGraph(mesh);
        MeshGraph[] graphs = SplitGraph(graph);

        MeshWithCenter[] meshes = new MeshWithCenter[graphs.Length];
        for (int i = 0; i < graphs.Length; i++) {
            meshes[i] = ConvertFromDirectedGraph(graphs[i]);
        }

        return meshes;
    }

    private static MeshWithCenter ConvertFromDirectedGraph(MeshGraph graph) {
        Vector3[] verticies = new Vector3[graph.allVerts.Length];
        Vector3[] normals = new Vector3[graph.allVerts.Length];
        Vector4[] tangents = new Vector4[graph.allVerts.Length];
        Vector2[] uv = new Vector2[graph.allVerts.Length];

        Bounds b = new Bounds(graph.allVerts[0].pos, Vector3.zero);

        for (int i = 0; i < graph.allVerts.Length; i++) {
            normals[i] = graph.allVerts[i].normal;
            tangents[i] = graph.allVerts[i].tangent;
            uv[i] = graph.allVerts[i].uv;

            b.Encapsulate(graph.allVerts[i].pos);
        }

        // put the verticie position in as an offset pos so the brick'll
        // be centered yo.
        for (int i = 0; i < graph.allVerts.Length; i++) {
            verticies[i] = graph.allVerts[i].pos - b.center;
        }


        List<int> triangles = new List<int>();

        for (int i = 0; i < graph.edges.Count; i++) {
            Triangle tri = graph.edges[i];
            
            triangles.Add(Array.FindIndex(graph.allVerts, s => s.isEqual(tri.a)));
            triangles.Add(Array.FindIndex(graph.allVerts, s => s.isEqual(tri.b)));
            triangles.Add(Array.FindIndex(graph.allVerts, s => s.isEqual(tri.c)));
        }

        Mesh mesh = new Mesh();
        mesh.SetVertices(verticies);
        mesh.SetUVs(0, uv);
        mesh.SetNormals(normals);
        mesh.SetTangents(tangents);
        mesh.SetTriangles(triangles, 0);
        mesh.RecalculateBounds();

        MeshWithCenter data = new MeshWithCenter();
        data.center = b.center;
        data.mesh = mesh;
        return data;
    }

    private static MeshGraph ConvertToDirectedGraph(Mesh mesh) {
        Vector3[] verticies = mesh.vertices;
        Vector3[] normals = mesh.normals;
        Vector4[] tangents = mesh.tangents;
        Vector2[] uvs = mesh.uv;
        int[] triangles = mesh.triangles;

        MeshGraph graph = new MeshGraph();

        graph.allVerts = new Vertex[verticies.Length];
        graph.edges = new List<Triangle>();

        for (int i = 0; i < verticies.Length; i++) {
            graph.allVerts[i] = new Vertex();

            graph.allVerts[i].pos = verticies[i];
            graph.allVerts[i].normal = normals[i];
            graph.allVerts[i].uv = uvs[i];
            graph.allVerts[i].tangent = tangents[i];
        }

        for (int i = 0; i < triangles.Length-2; i+=3) {
            int first = triangles[i];
            int second = triangles[i + 1];
            int third = triangles[i + 2];

            Triangle tri = new Triangle();
            tri.a = graph.allVerts[first];
            tri.b = graph.allVerts[second];
            tri.c = graph.allVerts[third];

            graph.edges.Add(tri);
        }

        return graph;
    }

    /// Possibly the most inefficient tree walk known to humankind!! 
    /// Recursive method to split the graph into pieces based on if the verticies are connected by the 
    /// triangle array (edges after conversion).
    static MeshGraph[] SplitGraph(MeshGraph unprocessedGraph) {
        if (unprocessedGraph.allVerts.Length == 0) {
            return new MeshGraph[0];
        }

        List<MeshGraph> subGraphs = new List<MeshGraph>();

        for(int i = 0; i < unprocessedGraph.allVerts.Length; i++) {
            Vertex v = unprocessedGraph.allVerts[i];
            MeshGraph g = GraphFromVertex(unprocessedGraph, v);

            if (!g.isEmpty) {
                subGraphs.Add(g);
            }
        }

        return subGraphs.ToArray();
    }

    static MeshGraph GraphFromVertex(MeshGraph graph, Vertex v) {
        if (v.isWalked) {
            MeshGraph emptyGraph = new MeshGraph();
            emptyGraph.allVerts = new Vertex[0];
            //emptyGraph.allVerts[0] = v;
            emptyGraph.edges = new List<Triangle>();
            return emptyGraph;
        }

        List<Vertex> verticies = new List<Vertex>();
        List<Triangle> edges = new List<Triangle>();

        Triangle[] activeEdges = graph.edgesForVertex(v);
        Triangle[] connectedEdges = graph.edgesConnectedToVertex(v);

        verticies.Add(v);
        edges.AddRange(activeEdges);

        v.isWalked = true;
            foreach(Vertex subV in graph.verticesForEdges(connectedEdges)) {
            MeshGraph subGraph = GraphFromVertex(graph, subV);
            verticies.AddRange(subGraph.allVerts);
            edges.AddRange(subGraph.edges);
        }

        MeshGraph filledGraph = new MeshGraph();
        filledGraph.allVerts = verticies.ToArray();
        filledGraph.edges = edges;

        return filledGraph;
    }
}
