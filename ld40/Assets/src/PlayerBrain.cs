using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerBrain : MonoBehaviour
{
    public float Speed = 1f;
	public float HeightOffset = 1f;

	private Rewired.Player player;
	private Rigidbody body;
	private float baseHeight = 0f;

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
        if (Mathf.Abs(hmov) > float.Epsilon)
        {
            moveDist = hmov * Time.deltaTime * Speed;
            moved = true;
        }

        float vmov = player.GetAxis(1);
        bool vmovJustPressed = false;
        if (Mathf.Abs(vmov) > 0.5f)
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
            }
        }

        if (moved)
        {
            var pos = body.position;
            pos.x += moveDist;
            pos.y = baseHeight + HeightOffset;
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
