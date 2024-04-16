using UnityEngine;
using UnityEngine.SceneManagement;

public class NormalDifficulty : MonoBehaviour
{
    public BossPatternManager<Boss1> bossPatternManager;

    public void SetNormalDifficulty()
    {
        if (bossPatternManager != null)
        {
            bossPatternManager.difficulty = DifficultyMode.Normal;
        }
        SceneManager.LoadScene("Playground");
    }
}
