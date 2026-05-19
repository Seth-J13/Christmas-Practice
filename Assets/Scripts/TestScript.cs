using System.Collections.Generic;
using UnityEngine;


public class TestScript : MonoBehaviour
{
    Rigidbody body;
    ParticleSystem ps;
    private void Start()
    {
        body = GetComponent<Rigidbody>();
        ps = Resources.Load<ParticleSystem>("Prefabs/Effects/Bullet Impact");
    }
    private void OnParticleCollision(GameObject other)
    {
        print(other.ToString());
        List<ParticleCollisionEvent> collisions = new List<ParticleCollisionEvent>();
        ps.GetCollisionEvents(gameObject, collisions);
        foreach(ParticleCollisionEvent e in collisions)
        {
            body.AddForce(Vector3.Reflect(other.transform.forward, e.normal));
            Instantiate(ps, e.intersection, Quaternion.LookRotation(e.normal));
        }
    }
}
