using UnityEngine;
using UnityEngine.UI;

public class ResultsImgSwitcher : MonoBehaviour {
	public Sprite winImage;
	public Sprite lossImage;

	void Start()
	{
        var img = GetComponent<Image>();
        if (ResultsStore.Instance.BuyersRemaining >= ResultsStore.Instance.MinBuyersForWin)
            img.sprite = winImage;
        else
            img.sprite = lossImage;
	}
}
