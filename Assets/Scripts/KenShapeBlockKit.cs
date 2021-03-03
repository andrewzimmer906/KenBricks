using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KenShapeBlockKit : MonoBehaviour
{
    public void GenerateModel() {
        MeshWithCenter[] meshes = MeshSplitter.splitMesh(GetComponent<MeshFilter>().sharedMesh);

        GameObject kit = new GameObject(gameObject.name + "_kit", typeof(MeshRenderer), typeof(MeshFilter));

        MeshRenderer renderer = GetComponent<MeshRenderer>();
        int i = 1;
        foreach (MeshWithCenter mesh in meshes) {
            CreateChildObject(mesh, GetComponent<MeshRenderer>(), kit, i++);
        }
    }

    private void CreateChildObject(MeshWithCenter mesh, MeshRenderer renderer, GameObject parent, int num) {
        GameObject child = new GameObject(gameObject.name + "_brick" + num, typeof(MeshRenderer), typeof(MeshFilter));

        MeshFilter childFilter = child.GetComponent<MeshFilter>();
        childFilter.sharedMesh = mesh.mesh;

        MeshRenderer childRenderer = child.GetComponent<MeshRenderer>();
        childRenderer.sharedMaterials = renderer.sharedMaterials;
        child.transform.SetParent(parent.transform, false);
        child.transform.position = mesh.center;
    }
}
