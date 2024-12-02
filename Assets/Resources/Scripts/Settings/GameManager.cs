using System.Collections;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public SaveData savaData;
    public SceneManagerSettings sceneManagerSettings;
    public AudioManager audioManager;
    public PlayerController playerController;
    public GameObject missile, banner, menu, tutorial;
    public Transform[] spawnsShoot;
    public Animator animPanelReplayEnd, animMenu, animTutorial, animSettings;
    public TextMeshProUGUI timeDisplay, textMeshReplayEnd, textButtonMeshReplayEnd;
    [HideInInspector]
    public int currentScore;
    [HideInInspector]
    public bool isEnd;
    [HideInInspector]
    public bool isReplay;
    [HideInInspector]
    public bool isStart = false;

    private int _initialScore = 50000, _accumulationRate = 1;
    private float _seconds, _afterAppearanceTime, _time, _minDelay = 5.0f, _maxDelay = 15.0f, _appearanceDelay;
    private string _textPanel, _textButton;
    private const string TEXT_REPLAY = "Take another chance", TEXT_TIME = "Time completed :\n\r", TEXT_SCORE = "\n\r\n\rScore :\n\r", TEXT_BUTTON_REPLAY = "Replay", TEXT_BUTTON_END = "Continue";

    private void Start()
    {
        isStart = false;
        banner.SetActive(false);
        currentScore = _initialScore;
        _appearanceDelay = Random.Range(_minDelay, _maxDelay);
    }

    private void Update()
    {
        if (isStart)
        {
            if (!playerController.isDead)
            {
                banner.SetActive(true);
            }
            SpawnPoint();
            TimeElapsed();
        }
    }

    private void SpawnPoint()
    {
        if (playerController != null)
        {
            if (!playerController.isDead && !playerController.isEnd)
            {
                _afterAppearanceTime += Time.deltaTime;

                if (_afterAppearanceTime > _appearanceDelay)
                {
                    SpawnMissile();
                }
            }
        }
    }

    private void SpawnMissile()
    {
        int randomIndex = Random.Range(0, spawnsShoot.Length);
        Transform spawnRandom = spawnsShoot[randomIndex];
        GameObject instantiated = Instantiate(missile);

        _appearanceDelay = Random.Range(_minDelay, _maxDelay);
        instantiated.transform.position = new(spawnRandom.position.x, spawnRandom.position.y, 1);
        _afterAppearanceTime = 0f;
    }

    private void TimeElapsed()
    {
        if (playerController != null)
        {
            if (!playerController.isDead && !playerController.isEnd)
            {
                timeDisplay.text = UpdateTime();
                int.TryParse(timeDisplay.text, out int time);
                currentScore -= Mathf.RoundToInt(_accumulationRate + time);
                savaData.lastScore = currentScore;
            }
        }
    }

    private string UpdateTime()
    {
        _time += Time.deltaTime; ;
        string minutes = Mathf.FloorToInt(_time / 60).ToString("0");
        string seconds = Mathf.FloorToInt(_time % 60).ToString(" : 00 : ");
        string thousandth = (Mathf.FloorToInt(_time * 1000) % 1000).ToString("000");
        string currentTime = minutes + seconds + thousandth;

        return currentTime;
    }

    // Affichage du panneau de fin
    public void EndGame()
    {
        audioManager.audioSource[audioManager.numberSoundLevel].Stop();

        if (playerController != null)
        {
            if (playerController.isOutEnd)
            {
                isReplay = false;
                isEnd = true;
                audioManager.audioSource[audioManager.numberSoundEnd].Play();
                savaData.SavePlayerData();
            }

            if (playerController.isDead)
            {
                isEnd = false;
                isReplay = true;
                audioManager.audioSource[audioManager.numberSoundLose].Play();
            }
            TextEndGame();
        }
    }

    private void TextEndGame()
    {
        string textCurrentScore = currentScore.ToString();

        if (banner != null && animPanelReplayEnd != null)
        {
            if (isReplay)
            {
                _textPanel = TEXT_REPLAY;
                _textButton = TEXT_BUTTON_REPLAY;
            }
            else if (isEnd)
            {
                _textPanel = TEXT_TIME + UpdateTime() + TEXT_SCORE + textCurrentScore + "\n\r";
                _textButton = TEXT_BUTTON_END;
            }
            banner.SetActive(false);
            animPanelReplayEnd.SetBool("Open End Game", true);
            textMeshReplayEnd.text = _textPanel;
            textButtonMeshReplayEnd.text = _textButton;
        }
    }

    public void PanelGameStart()
    {
        audioManager.PlaySounds(audioManager.sounds[audioManager.numberSfxClick], audioManager.mixerSounds[audioManager.numberSfxClick], transform.position);
        animPanelReplayEnd.SetBool("Close End Game", true);
        StartCoroutine(TimerLoadScene());

        IEnumerator TimerLoadScene()
        {
            float seconds = 1.0f;

            yield return new WaitForSeconds(seconds);
            playerController.gameObject.SetActive(true);
            sceneManagerSettings.LoadScene();
        }
    }

    // Lancer le jeu
    public void GameStart()
    {
        audioManager.PlaySounds(audioManager.sounds[audioManager.numberSfxClick], audioManager.mixerSounds[audioManager.numberSfxClick], transform.position);
        animMenu.SetBool("Open Menu", false);
        animMenu.SetBool("Close Menu", true);
        StartCoroutine(TimerGameStart());

        IEnumerator TimerGameStart()
        {
            _seconds = 0.5f;
            yield return new WaitForSeconds(_seconds);
            isStart = true;
            menu.SetActive(false);
            animMenu.SetBool("Close Menu", false);
        }
    }

    // Aller au panneau du tutoriel
    public void Tutorial()
    {
        audioManager.PlaySounds(audioManager.sounds[audioManager.numberSfxClick], audioManager.mixerSounds[audioManager.numberSfxClick], transform.position);
        animTutorial.SetBool("Close Tutorial", false);
        animMenu.SetBool("Open Menu", false);
        animMenu.SetBool("Close Menu", true);
        StartCoroutine(TimerMenus(animTutorial, "Open Tutorial"));
    }

    public void BackTutorial()
    {
        audioManager.PlaySounds(audioManager.sounds[audioManager.numberSfxClick], audioManager.mixerSounds[audioManager.numberSfxClick], transform.position);
        animMenu.SetBool("Close Menu", false);
        animTutorial.SetBool("Open Tutorial", false);
        animTutorial.SetBool("Close Tutorial", true);
        StartCoroutine(TimerMenus(animMenu, "Open Menu"));
    }

    public void Settings()
    {
        audioManager.PlaySounds(audioManager.sounds[audioManager.numberSfxClick], audioManager.mixerSounds[audioManager.numberSfxClick], transform.position);
        animSettings.SetBool("Close Settings", false);
        animMenu.SetBool("Open Menu", false);
        animMenu.SetBool("Close Menu", true);
        StartCoroutine(TimerMenus(animSettings, "Open Settings"));
    }

    public void BackSettings()
    {
        audioManager.PlaySounds(audioManager.sounds[audioManager.numberSfxClick], audioManager.mixerSounds[audioManager.numberSfxClick], transform.position);
        animMenu.SetBool("Close Menu", false);
        animSettings.SetBool("Open Settings", false);
        animSettings.SetBool("Close Settings", true);
        StartCoroutine(TimerMenus(animMenu, "Open Menu"));
    }

    public void ExitApp()
    {
        audioManager.PlaySounds(audioManager.sounds[audioManager.numberSfxClick], audioManager.mixerSounds[audioManager.numberSfxClick], transform.position);
        StartCoroutine(TimerExit());

        IEnumerator TimerExit()
        {
            _seconds = 0.5f;
            yield return new WaitForSeconds(_seconds);
            Application.Quit();
        }
    }

    private IEnumerator TimerMenus(Animator animMenu, string name)
    {
        _seconds = 0.3f;
        yield return new WaitForSeconds(_seconds);
        animMenu.SetBool(name, true);
    }
}