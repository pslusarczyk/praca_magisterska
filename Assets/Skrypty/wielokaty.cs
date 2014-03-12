using UnityEngine;

namespace Assets.Skrypty
{
    public class Wielokaty : MonoBehaviour
    {
        private bool czy;

        // Use this for initialization
        private void Start()
        {
        }

        // Update is called once per frame
        private void Update()
        {
            if (czy) return;
            var meshFilter = GetComponent<MeshFilter>();
            if (meshFilter == null)
            {
                Debug.LogError("MeshFilter not found!");
                return;
            }

            var p0 = new Vector3(0, 0, 0);
            var p1 = new Vector3(1, 0, 0);
            var p2 = new Vector3(0.5f, 0, Mathf.Sqrt(0.75f));
            var p3 = new Vector3(0.5f, Mathf.Sqrt(0.75f), Mathf.Sqrt(0.75f)/3);

            Mesh mesh = meshFilter.sharedMesh;
            if (mesh == null)
            {
                meshFilter.mesh = new Mesh();
                mesh = meshFilter.sharedMesh;
            }
            mesh.Clear();
            mesh.vertices = new[] {p0, p1, p2, p3};
            mesh.triangles = new[]
                {
                    0, 1, 2,
                    0, 2, 3,
                    2, 1, 3,
                    0, 3, 1
                };

            mesh.RecalculateNormals();
            mesh.RecalculateBounds();
            mesh.Optimize();

            czy = true;
        }
    }
}