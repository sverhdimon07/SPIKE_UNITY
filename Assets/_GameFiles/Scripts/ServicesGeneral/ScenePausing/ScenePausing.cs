using UnityEngine;

public sealed class ScenePausing //пересмотреть по аналогии с SceneLoading
{
    public void PauseOrResume() //скорее всего хардкод, но мб и валидно
    {
        if (Time.timeScale == 1f)
        {
            Pause();
        }
        else if (Time.timeScale == 0f)
        {
            Resume();
        }
    }

    private void Pause()
    {
        Time.timeScale = 0f;
    }

    private void Resume()
    {
        Time.timeScale = 1f;
    }
}
