using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StairZone : MonoBehaviour
{
	public float Height;
	public GameObject Upstairs;

	private StairZone upZone;
	private StairZone downZone;

    public bool HasUpZone { get { return upZone != null; } }
    public bool HasDownZone { get { return downZone != null; } }

	public float GetUpZoneHeight()
	{
		return upZone!=null ? upZone.Height : Height;
	}

	public float GetDownZoneHeight()
	{
		return downZone != null ? downZone.Height : Height;
	}

	void Start()
	{
		Height = transform.position.y;
        if (Upstairs != null)
		{
            upZone = Upstairs.GetComponent<StairZone>();
            if (upZone != null)
                upZone.downZone = this;
            else
                Debug.LogErrorFormat("Upstairs object on {0} must have a StairZone component", name);
		}
	}
}
