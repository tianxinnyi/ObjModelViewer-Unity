using UnityEngine;

public class CameraOrbit : MonoBehaviour
{
    public Transform target;
    public float distance = 5f;
    public float rotSpeed = 2f;
    public float zoomSpeed = 2f;
    private float rotX, rotY;

    void Start()
    {
        Vector3 angles = transform.eulerAngles;
        rotX = angles.x;
        rotY = angles.y;
    }

    void LateUpdate()
    {
        if (!target) return;
        // 鼠标左键拖拽旋转
        if (Input.GetMouseButton(0))
        {
            rotY += Input.GetAxis("Mouse X") * rotSpeed;
            rotX -= Input.GetAxis("Mouse Y") * rotSpeed;
        }
        // 滚轮缩放
        distance += Input.GetAxis("Mouse ScrollWheel") * -zoomSpeed;

        Quaternion rot = Quaternion.Euler(rotX, rotY, 0);
        Vector3 pos = rot * new Vector3(0,0,-distance) + target.position;
        transform.rotation = rot;
        transform.position = pos;
    }
}