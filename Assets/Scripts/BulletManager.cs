using System.Collections;
using UnityEngine;

public class BulletManager : MonoBehaviour
{
    //Variables
        //Unity Basics
    Rigidbody body;
    Camera cam;
    Transform camTransform;
        //Player
    CrossHairFireSpread crossHair;
        //Bullet Stats
    [SerializeField] private float speed = 1f;
    [SerializeField] private float bulletLifeTime = 5f;
    [SerializeField] private static float spreadDecreaser = 0.25f;
    private Vector3 spread = Vector2.one;
        //Bullet info
    private int bulletNum;
    private static Quaternion originalRot = Quaternion.Euler(0, 0, 0);
    private Vector3 originalPos;
    //Unity Basics
    private void Start()
    {
        //Get Rigidbody
        body = GetComponent<Rigidbody>();
        //Get Player Camera
        cam = GameObject.FindWithTag("MainCamera").GetComponent<Camera>();
        camTransform = cam.transform;
        //set originalPos
        originalPos = new Vector3(bulletNum * 2, -20, 0);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.name != "Player" && !other.name.StartsWith("bullet"))
        {
            StopAllCoroutines();
            ResetBullet();
        }
    }
    //Methods
    public void SetBulletSpread(Vector2 spread, CrossHairFireSpread crossHair)
    {
        this.spread = spread;
        this.crossHair = crossHair;
    }
    public void SetBulletNumber(int bulletNum)
    {
        this.bulletNum = bulletNum;
    }

    private void ResetBullet()
    {
        body.linearVelocity = Vector3.zero;
        body.rotation = originalRot;
        transform.position = originalPos;
    }
    private Vector3 CalculateSpread()
    {
        Vector3 bulletForward = transform.forward;
        Vector3 newSpread = crossHair.GetCurrentSpreadRange().normalized * spreadDecreaser;
        float x, y;
        if(newSpread.x < spread.x && newSpread.y < spread.y)
        {
            x = Random.Range(-newSpread.x, newSpread.x);
            y = Random.Range(-newSpread.y, newSpread.y);
        }
        else
        {
            x = spread.x;
            y = spread.y;
        }
        return new Vector3(bulletForward.x + x, bulletForward.y + y, bulletForward.z);
    }
    public void FireBullet()
    {
        StopAllCoroutines();
        body.transform.rotation = Quaternion.FromToRotation(transform.forward, camTransform.forward);
        body.AddForce(CalculateSpread() * speed, ForceMode.Impulse);
        StartCoroutine(PutBulletBack());
    }
    //IEnumerators
    IEnumerator PutBulletBack()
    {
        yield return new WaitForSeconds(bulletLifeTime);
        ResetBullet();
    }
}
