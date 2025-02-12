using System.Collections.Generic;
using System.Threading.Tasks;

using UnityEngine;

namespace Reflectis.CreatorKit.Worlds.Core.Interaction
{
    [RequireComponent(typeof(MeshRenderer))]
    public class InteractableSubmesh : MonoBehaviour
    {
        public const int MAX_TRIANGLES_PER_SUBMESH = 20000;

        private bool isSettingUpColliders = false;

        [SerializeField]
        private List<Collider> colliders;
        [SerializeField]
        private MeshRenderer meshRenderer;
        public List<Collider> Colliders { get => colliders; set => colliders = value; }
        public MeshRenderer MeshRenderer { get => meshRenderer; set => meshRenderer = value; }

        public async Task SetupSubmeshColliders()
        {
            if (!isSettingUpColliders) // we have not setup the colliders yet
            {
                isSettingUpColliders = true;
                colliders = new List<Collider>();
                var existingColliders = meshRenderer.GetComponents<Collider>();
                if (existingColliders == null || existingColliders.Length == 0)
                {
                    var colliders = await SplitMeshAndCreateColliders(meshRenderer);
                    Colliders.AddRange(colliders);
                }
                else
                {
                    Colliders.AddRange(existingColliders);
                }
                isSettingUpColliders = false;
            }
            else
            {
                while (isSettingUpColliders)
                {
                    await Task.Yield();
                }
            }
        }


        // Function that splits the mesh and generates the colliders for each submesh
        private async Task<List<Collider>> SplitMeshAndCreateColliders(MeshRenderer meshRenderer)
        {
            Mesh originalMesh = meshRenderer.GetComponent<MeshFilter>().sharedMesh;
            int[] triangles = originalMesh.triangles;
            Vector3[] vertices = originalMesh.vertices;
            Vector3[] normals = originalMesh.normals;
            Vector2[] uvs = originalMesh.uv;

            int triangleCount = triangles.Length / 3; // Total number of triangles in the mesh

            // List to hold all submeshes
            var subMeshes = new List<Mesh>();

            // Temporary variables to construct the submeshes
            List<int> submeshTriangles = new List<int>();
            List<Vector3> submeshVertices = new List<Vector3>();
            List<Vector3> submeshNormals = new List<Vector3>();
            List<Vector2> submeshUVs = new List<Vector2>();

            int currentTriangleIndex = 0;

            List<Collider> colliders = new List<Collider>();

            while (currentTriangleIndex < triangleCount)
            {
                submeshTriangles.Clear();
                submeshVertices.Clear();
                submeshNormals.Clear();
                submeshUVs.Clear();

                // Construct the submesh until the triangle threshold is reached
                int trianglesAdded = 0;
                while (trianglesAdded < MAX_TRIANGLES_PER_SUBMESH && currentTriangleIndex < triangleCount)
                {
                    for (int j = 0; j < 3; j++) // Adds the 3 indices of the current triangle
                    {
                        int triangleVertexIndex = triangles[currentTriangleIndex * 3 + j];
                        submeshTriangles.Add(submeshVertices.Count); // New index for the submesh
                        submeshVertices.Add(vertices[triangleVertexIndex]);
                        submeshNormals.Add(normals[triangleVertexIndex]);
                        // Check if the UV array exists and has enough entries
                        if (uvs.Length > 0 && triangleVertexIndex < uvs.Length)
                        {
                            submeshUVs.Add(uvs[triangleVertexIndex]);
                        }
                        else
                        {
                            // Add a default UV coordinate if UVs are not present or are out of range
                            submeshUVs.Add(Vector2.zero);
                        }
                    }

                    trianglesAdded++;
                    currentTriangleIndex++;
                }

                // Create the mesh for the submesh
                Mesh submesh = new Mesh();
                submesh.SetVertices(submeshVertices);
                submesh.SetTriangles(submeshTriangles, 0);
                submesh.SetNormals(submeshNormals);
                submesh.SetUVs(0, submeshUVs);

                // Add the submesh to the list
                subMeshes.Add(submesh);

                // Create the collider for the submesh (back to the main thread to interact with Unity)
                MeshCollider submeshCollider = meshRenderer.gameObject.AddComponent<MeshCollider>();
                submeshCollider.convex = false;  // If required, it can be set to true to simplify collisions
                submeshCollider.sharedMesh = submesh;
                colliders.Add(submeshCollider);
                await Task.Yield();

            }

            //Debug.Log($"Mesh divided into {subMeshes.Count} submeshes.");

            return colliders;
        }
    }
}
