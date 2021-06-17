﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace MakeKomaPrefs
{

    [RequireComponent(typeof(MeshRenderer))]
    [RequireComponent(typeof(MeshFilter))]

    public class OuPrefab : KomaPrefab
    {
        int cnt = 0;

        // Start is called before the first frame update
        void Awake()
        {
            InitVars(Sunpou.Kind.Ou);
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