using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MakeKomaPrefs
{

    [System.Serializable]
    public class KomaPrefab2D : MonoBehaviour
    {
        [SerializeField] protected Material[] _mat = new Material[2];
        protected int mode = 0;

        protected float[] sunpou;
        protected float[] degs;

        protected float c, b, d, P, R, Gamma, h, a, id; //駒の形状の規定に使用する変数

        protected Mesh mesh; //meshの本体

        int komakind; //Fu -> 1, Kyo -> 2,...
        protected void InitVars(Sunpou.Kind k)
        {
            komakind = (int)k;
            sunpou = new float[3];
            degs = new float[3];
            for (int i = 0; i < 3; i++)
            {
                sunpou[i] = Sunpou.sunpou[komakind, i];
                degs[i] = Sunpou.deg[komakind, i];
            }
            c = sunpou[0]; b = sunpou[1]; d = sunpou[2];
            P = Mathf.Deg2Rad * degs[0]; R = Mathf.Deg2Rad * degs[1]; Gamma = Mathf.Deg2Rad * degs[2];
            h = d / 2.0f - b / (Mathf.Tan(Gamma) * 2);
            a = (b * Mathf.Cos(R) - c * Mathf.Sin(R) / 2) / Mathf.Cos(P / 2 + R);
            id = 0.5f * b / Mathf.Tan(Gamma);
            mesh = new Mesh();

            Vector3[] positions = new Vector3[]
            {
                new Vector3(0f, 0, 0f),
                new Vector3(0f, 0, 0.5f*b),
                new Vector3(a*Mathf.Sin(P/2), 0, 0.5f*b-a*Mathf.Cos(P/2)),
                new Vector3(c/2, 0, -0.5f*b),
                new Vector3(-c/2, 0, -0.5f*b),
                new Vector3(-a*Mathf.Sin(P/2), 0, 0.5f*b-a*Mathf.Cos(P/2)),
            };
            mesh.vertices = new Vector3[]
            {
            positions[1],positions[2],positions[3],positions[4],positions[5]
            };
            mesh.uv = new Vector2[]{
            new Vector2(0.5f, 1.0f),
            new Vector2(a*Mathf.Sin(P/2)/c + 0.5f, (0.5f*b-a*Mathf.Cos(P/2))/b + 0.5f),
            new Vector2(1, 0),
            new Vector2(0, 0),
            new Vector2(-a*Mathf.Sin(P/2)/c + 0.5f, (0.5f*b-a*Mathf.Cos(P/2))/b + 0.5f),
        };
            mesh.triangles = new int[]{
            0, 1, 4,
            1, 2, 4,
            2, 3, 4,
        };
            mesh.RecalculateNormals();
        }

        public void ChangeMat()
        {
            MeshRenderer mr = GetComponent<MeshRenderer>();
            if (mode == 0)
            {
                mr.material = _mat[1];
                mode = 1;
            }
            else
            {
                mr.material = _mat[0];
                mode = 0;
            }
        }

        public int Komakind { get { return komakind; } }
    }
}