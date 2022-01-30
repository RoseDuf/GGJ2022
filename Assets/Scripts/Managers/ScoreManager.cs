namespace Game
{
    public class ScoreManager : Singleton<ScoreManager>
    {
        private int score = 0;

        public int Score => score;

        public void AddScore(int scoreToAdd) => score += scoreToAdd;
    }
}