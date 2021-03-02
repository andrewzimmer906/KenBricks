using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KenShapeBlockKit : MonoBehaviour
{
    public void GenerateModel() {
        Mesh[] meshes = MeshSplitter.splitMesh(GetComponent<MeshFilter>().sharedMesh);
        CreateChildObject(GetComponent<MeshFilter>().sharedMesh, GetComponent<MeshRenderer>());

        /*
        foreach (MeshFilter filter in filters) {

        }
        */
    }

    private void CreateChildObject(Mesh mesh, MeshRenderer renderer) {
        GameObject child = new GameObject(gameObject.name + "_sub", typeof(MeshRenderer), typeof(MeshFilter));

        MeshFilter childFilter = child.GetComponent<MeshFilter>();
        childFilter.sharedMesh = mesh;

        MeshRenderer childRenderer = child.GetComponent<MeshRenderer>();
        childRenderer.sharedMaterials = renderer.sharedMaterials;

        child.transform.SetParent(gameObject.transform, false);
    }
}
