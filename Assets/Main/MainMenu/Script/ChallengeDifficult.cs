using UnityEngine;
using UnityEngine.SceneManagement;

public class ChallengeDifficulty : MonoBehaviour
{
    public BossPatternManager<Boss1> bossPatternManager;

    public void SetChallengeDifficulty()
    {
        if (bossPatternManager != null)
        {
            bossPatternManager.difficulty = DifficultyMode.Challenge;
        }
        SceneManager.LoadScene("Playground");
    }
}
