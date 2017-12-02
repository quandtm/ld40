using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hauntable : MonoBehaviour
{
	public bool IsPossessed;

	private bool animating;
	private float dumbAnimTimer;

	void Start()
	{
		IsPossessed = false;
        animating = false;

		GameDirector.Instance.RegisterHauntable(this);
		GameDirector.Instance.HintEvent += new Action(OnHint);
	}

	void Update()
	{
		if (animating && (dumbAnimTimer >= 0f))
		{
            dumbAnimTimer -= Time.deltaTime;
			transform.rotation = Quaternion.Euler(0f, 0f, 1.5f * Mathf.Sin(Mathf.Rad2Deg * dumbAnimTimer));
		}
	}

	private void OnHint()
	{
		if (IsPossessed)
		{
			dumbAnimTimer = 2f;
			animating = true;
		}
	}
}
