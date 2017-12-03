using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum LookDirection
{
    Left = 0,
    Right = 180,
    Background = 90,
    Foreground = 270
}

[RequireComponent(typeof(Rigidbody))]
public class PlayerBrain : MonoBehaviour
{
    public float Speed = 1f;
    public float HorizontalDeadzone = 0.3f;
    public float VerticalThreshold = 0.8f;
    public float HeightOffset = 1f;
    public float LeftYRotation = 0f;

    private Rigidbody body;
    private float baseHeight = 0f;
    private LookDirection lookDir = LookDirection.Left;

    private StairZone stairZone;
    private bool vMovHandled = false;
    private Hauntable[] proxHaunts = new Hauntable[2]; // 0=fore, 1=back

    public GameObject mesh;
    private Animator anim;
    private int speedParamHash;
    private int scareHash;

    void Start()
    {
        body = GetComponent<Rigidbody>();
        anim = mesh.GetComponent<Animator>();
        speedParamHash = Animator.StringToHash("MoveSpeed");
        scareHash = Animator.StringToHash("DoScare");
    }

    void Update()
    {
        if (GameDirector.Instance.CurrentPhase == Phase.Play)
        {
            if (Input.GetButtonUp("Fire1"))
            {
                bool shouldAnim = false;
                if (lookDir == LookDirection.Background)
                {
                    if (proxHaunts[1] != null && !proxHaunts[1].PreviouslyInteracted)
                    {
                        // TODO: Animation based on IsPossessed state BEFORE change
                        if (proxHaunts[1].IsPossessed)
                        {
                            proxHaunts[1].IsPossessed = false;
                            proxHaunts[1].PreviouslyInteracted = true;
                            Debug.Log("Ghost eliminated");
                            shouldAnim = true;
                        }
                    }
                }
                else if (lookDir == LookDirection.Foreground)
                {
                    if (proxHaunts[0] != null && !proxHaunts[0].PreviouslyInteracted)
                    {
                        // TODO: Animation based on IsPossessed state BEFORE change
                        if (proxHaunts[0].IsPossessed)
                        {
                            proxHaunts[0].IsPossessed = false;
                            proxHaunts[0].PreviouslyInteracted = true;
                            Debug.Log("Ghost eliminated");
                            shouldAnim = true;
                        }
                    }
                }

                if (shouldAnim)
                    anim.SetTrigger(scareHash);
            }
            else
            {
                float speed = body.velocity.magnitude;
                if (speed > 0f)
                    anim.SetFloat(speedParamHash, speed > HorizontalDeadzone ? speed : 0f);
            }
        }
    }

    void FixedUpdate()
    {
        if (GameDirector.Instance.CurrentPhase == Phase.Play)
        {
            float vmov = Input.GetAxis("Vertical");
            bool vmovJustPressed = false;
            if (Mathf.Abs(vmov) > VerticalThreshold)
            {
                if (!vMovHandled)
                {
                    vMovHandled = true;
                    vmovJustPressed = true;
                }
            }
            else
            {
                vMovHandled = false;

                float hmov = Input.GetAxis("Horizontal");
                float absHmov = Mathf.Abs(hmov);
                if (absHmov > HorizontalDeadzone)
                {
                    float moveDist = Mathf.Sign(hmov) * ((absHmov - .3f) / .7f) * Speed;
                    lookDir = moveDist > 0f ? LookDirection.Right : LookDirection.Left;
                    body.velocity = new Vector3(moveDist, 0f, 0f);
                    var rot = Quaternion.Euler(0f, LeftYRotation + (float)lookDir, 0f);
                    body.rotation = rot;
                }
            }

            if (vmovJustPressed)
            {
                if (stairZone != null)
                {
                    if (vmov > 0f)
                        baseHeight = stairZone.GetUpZoneHeight();
                    else
                        baseHeight = stairZone.GetDownZoneHeight();

                    body.velocity = Vector3.zero;
                    var p = transform.position;
                    p.y = baseHeight + HeightOffset;
                    transform.position = p;
                }
                else
                {
                    if (vmov > 0f)
                    {
                        if (proxHaunts[1] != null)
                            lookDir = LookDirection.Background;
                    }
                    else
                    {
                        if (proxHaunts[0] != null)
                            lookDir = LookDirection.Foreground;
                    }

                    var rot = Quaternion.Euler(0f, LeftYRotation + (float)lookDir, 0f);
                    body.rotation = rot;
                }
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        var zone = other.GetComponent<StairZone>();
        if (zone != null)
            stairZone = zone;
        else
        {
            var haunt = other.GetComponent<Hauntable>();
            if (haunt != null)
            {
                if (haunt.transform.position.z < transform.position.z)
                {
                    // Foreground
                    proxHaunts[0] = haunt;
                }
                else
                {
                    // Background
                    proxHaunts[1] = haunt;
                }
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        var zone = other.GetComponent<StairZone>();
        if (zone != null)
            stairZone = null;
        else
        {
            var haunt = other.GetComponent<Hauntable>();
            if (haunt != null)
            {
                if (haunt.transform.position.z < transform.position.z)
                {
                    // Foreground
                    proxHaunts[0] = null;
                }
                else
                {
                    // Background
                    proxHaunts[1] = null;
                }
            }
        }
    }
}
