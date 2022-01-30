namespace Game
{
    public class LastScoreSystem : PersistentSingleton<LastScoreSystem>
    {
        private int lastScore = 0;

        public int LastScore => lastScore;

        public void UpdateLastScore(int score) => lastScore = score;
    }
}