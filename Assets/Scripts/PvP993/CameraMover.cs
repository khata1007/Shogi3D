using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PvP993
{
    public class CameraMover : MonoBehaviour
    {
        private int xLength = Choose.InitialSetting.xLength;
        private int yLength = Choose.InitialSetting.yLength;
        private int zLength = Choose.InitialSetting.zLength;
        private float movingSpeed; //カメラの動くスピードを設定
        private float squaredDistance; //カメラを球面状で動かす時の半径の二乗
        private float upLimit; //カメラの上方向に動く限界のy座標
        private float downLimit; //カメラの上方向に動く限界のy座標
        private Vector3 defaultPosition; //カメラの初期位置
        private Vector3 center;  //オセロ盤の中心位置
        private Transform mainCameraTransform;


        void Awake()
        {
            movingSpeed = PlayerPrefs.GetFloat("Value_of_MovingSpeed", 20f);
        }

        void Start()
        {
            float xCenterCoordi = (xLength - 1f) / 2f;
            float yCenterCoordi = (yLength - 1f) / 2f;
            float zCenterCoordi = (zLength - 1f) / 2f;
            center = new Vector3(xCenterCoordi, yCenterCoordi, zCenterCoordi); //中心位置の定義

            mainCameraTransform = this.gameObject.transform;
            if (yLength == 4) { mainCameraTransform.position = new Vector3(xCenterCoordi, yCenterCoordi, zCenterCoordi - 7.2f); };
            if (yLength == 6) { mainCameraTransform.position = new Vector3(xCenterCoordi, yCenterCoordi, zCenterCoordi - 8.4f); };
            defaultPosition = mainCameraTransform.position;

            squaredDistance = (defaultPosition.x - center.x) * (defaultPosition.x - center.x) + (defaultPosition.y - center.y) * (defaultPosition.y - center.y) + (defaultPosition.z - center.z) * (defaultPosition.z - center.z);
            upLimit = center.y + Mathf.Sqrt(squaredDistance) - 0.5f;
            downLimit = center.y - Mathf.Sqrt(squaredDistance) + 0.5f;

            mainCameraTransform.LookAt(center, Vector3.up);
        }

        void LateUpdate()
        {
            CameraPosotionControl();
        }


        private void CameraPosotionControl() //矢印キーでメインカメラを動かす
        {
            Vector3 pos;
            if (Input.GetKey(KeyCode.RightArrow))
            {
                pos = mainCameraTransform.position;
                float r = Mathf.Sqrt(squaredDistance - (pos.y - center.y) * (pos.y - center.y));
                float d = Time.deltaTime * movingSpeed;
                float x = center.x + (pos.x - center.x) * Mathf.Cos(d / r) - (pos.z - center.z) * Mathf.Sin(d / r);
                float z = center.z + (pos.z - center.z) * Mathf.Cos(d / r) + (pos.x - center.x) * Mathf.Sin(d / r);
                mainCameraTransform.position = new Vector3(x, pos.y, z);
            }
            if (Input.GetKey(KeyCode.LeftArrow))
            {
                pos = mainCameraTransform.position;
                float r = Mathf.Sqrt(squaredDistance - (pos.y - center.y) * (pos.y - center.y));
                float d = Time.deltaTime * movingSpeed;
                float x = center.x + (pos.x - center.x) * Mathf.Cos(d / r) + (pos.z - center.z) * Mathf.Sin(d / r);
                float z = center.z + (pos.z - center.z) * Mathf.Cos(d / r) - (pos.x - center.x) * Mathf.Sin(d / r);
                mainCameraTransform.position = new Vector3(x, pos.y, z);
            }
            if (Input.GetKey(KeyCode.UpArrow))
            {
                pos = mainCameraTransform.position;
                if (pos.y <= upLimit)
                {
                    float r = Mathf.Sqrt(squaredDistance);
                    float rxy = Mathf.Sqrt(squaredDistance - (pos.y - center.y) * (pos.y - center.y));
                    float d = Time.deltaTime * movingSpeed;
                    float y = center.y + (pos.y - center.y) * Mathf.Cos(d / r) + rxy * Mathf.Sin(d / r);
                    float con = Mathf.Sqrt((squaredDistance - (y - center.y) * (y - center.y)) / (squaredDistance - (pos.y - center.y) * (pos.y - center.y)));
                    float x = center.x + con * (pos.x - center.x);
                    float z = center.z + con * (pos.z - center.z);
                    mainCameraTransform.position = new Vector3(x, y, z);
                }
            }
            if (Input.GetKey(KeyCode.DownArrow))
            {
                pos = mainCameraTransform.position;
                if (pos.y >= downLimit)
                {
                    float r = Mathf.Sqrt(squaredDistance);
                    float rxy = Mathf.Sqrt(squaredDistance - (pos.y - center.y) * (pos.y - center.y));
                    float d = Time.deltaTime * movingSpeed;
                    float y = center.y + (pos.y - center.y) * Mathf.Cos(d / r) - rxy * Mathf.Sin(d / r);
                    float con = Mathf.Sqrt((squaredDistance - (y - center.y) * (y - center.y)) / (squaredDistance - (pos.y - center.y) * (pos.y - center.y)));
                    float x = center.x + con * (pos.x - center.x);
                    float z = center.z + con * (pos.z - center.z);
                    mainCameraTransform.position = new Vector3(x, y, z);
                }
            }
            mainCameraTransform.LookAt(center, Vector3.up);
        }

        public Vector3 MainCameraTransformPosition { get { return this.mainCameraTransform.position; } }

        public float MovingSpeed { get { return this.movingSpeed; } set { this.movingSpeed = value; } }
    }

}
