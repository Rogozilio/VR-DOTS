using UnityEngine;

public class CameraControl : MonoBehaviour
{
    public float lookSpeedH = 2f;
    public float lookSpeedV = 2f;
    public float zoomSpeed = 2f;
    public float dragSpeed = 5f;

    private float yaw;
    private float pitch;


    private void Start()
    {
        // x - right    pitch
        // y - up       yaw
        // z - forward  roll
        yaw = transform.eulerAngles.y;
        pitch = transform.eulerAngles.x;
    }

    void Update()
    {
        if (!enabled) return;

        if (UnityEngine.Input.touchCount > 0)
        {
            float touchToMouseScale = 0.25f;
            // look around with first touch
            Touch t0 = UnityEngine.Input.GetTouch(0);
            yaw += lookSpeedH * touchToMouseScale * t0.deltaPosition.x;
            pitch -= lookSpeedV * touchToMouseScale * t0.deltaPosition.y;
            transform.eulerAngles = new Vector3(pitch, yaw, 0f);

            // and if have extra touch, also fly forward
            if (UnityEngine.Input.touchCount > 1)
            {
                Touch t1 = UnityEngine.Input.GetTouch(1);
                Vector3 offset = new Vector3(t1.deltaPosition.x, 0, t1.deltaPosition.y);
                transform.Translate(offset * Time.deltaTime * touchToMouseScale, Space.Self);
            }
        }
        else
        {
            //Look around with Right Mouse
            if (UnityEngine.Input.GetMouseButton(1))
            {
                yaw += lookSpeedH * UnityEngine.Input.GetAxis("Mouse X");
                pitch -= lookSpeedV * UnityEngine.Input.GetAxis("Mouse Y");

                transform.eulerAngles = new Vector3(pitch, yaw, 0f);

                Vector3 offset = Vector3.zero;
                float offsetDelta = Time.deltaTime * dragSpeed;
                if (UnityEngine.Input.GetKey(KeyCode.LeftShift)) offsetDelta *= 5.0f;
                if (UnityEngine.Input.GetKey(KeyCode.S)) offset.z -= offsetDelta;
                if (UnityEngine.Input.GetKey(KeyCode.W)) offset.z += offsetDelta;
                if (UnityEngine.Input.GetKey(KeyCode.A)) offset.x -= offsetDelta;
                if (UnityEngine.Input.GetKey(KeyCode.D)) offset.x += offsetDelta;
                if (UnityEngine.Input.GetKey(KeyCode.Q)) offset.y -= offsetDelta;
                if (UnityEngine.Input.GetKey(KeyCode.E)) offset.y += offsetDelta;

                transform.Translate(offset, Space.Self);
            }

            //drag camera around with Middle Mouse
            if (UnityEngine.Input.GetMouseButton(2))
            {
                transform.Translate(-UnityEngine.Input.GetAxisRaw("Mouse X") * Time.deltaTime * dragSpeed, -UnityEngine.Input.GetAxisRaw("Mouse Y") * Time.deltaTime * dragSpeed, 0);
            }

            //Zoom in and out with Mouse Wheel
            transform.Translate(0, 0, UnityEngine.Input.GetAxis("Mouse ScrollWheel") * zoomSpeed, Space.Self);
        }
    }
}
