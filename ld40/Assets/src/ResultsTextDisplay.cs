using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class ResultsTextDisplay : MonoBehaviour
{
    private float CashPerBuyer = 100000f;
    private float BaseAmount = 500000f;
    void Start()
    {
        var txt = GetComponent<Text>();
        var results = ResultsStore.Instance;
        float amnt = results.BuyersRemaining > 0 ? BaseAmount : 0f;
        amnt += results.BuyersRemaining * CashPerBuyer;
        var min = (int)(results.TimeRemaining / 60f);
        var sec = (int)(results.TimeRemaining % 60f);
        txt.text = string.Format(txt.text, string.Format("{0:00}:{1:00}", min, sec), (results.TotalPossibleBuyers - results.BuyersRemaining), results.BuyersRemaining, amnt);
    }
}
