using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneManagerSettings : MonoBehaviour
{
    public GameManager gameManager;
    public GameObject loading;
    public GameObject backgroundLoading;
    public GameObject backgroundSlider;
    public Animator animLoading;
    public Animator animBackgroundLoading;
    public Animator animBackgroundSlider;
    public Slider sliderProgress;
    public int currentSceneIndex;
    public bool gameIsPause;

    private int _sceneIndex, _firstSceneIndex = 0, _sceneIndexMax = 1;
    private float _seconds;

    private void Awake()
    {
        currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
    }

    public void LoadScene()
    {
        //ActiveLoading();
        //StartCoroutine(LoadImageLoading());
        SceneManager.LoadScene(SceneNumber());
        //StartCoroutine(LoadAsync(SceneNumber(name)));
    }

    private void ActiveLoading()
    {
        Time.timeScale = 1;
        gameIsPause = false;
        if (loading != null && backgroundLoading != null && backgroundSlider != null)
        {
            loading.SetActive(true);
            backgroundLoading.SetActive(true);
            backgroundSlider.SetActive(true);
        }
    }

    private int SceneNumber()
    {
        if (gameManager.isEnd)
        {
            _sceneIndex = currentSceneIndex++;
        }
        else if (gameManager.isReplay)
        {
            _sceneIndex = currentSceneIndex;
        }

        if (_sceneIndex > _sceneIndexMax)
        {
            _sceneIndex = _firstSceneIndex;
        }
        return _sceneIndex;
    }

    IEnumerator LoadImageLoading()
    {
        _seconds = 0.2f;
        yield return new WaitForSeconds(_seconds);
        if (animLoading != null && animBackgroundLoading != null && animBackgroundSlider != null)
        {
            animLoading.SetBool("Open Loading", true);
            animBackgroundLoading.SetBool("Open Text", true);
            animBackgroundSlider.SetBool("Open Slider", true);
        }
    }

    IEnumerator LoadAsync(int sceneIndex)
    {
        _seconds = 2.0f;

        yield return new WaitForSeconds(_seconds);
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneIndex);
        while (!operation.isDone)
        {
            float progress = Mathf.Clamp01(operation.progress / 0.9f);

            sliderProgress.value = progress;
            yield return null;
        }
    }
}
