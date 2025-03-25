using UnityEngine;

public class PlayerLook : MonoBehaviour
{

    public Camera cam;
    private float xRotation = 0f;

    public float xSensitivity = 90f;
    public float ySensitivity = 90f;

    public void ProcesssLook(Vector2 input)
    {
        float mouseX = input.x;
        float mouseY = input.y;

        // calculate the camera rotation for up and down
        xRotation -= (mouseY * Time.deltaTime) * ySensitivity;
        xRotation = Mathf.Clamp(xRotation, -80f, 80f);

        // apply this to the camera's transform
        cam.transform.localRotation = Quaternion.Euler(xRotation, 0, 0);

        // rotate player to look left and right
        transform.Rotate(Vector3.up * (mouseX * Time.deltaTime) * xSensitivity);
    }

    void Start()
    {
        Debug.Log("X Sensitivity: " + xSensitivity);
        Debug.Log("Y Sensitivity: " + ySensitivity);
    }
}
