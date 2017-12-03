using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class UITimeRemaining : MonoBehaviour
{
    private Text txt;
	private const string FormatStr = "{0}:{1}";

    void Start()
    {
        txt = GetComponent<Text>();
    }

    void Update()
    {
		float time = GameDirector.Instance.SecondsRemaining;
		int min = (int)(time / 60);
		time = time % 60;
		int sec = (int)time;
		txt.text = string.Format(FormatStr, min, sec);
    }
}
