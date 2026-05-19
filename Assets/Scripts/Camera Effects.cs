using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;

public class CameraEffects : MonoBehaviour
{
    ParticleSystem runningEffect;
    GameObject player;
    Rigidbody playerBody;


    private void OnEnable()
    {
        PlayerMovement.sprinting += ManageSprintEffect;
    }
    private void OnDisable()
    {
        PlayerMovement.sprinting -= ManageSprintEffect;
    }
    private void Start()
    {
        runningEffect = transform.GetComponentInChildren<ParticleSystem>();
        playerBody = player.GetComponent<Rigidbody>();
    }
    void ManageSprintEffect()
    {
        StopAllCoroutines();
        var emission = runningEffect.emission;
        var rate = emission.rateOverTime;
        rate.constant = playerBody.linearVelocity.magnitude;
        emission.rateOverTime = rate;

        var speedVariant = runningEffect.main.startSpeed;
        speedVariant = playerBody.linearVelocity.magnitude;
        StartCoroutine(ResetSprintEffect());
    }
    public void SetPlayer(GameObject player) => this.player = player;
    IEnumerator ResetSprintEffect()
    {
        yield return new WaitForSeconds(0.5f);
        var emission = runningEffect.emission;
        var rate = emission.rateOverTime;
        rate.constant = 0;
        emission.rateOverTime = rate;
    }
}
