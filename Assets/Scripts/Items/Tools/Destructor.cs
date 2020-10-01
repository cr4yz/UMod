using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destructor : ToolGunTool
{

    public override void HandleKeyDown(ToolGun toolGun, KeyCode button, Ray eyeRay)
    {
        if(Physics.Raycast(eyeRay, out RaycastHit hit)
            && hit.rigidbody
            && !hit.rigidbody.CompareTag("Player"))
        {
            var mf = hit.rigidbody.GetComponent<MeshFilter>();
            if (!mf)
            {
                return;
            }
            var projectile = GameObject.CreatePrimitive(PrimitiveType.Cube);
            projectile.GetComponent<Renderer>().material.color = Color.red;
            projectile.transform.localScale = Vector3.one * .15f;
            StartCoroutine(toolGun.ViewModel.FireProjectile(projectile, hit.transform, 35f, () =>
            {
                Explode(hit.transform);
            }));
        }
    }

    // thx: https://github.com/unitycoder/SimpleMeshExploder/blob/master/Assets/Scripts/SimpleMeshExploder.cs
    void Explode(Transform target)
    {
        var parent = new GameObject("Destructed Prop");
        parent.AddComponent<PropRoot>();

        Mesh mesh = target.GetComponent<MeshFilter>().mesh;
        Vector3[] vertices = mesh.vertices;
        Vector3[] normals = mesh.normals;
        int[] triangles = mesh.triangles;
        Vector2[] uvs = mesh.uv;
        int index = 0;

        // remove collider from original
        target.GetComponent<Collider>().enabled = false;

        // get each face
        for (int i = 0; i < triangles.Length; i += 3)
        {
            // TODO: inherit speed, spin...?
            Vector3 averageNormal = (normals[triangles[i]] + normals[triangles[i + 1]] + normals[triangles[i + 2]]).normalized;
            Vector3 s = target.GetComponent<Renderer>().bounds.size;
            float extrudeSize = ((s.x + s.y + s.z) / 3) * 0.3f;
            var piece = CreateMeshPiece(extrudeSize, target.transform.position, target.GetComponent<Renderer>().material, index, averageNormal, vertices[triangles[i]], vertices[triangles[i + 1]], vertices[triangles[i + 2]], uvs[triangles[i]], uvs[triangles[i + 1]], uvs[triangles[i + 2]]);
            index++;

            piece.transform.SetParent(parent.transform, true);
        }
        // destroy original
        Destroy(target.gameObject);
    }

    private GameObject CreateMeshPiece(float extrudeSize, Vector3 pos, Material mat, int index, Vector3 faceNormal, Vector3 v1, Vector3 v2, Vector3 v3, Vector2 uv1, Vector2 uv2, Vector2 uv3)
    {
        GameObject go = new GameObject("piece_" + index);

        Mesh mesh = go.AddComponent<MeshFilter>().mesh;
        go.AddComponent<MeshRenderer>();
        //go.tag = "Explodable"; // set this only if should be able to explode this piece also
        go.GetComponent<Renderer>().material = mat;
        go.transform.position = pos;

        Vector3[] vertices = new Vector3[3 * 4];
        int[] triangles = new int[3 * 4];
        Vector2[] uvs = new Vector2[3 * 4];

        // get centroid
        Vector3 v4 = (v1 + v2 + v3) / 3;
        // extend to backwards
        v4 = v4 + (-faceNormal) * extrudeSize;

        // not shared vertices
        // orig face
        //vertices[0] = (v1);
        vertices[0] = (v1);
        vertices[1] = (v2);
        vertices[2] = (v3);
        // right face
        vertices[3] = (v1);
        vertices[4] = (v2);
        vertices[5] = (v4);
        // left face
        vertices[6] = (v1);
        vertices[7] = (v3);
        vertices[8] = (v4);
        // bottom face
        vertices[9] = (v2);
        vertices[10] = (v3);
        vertices[11] = (v4);

        // orig face
        triangles[0] = 0;
        triangles[1] = 1;
        triangles[2] = 2;
        //  right face
        triangles[3] = 5;
        triangles[4] = 4;
        triangles[5] = 3;
        //  left face
        triangles[6] = 6;
        triangles[7] = 7;
        triangles[8] = 8;
        //  bottom face
        triangles[9] = 11;
        triangles[10] = 10;
        triangles[11] = 9;

        // orig face
        uvs[0] = uv1;
        uvs[1] = uv2;
        uvs[2] = uv3; // todo
                      // right face
        uvs[3] = uv1;
        uvs[4] = uv2;
        uvs[5] = uv3; // todo

        // left face
        uvs[6] = uv1;
        uvs[7] = uv3;
        uvs[8] = uv3;   // todo
                        // bottom face (mirror?) or custom color? or fixed from uv?
        uvs[9] = uv1;
        uvs[10] = uv2;
        uvs[11] = uv1; // todo

        mesh.vertices = vertices;
        mesh.uv = uvs;
        mesh.triangles = triangles;
        mesh.RecalculateBounds();
        mesh.RecalculateNormals();
        mesh.RecalculateTangents();

        go.AddComponent<Rigidbody>();
        MeshCollider mc = go.AddComponent<MeshCollider>();

        mc.sharedMesh = mesh;
        mc.convex = true;

        return go;
    }

}
