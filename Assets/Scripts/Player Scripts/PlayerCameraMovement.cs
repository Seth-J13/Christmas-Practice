using UnityEngine;

public class PlayerCameraMovement : MonoBehaviour
{
    [SerializeField] float camHeight = 0.7f;
    private new Transform camera;
    private void OnEnable()
    {
        PlayerMovement.look += UpdateCamPos;
    }
    private void OnDisable()
    {
        PlayerMovement.look -= UpdateCamPos;
    }
    private void Start()
    {
        camera = transform.GetChild(0).transform;
        camera.position = new Vector3(0, camHeight, 0);
        print(transform.ToString());
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
}
