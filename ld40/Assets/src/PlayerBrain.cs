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

	private Rewired.Player player;
	private Rigidbody body;
	private float baseHeight = 0f;
	private LookDirection lookDir = LookDirection.Left;

	private StairZone stairZone;
	private bool vMovHandled = false;

	void Start()
	{
		player = Rewired.ReInput.players.GetPlayer("MainPlayer");
		body = GetComponent<Rigidbody>();
    }

    void Update()
    {
        bool moved = false;

        float hmov = player.GetAxis(0);
        float moveDist = 0f;
        if (Mathf.Abs(hmov) > HorizontalDeadzone)
        {
            moveDist = hmov * Time.deltaTime * Speed;
			lookDir = moveDist > 0f ? LookDirection.Right : LookDirection.Left;
            moved = true;
        }

        float vmov = player.GetAxis(1);
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
        }

        if (vmovJustPressed)
        {
            if (stairZone != null)
            {
                if (vmov > 0f)
                    baseHeight = stairZone.GetUpZoneHeight();
                else
                    baseHeight = stairZone.GetDownZoneHeight();
                moved = true;
            }
            else
            {
                lookDir = vmov > 0f ? LookDirection.Background : LookDirection.Foreground;
				moved = true;
            }
        }

        if (moved)
        {
            var pos = body.position;
            pos.x += moveDist;
            pos.y = baseHeight + HeightOffset;
            var rot = Quaternion.Euler(0f, LeftYRotation + (float)lookDir, 0f);
			body.MoveRotation(rot);
            body.MovePosition(pos);
        }
    }

	void OnTriggerEnter(Collider other)
	{
		var zone = other.GetComponent<StairZone>();
		if (zone != null)
			stairZone = zone;
	}

	void OnTriggerExit(Collider other)
	{
		var zone = other.GetComponent<StairZone>();
		if (zone != null)
			stairZone = null;
	}
}
