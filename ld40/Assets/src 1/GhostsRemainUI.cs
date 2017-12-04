using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class GhostsRemainUI : MonoBehaviour
{
    private Text txt;

    void Start()
    {
        txt = GetComponent<Text>();
    }

    void Update()
    {
		txt.text = GameDirector.Instance.RemainingHaunts.ToString();
    }
}
