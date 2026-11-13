public sealed class UIController
{
    private readonly GameplayMenuUI _pauseMenu;
    private readonly WinUI _winUI;
    private readonly LoseUI _loseUI;
    private readonly InputController _input;
    private readonly ScenePausing _scenePausing;
    private readonly SceneLoading _sceneLoading;
    private readonly ScoreController _score;
    private readonly GameSaveLoadInteractor _saveLoad;
    private bool _hasResult;

    public UIController(GameplayMenuUI pauseMenu, WinUI winUI, LoseUI loseUI,
        InputController input, ScenePausing scenePausing, SceneLoading sceneLoading, ScoreController score,
        GameSaveLoadInteractor saveLoad)
    {
        _pauseMenu = pauseMenu;
        _winUI = winUI;
        _loseUI = loseUI;
        _input = input;
        _scenePausing = scenePausing;
        _sceneLoading = sceneLoading;
        _score = score;
        _saveLoad = saveLoad;

        _pauseMenu.Initialize();
        _winUI?.Initialize();
        _loseUI?.Initialize();
    }

    public void Bind()
    {
        _input.OpeningGameplayeMenuButtonPressed += TogglePauseMenu;
        _pauseMenu.ContinueButton.onClick.AddListener(Resume);
        _pauseMenu.ExitButton.onClick.AddListener(_sceneLoading.LoadMainMenuScene);
        if (_saveLoad != null)
        {
            _pauseMenu.SaveButton.onClick.AddListener(_saveLoad.ExecuteSaveOperation);
            _pauseMenu.LoadSavingButton.onClick.AddListener(_saveLoad.ReloadFromSave);
        }

        if (_winUI != null)
        {
            _winUI.MenuButton.onClick.AddListener(_sceneLoading.LoadMainMenuScene);
        }

        if (_loseUI != null)
        {
            _loseUI.MenuButton.onClick.AddListener(_sceneLoading.LoadMainMenuScene);
            _loseUI.LoadLevelButton.onClick.AddListener(_sceneLoading.LoadLevelScene);
        }
    }

    public void Unbind()
    {
        _input.OpeningGameplayeMenuButtonPressed -= TogglePauseMenu;
        _pauseMenu.ContinueButton.onClick.RemoveListener(Resume);
        _pauseMenu.ExitButton.onClick.RemoveListener(_sceneLoading.LoadMainMenuScene);
        if (_saveLoad != null)
        {
            _pauseMenu.SaveButton.onClick.RemoveListener(_saveLoad.ExecuteSaveOperation);
            _pauseMenu.LoadSavingButton.onClick.RemoveListener(_saveLoad.ReloadFromSave);
        }

        if (_winUI != null)
        {
            _winUI.MenuButton.onClick.RemoveListener(_sceneLoading.LoadMainMenuScene);
        }

        if (_loseUI != null)
        {
            _loseUI.MenuButton.onClick.RemoveListener(_sceneLoading.LoadMainMenuScene);
            _loseUI.LoadLevelButton.onClick.RemoveListener(_sceneLoading.LoadLevelScene);
        }
    }

    public void SetFighterForm(bool isFighter)
    {
        _pauseMenu.SetFighterForm(isFighter);
    }

    private void TogglePauseMenu()
    {
        if (_hasResult)
        {
            return;
        }

        if (_pauseMenu.IsOpen)
        {
            Resume();
            return;
        }

        _scenePausing.Pause();
        _input.SetGameplayInputBlocked(true);
        _pauseMenu.SetOpen(true);
    }

    private void Resume()
    {
        if (_hasResult || !_pauseMenu.IsOpen)
        {
            return;
        }

        _scenePausing.Resume();
        _input.SetGameplayInputBlocked(false);
        _pauseMenu.SetOpen(false);
    }

    public void ShowVictory()
    {
        if (_hasResult)
        {
            return;
        }

        StopForResult();
        _winUI?.Show(_score.TotalScore);
        _saveLoad?.ClearSaveAfterVictory();
    }

    public void ShowDefeat()
    {
        if (_hasResult)
        {
            return;
        }

        StopForResult();
        _loseUI?.Show(_score.TotalScore);
    }

    private void StopForResult()
    {
        _hasResult = true;
        _scenePausing.Pause();
        _input.SetGameplayInputBlocked(true);
        _pauseMenu.SetOpen(false);
    }
}
