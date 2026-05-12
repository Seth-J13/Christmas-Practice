using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCameraMovement : MonoBehaviour
{
    [SerializeField] float camHeight = 0.7f;
    private new Transform camera;
    private void OnEnable()
    {
        PlayerMovement.look += UpdateCamPos;
        PlayerMovement.playerMoving += TiltCamera;
    }
    private void OnDisable()
    {
        PlayerMovement.look -= UpdateCamPos;
        PlayerMovement.playerMoving -= TiltCamera;
    }
    private void Start()
    {
        camera = transform.GetChild(0).transform;
        camera.localPosition = new Vector3(0, camHeight, 0);
    }
    public void UpdateCamPos(float lookSpeedX, float lookSpeedY, float lookAngleX, float lookAngleY, Vector3 playerPos)
    {
        //Camera position to player position
        transform.position = playerPos;
        //camera faces same way as player
        float headMovement = lookAngleY * -lookSpeedY * Time.deltaTime;
        if (headMovement > 85)
            headMovement = 85;
        else if (headMovement < -90)
            headMovement = -90;
        //Change camera rotation
        camera.rotation = Quaternion.Euler(headMovement, lookAngleX * lookSpeedX * Time.deltaTime, 0);
    }
    private void TiltCamera(Vector3 playerDir)
    {
        transform.rotation = Quaternion.Euler(transform.eulerAngles.x + (playerDir.x * 10), transform.eulerAngles.y, transform.eulerAngles.z + (playerDir.z * 10));
        StartCoroutine(TiltBack(playerDir));
    }
    IEnumerator TiltBack(Vector3 playerDir)
    {
        yield return new WaitForSeconds(2.5f);
        transform.rotation = Quaternion.Euler(transform.eulerAngles.x - (playerDir.x * 10), transform.eulerAngles.y, transform.eulerAngles.z - (playerDir.z * 10));
    }
}
