using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MakeKomaPrefs
{
    public class Sunpou : MonoBehaviour
    {
        //-----------------------駒の設計図-----------------------//
        //備考: sunpou と deg は ナンバリングが fu, kyo, kei, gin, kin, kak, hi, ou, gyo になっているので Kind との混同に注意
        //      本当は直すべきだけど sunpou と deg に触れるのは prefab 生成時だけなのでこのまま放置
        private const float k = 0.9f / 31; //王将のz軸方向が0.90fになるようにする
        public static readonly float[,] sunpou =
        {
        { k*22.0f,k*27.0f,k*7.6f},
        { k*23.0f,k*28.0f,k*7.9f},
        { k*25.0f,k*28.0f,k*7.9f},
        { k*26.0f,k*29.0f,k*8.2f},
        { k*26.0f,k*29.0f,k*8.2f},
        { k*27.0f,k*30.0f,k*8.6f},
        { k*27.0f,k*30.0f,k*8.6f},
        { k*28.0f,k*31.0f,k*9.2f},
        { k*28.0f,k*31.0f,k*9.2f},
    };
        public static readonly float[,] deg =
        {
        {146.0f, 80.5f, 85.0f },
        {146.0f, 80.5f, 85.0f },
        {146.0f, 80.5f, 85.0f },
        {146.0f, 80.5f, 85.0f },
        {146.0f, 80.5f, 85.0f },
        {146.0f, 80.5f, 85.0f },
        {146.0f, 80.5f, 85.0f },
        {146.0f, 80.5f, 85.0f },
        {146.0f, 80.5f, 85.0f },
    };
        //---------------ここまで駒の設計図-----------------//
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}
