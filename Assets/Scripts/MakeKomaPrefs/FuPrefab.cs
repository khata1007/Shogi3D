using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MakeKomaPrefs
{

    [RequireComponent(typeof(MeshRenderer))]
    [RequireComponent(typeof(MeshFilter))]

    public class FuPrefab : KomaPrefab
    {
        int cnt = 0;

        // Start is called before the first frame update
        void Awake()
        {
            Debug.Log("start called");
            InitVars(Sunpou.Kind.Fu);
            var filter = GetComponent<MeshFilter>();
            filter.sharedMesh = mesh;
            var renderer = GetComponent<MeshRenderer>();
            renderer.material = _mat[0];
            Debug.Log("start fin");
        }

        private void Update()
        {

        }
    }
}