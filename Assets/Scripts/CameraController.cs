using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float sensitivityX = 2.0f;  // Czułość na oś X
    public float sensitivityY = 2.0f;  // Czułość na oś Y
    public Transform playerBody;       // Obiekt gracza (do obracania ciała)



    private float rotationX = 0f;      // Kąt obrotu na osi X (pionowej)

    void Update()
    {
        // Pobieramy ruch myszy
        float mouseX = Input.GetAxis("Mouse X") * sensitivityX;
        float mouseY = Input.GetAxis("Mouse Y") * sensitivityY;

        // Obrót kamery na osi X (góra-dół)
        rotationX -= mouseY;
        rotationX = Mathf.Clamp(rotationX, -90f, 90f); // Ograniczenie kąta obrotu na osi X

        // Obrót kamery na osi Y (lewo-prawo)
        transform.localRotation = Quaternion.Euler(rotationX, 0, 0f);

        // Obracamy ciało gracza na osi Y
        playerBody.Rotate(Vector3.up * mouseX);
    }
}
