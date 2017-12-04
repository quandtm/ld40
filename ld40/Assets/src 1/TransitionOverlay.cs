using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RawImage))]
public class TransitionOverlay : MonoBehaviour
{
	private RawImage image;

	void Start()
	{
        image = GetComponent<RawImage>();
        var col = image.color;
        col.a = 0f;
        image.color = col;
	}

    void Update()
    {
        float transitionAmnt = GameDirector.Instance.TransitionProgress;
        if (transitionAmnt > 0f)
        {
			var col = image.color;
			col.a = Mathf.Sin(transitionAmnt * Mathf.PI);
			image.color = col;
        }
    }
}
