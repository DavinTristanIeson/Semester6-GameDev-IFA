using UnityEngine;
using UnityEngine.SceneManagement;

public class CasualDifficulty : MonoBehaviour
{
    public BossPatternManager<Boss1> bossPatternManager;

    public void SetCasualDifficulty()
    {
        if (bossPatternManager != null)
        {
            bossPatternManager.difficulty = DifficultyMode.Casual;
        }
        SceneManager.LoadScene("Playground");
    }
}
