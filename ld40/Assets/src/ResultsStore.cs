public class ResultsStore
{
    private static ResultsStore inst;
    public static ResultsStore Instance
    {
        get
        {
            if (inst == null)
                inst = new ResultsStore();
            return inst;
        }
    }

    public int BuyersRemaining;
    public float TimeRemaining;
    public int MinBuyersForWin;
}