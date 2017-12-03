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

    void Start()
    {
        body = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (GameDirector.Instance.CurrentPhase == Phase.Play)
        {
            if (Input.GetButtonUp("Fire1"))
            {
                if (lookDir == LookDirection.Background)
                {
                    if (proxHaunts[1] != null && !proxHaunts[1].PreviouslyInteracted)
                    {
                        // TODO: Animation based on IsPossessed state BEFORE change
                        if (proxHaunts[1].IsPossessed)
                        {
                            proxHaunts[1].IsPossessed = false;
                            Debug.Log("Ghost eliminated");
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
                            Debug.Log("Ghost eliminated");
                        }
                    }
                }
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
                float moveDist = 0f;
                if (Mathf.Abs(hmov) > HorizontalDeadzone)
                {
                    moveDist = hmov * Time.deltaTime * Speed;
                    lookDir = moveDist > 0f ? LookDirection.Right : LookDirection.Left;
                    float newVelocity = Speed * (moveDist > 0f ? 1f : -1f);
                    body.velocity = new Vector3(newVelocity, 0f, 0f);
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
