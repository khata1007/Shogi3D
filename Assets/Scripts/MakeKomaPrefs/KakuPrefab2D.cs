using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MakeKomaPrefs
{

    [RequireComponent(typeof(MeshRenderer))]
    [RequireComponent(typeof(MeshFilter))]

    public class KakuPrefab2D : KomaPrefab2D
    {
        int cnt = 0;

        // Start is called before the first frame update
        void Start()
        {
            InitVars(Sunpou.Kind.Kak);

            var filter = GetComponent<MeshFilter>();
            filter.sharedMesh = mesh;
            var renderer = GetComponent<MeshRenderer>();
            renderer.material = _mat[0];
        }


        private void Update()
        {

        }
    }
}