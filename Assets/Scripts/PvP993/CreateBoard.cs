using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PvP993
{

    public class CreateBoard : MonoBehaviour
    {
        private int xLength = Choose.InitialSetting.xLength;
        private int yLength = Choose.InitialSetting.yLength;
        private int zLength = Choose.InitialSetting.zLength;

        [SerializeField, Range(0.0f, 0.03f)] private float frameWidtth = 0.01f;
        public GameObject boardPrefab;
        public GameObject horiFramePrefab;
        public GameObject vertFramePrefab;
        // Start is called before the first frame update
        void Start()
        {
            Debug.Log("start() called.");
            CreateClearBoard();
            CreateFrame();
        }

        void CreateClearBoard()
        {
            int cnt = 0;
            Transform ClearBoardTransform = this.transform.GetChild(0).gameObject.transform;
            for(int y = 0; y < yLength; y++)
            {
                for (int z = 0; z < zLength; z++)
                {
                    for(int x = 0; x < xLength; x++)
                    {
                        GameObject b = Instantiate(boardPrefab, ClearBoardTransform);
                        b.transform.position = new Vector3(x, y, z);
                        b.tag = "tagB" + x + y + z;
                        cnt++;
                    }
                }
            }
            Debug.Log(cnt);
            Debug.Log("xLength is " + xLength + ",yLength is  " + yLength);
        }

        void CreateFrame()
        {
            float xCenter = (xLength - 1.0f) / 2.0f;
            float yCenter = (yLength - 1.0f) / 2.0f;
            float zCenter = (zLength - 1.0f) / 2.0f;
            Transform frameXTransform = this.transform.GetChild(1).gameObject.transform;
            Transform frameZTransform = this.transform.GetChild(2).gameObject.transform;
            Transform frameYTransform = this.transform.GetChild(3).gameObject.transform;

            horiFramePrefab.transform.localScale = new Vector3(xLength, frameWidtth, frameWidtth);
            for(int y = 0; y <= yLength; y++)
            {
                for(int z = 0; z <= zLength; z++)
                {
                    GameObject f = Instantiate(horiFramePrefab, frameXTransform);
                    f.transform.position = new Vector3(xCenter, y - 0.5f, z - 0.5f);
                }
            }

            horiFramePrefab.transform.localScale = new Vector3(frameWidtth, frameWidtth, zLength);
            for(int y = 0; y <= yLength; y++)
            {
                for(int x = 0; x <= xLength; x++)
                {
                    GameObject f = Instantiate(horiFramePrefab, frameZTransform);
                    f.transform.position = new Vector3(x - 0.5f, y - 0.5f, zCenter);
                }
            }

            vertFramePrefab.transform.localScale = new Vector3(frameWidtth, yLength, frameWidtth);
            for(int z = 0; z <= zLength; z++)
            {
                for(int x = 0; x <= xLength; x++)
                {
                    GameObject f = Instantiate(vertFramePrefab, frameYTransform);
                    f.transform.position = new Vector3(x - 0.5f, yCenter, z - 0.5f);
                }
            }
            Debug.Log("Frame Created.");
        }
    }

}
