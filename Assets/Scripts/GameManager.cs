using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{

    public static GameManager gameManager;

    public static LevelManager levelManager;

    public static LaneController laneController;

    public static AudioManager audioManager;

    public static bool gameIsPaused = false;

    UIManager _UIManager;

    //[System.Obsolete]
    /// <summary>
    /// All <see cref="PuppetModificationBase"/> game object are stored here!
    /// </summary>
    //public GameObject PuppetModificationGameObjectParent;


    private void Awake()
    {
        if (gameManager == null)
        {
            gameManager = this;
        }
        else Destroy(gameObject);
        if(levelManager == null)
        {
            LevelManager lm = GetComponent<LevelManager>();
            if (lm == null)
            {
                levelManager = gameObject.AddComponent<LevelManager>();
            }
            else
            {
                levelManager = lm;
            }
        }
        if (laneController == null)
        {
            LaneController lc = GetComponent<LaneController>();
            if (lc == null)
            {
                laneController = gameObject.AddComponent<LaneController>();
            }
            else
            {
                laneController = lc;
            }
        }
        if (audioManager == null)
        {
            AudioManager am = GameObject.Find("AudioManager").GetComponent<AudioManager>();
            if (am == null)
            {
                Debug.LogError("There is no game object named AudioManager containing a component AudioManager in the scene.");
            }
            else
            {
                audioManager = am;
            }
        }
        DontDestroyOnLoad(this);

    }

    // Start is called before the first frame update
    void Start()
    {
        Settings.VolumeMusic = 1f;
        Settings.VolumeSfx = 1f;

        _UIManager = FindObjectOfType<UIManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && SceneManager.sceneCount != 0)
        {
            Pause();
        }
    }

    public static string GetFullParentList(Transform tr)
    {
        string s = "";
        while (tr.parent != null)
        {
            s = tr.parent.name + "/" + s;
            tr = tr.parent;
        }
        return s;
    }

    public void SceneChange(int changeTheScene)
    {
        SceneManager.LoadScene(changeTheScene);
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    public void Pause()
    {
        //pauseAudio.Play();
        gameIsPaused = !gameIsPaused;

        _UIManager.UpdatePause(gameIsPaused);

        Time.timeScale = (Time.timeScale + 1) % 2;
    }

    public void SetMusicVolume(float volume)
    {
        Settings.VolumeMusic = volume;
    }
    public void SetSfxVolume(float volume)
    {
        Settings.VolumeSfx = volume;
    }
}
