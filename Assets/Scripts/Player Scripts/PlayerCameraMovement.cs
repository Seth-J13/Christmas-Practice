using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCameraMovement : MonoBehaviour
{
    [SerializeField]
    float camHeight = 0.7f;
    public void UpdateCamPos(float lookSpeedX, float lookSpeedY, float lookAngleX, float lookAngleY, Vector3 playerPos)
    {
        //Camera position to player position
        transform.position = playerPos + new Vector3(0, camHeight, 0);
        //camera faces same way as player
        float headMovement = lookAngleY * -lookSpeedY * Time.deltaTime;
        if (headMovement > 85)
            headMovement = 85;
        else if (headMovement < -90)
            headMovement = -90;
        //Change camera rotation
        transform.rotation = Quaternion.Euler(headMovement, lookAngleX * lookSpeedX * Time.deltaTime, 0);
    }
}
