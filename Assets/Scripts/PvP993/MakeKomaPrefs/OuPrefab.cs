using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PvP993.MakeKomaPrefs
{
    [RequireComponent(typeof(MeshRenderer))]
    [RequireComponent(typeof(MeshFilter))]

    public class OuPrefab : MonoBehaviour
    {
        [SerializeField] private Material _mat;

        // Start is called before the first frame update
        void Start()
        {
            float[] sunpou = new float[3];
            for (int i = 0; i < 3; i++) sunpou[i] = PvP993.Koma.sunpou[7, i];

            float[] degs = new float[3];
            for (int i = 0; i < 3; i++) degs[i] = PvP993.Koma.deg[7, i];

            float c = sunpou[0], b = sunpou[1], d = sunpou[2];
            float P = Mathf.Deg2Rad * degs[0], R = Mathf.Deg2Rad * degs[1], Gamma = Mathf.Deg2Rad * degs[2];
            float h = d / 2.0f - b / (Mathf.Tan(Gamma) * 2);
            float a = (b * Mathf.Cos(R) - c * Mathf.Sin(R) / 2) / Mathf.Cos(P / 2 + R);
            float id = 0.5f * b / Mathf.Tan(Gamma);

            Mesh mesh = new Mesh();

            Vector3[] positions = new Vector3[]
            {
                new Vector3(0f, h, 0f),
                new Vector3(0f, h-id, 0.5f*b),
                new Vector3(a*Mathf.Sin(P/2), h-id+a*Mathf.Cos(P/2)/Mathf.Tan(Gamma), 0.5f*b-a*Mathf.Cos(P/2)),
                new Vector3(c/2, h+id, -0.5f*b),
                new Vector3(-c/2, h+id, -0.5f*b),
                new Vector3(-a*Mathf.Sin(P/2), h-id+a*Mathf.Cos(P/2)/Mathf.Tan(Gamma), 0.5f*b-a*Mathf.Cos(P/2)),

                new Vector3(0f, -h, 0f),
                new Vector3(0f, -(h-id), 0.5f*b),
                new Vector3(a*Mathf.Sin(P/2), -(h-id+a*Mathf.Cos(P/2)/Mathf.Tan(Gamma)), 0.5f*b-a*Mathf.Cos(P/2)),
                new Vector3(c/2, -(h+id), -0.5f*b),
                new Vector3(-c/2, -(h+id), -0.5f*b),
                new Vector3(-a*Mathf.Sin(P/2), -(h-id+a*Mathf.Cos(P/2)/Mathf.Tan(Gamma)), 0.5f*b-a*Mathf.Cos(P/2)),
            };
            mesh.vertices = new Vector3[]
            {
                //天板
                positions[ 0],positions[ 1],positions[ 2],
                positions[ 0],positions[ 2],positions[ 3],
                positions[ 0],positions[ 3],positions[ 4],
                positions[ 0],positions[ 4],positions[ 5],
                positions[ 0],positions[ 5],positions[ 1],

                //底板
                positions[ 6],positions[ 8],positions[ 7],
                positions[ 6],positions[ 9],positions[ 8],
                positions[ 6],positions[10],positions[ 9],
                positions[ 6],positions[11],positions[10],
                positions[ 6],positions[ 7],positions[11],

                //側面
                positions[ 2],positions[ 1],positions[ 7],
                positions[ 2],positions[ 7],positions[ 8],

                positions[ 3],positions[ 2],positions[ 8],
                positions[ 3],positions[ 8],positions[ 9],

                positions[ 4],positions[ 3],positions[ 9],
                positions[ 4],positions[ 9],positions[10],

                positions[ 5],positions[ 4],positions[10],
                positions[ 5],positions[10],positions[11],

                positions[ 1],positions[ 5],positions[11],
                positions[ 1],positions[11],positions[ 7],
            };

            int[] triangles0 = new int[mesh.vertices.Length]; //駒の表面全体にマテリアルを設定
            for (int i = 0; i < triangles0.Length; i++) triangles0[i] = i;
            mesh.SetTriangles(triangles0, 0);

            mesh.RecalculateNormals();

            var filter = GetComponent<MeshFilter>();
            filter.sharedMesh = mesh;
            var renderer = GetComponent<MeshRenderer>();
            renderer.material = _mat;
        }

        private void Update()
        {
            //this.transform.Rotate(new Vector3(0f, 0f, 1.0f));
        }
    }
}