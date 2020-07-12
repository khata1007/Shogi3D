using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PvP993
{

    public class Board : MonoBehaviour
    {
        private int xLength = Choose.InitialSetting.xLength;
        private int yLength = Choose.InitialSetting.yLength;
        private int zLength = Choose.InitialSetting.zLength;

        private Color[] frameColor = new Color[]
        {
            new Color(192/255.0f, 159/255.0f, 116/255.0f),
            new Color(105/255.0f, 89/255.0f, 70/255.0f),
            new Color(75/255.0f, 63/255.0f, 48/255.0f),
        };


        [SerializeField, Range(0.0f, 0.03f)] private float frameWidth = 0.01f;
        private static float boardScale2D;
        public GameObject orangeBoardPrefab3D;
        public GameObject greenBoardPrefab3D;
        public GameObject framePrefab;

        public GameObject orangeBoardPrefab2D;
        public GameObject greenBoardPrefab2D;
        // Start is called before the first frame update
        void Start()
        {
            
        }

        public void CreateBoard(GameObject[,,] ob3D, GameObject[,,] gb3D, GameObject[,,] ob2D, GameObject[,,] gb2D)
        {
            Transform orangeBoardTransform = this.transform.GetChild(0).gameObject.transform;
            Transform greenBoardTransform = this.transform.GetChild(1).gameObject.transform;
            for(int y = 0; y < yLength; y++)
            {
                for (int z = 0; z < zLength; z++)
                {
                    for(int x = 0; x < xLength; x++)
                    {
                        ob3D[x, y, z] = Instantiate(orangeBoardPrefab3D, orangeBoardTransform);
                        ob3D[x, y, z].transform.position = new Vector3(x, y-0.5f, z);
                        ob3D[x, y, z].SetActive(false);
                        gb3D[x, y, z] = Instantiate(greenBoardPrefab3D, greenBoardTransform);
                        gb3D[x, y, z].transform.position = new Vector3(x, y-0.5f, z);
                        gb3D[x, y, z].SetActive(false);
                    }
                }
            }
            gb3D[0, 0, 0].SetActive(true);


            orangeBoardPrefab2D.transform.localScale = new Vector3(boardScale2D , 0.01f, boardScale2D);
            greenBoardPrefab2D.transform.localScale = new Vector3(boardScale2D, 0.01f, boardScale2D);
        }

        public void CreateFrame(GameObject[,] fx3D, GameObject[,] fz3D, GameObject[,] fx2D, GameObject[,] fz2D)
        {
            float xCenter = (xLength - 1.0f) / 2.0f;
            float zCenter = (zLength - 1.0f) / 2.0f;
            Transform frameXTransform = this.transform.GetChild(2).gameObject.transform;
            Transform frameZTransform = this.transform.GetChild(3).gameObject.transform;

            // ---------------- 3D盤面のフレーム作成 ------------------ //
            
            framePrefab.transform.localScale = new Vector3(xLength, frameWidth, frameWidth);
            for(int y = 0; y < yLength; y++)
            {
                for(int z = 0; z <= zLength; z++)
                {
                    fx3D[y, z] = Instantiate(framePrefab, frameXTransform);
                    fx3D[y, z].GetComponent<Renderer>().material.color = frameColor[y];
                    fx3D[y, z].transform.position = new Vector3(xCenter, y - 0.5f, z - 0.5f);
                }
            }

            framePrefab.transform.localScale = new Vector3(frameWidth, frameWidth, zLength);
            for(int y = 0; y < yLength; y++)
            {
                for (int x = 0; x <= xLength; x++)
                {
                    fz3D[y, x] = Instantiate(framePrefab, frameZTransform);
                    fz3D[y, x].GetComponent<Renderer>().material.color = frameColor[y];
                    fz3D[y, x].transform.position = new Vector3(x - 0.5f, y - 0.5f, zCenter);
                }
            }
            
            // ---------------- 2D盤面のフレーム作成 ------------------ //
            xCenter *= boardScale2D;
            zCenter *= boardScale2D;
            framePrefab.transform.localScale = new Vector3(boardScale2D * xLength, frameWidth, frameWidth);
            for(int y = 0; y < yLength; y++)
            {
                float offset = (y - 1) * 10 * boardScale2D;
                for(int z = 0; z <= zLength; z++)
                {
                    fx2D[y, z] = Instantiate(framePrefab, frameXTransform);
                    fx2D[y, z].GetComponent<Renderer>().material.color = frameColor[y];
                    fx2D[y, z].transform.position = new Vector3(xCenter + offset, 0, z * boardScale2D - 0.4f);
                    fx2D[y, z].SetActive(false);
                }
            }

            framePrefab.transform.localScale = new Vector3(frameWidth, frameWidth, boardScale2D * zLength);
            for (int y = 0; y < yLength; y++)
            {
                float offset = (y - 1) * 10 * boardScale2D;
                for (int x = 0; x <= xLength; x++)
                {
                    fz2D[y, x] = Instantiate(framePrefab, frameZTransform);
                    fz2D[y, x].GetComponent<Renderer>().material.color = frameColor[y];
                    fz2D[y, x].transform.position = new Vector3(x * boardScale2D - 0.4f + offset, 0, zCenter);
                    fz2D[y, x].SetActive(false);
                }
            }
            Debug.Log("2DFrame Created.");
        }

        public static float BoardScale2D { set { boardScale2D = value; } }
    }

}
