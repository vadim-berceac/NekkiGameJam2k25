using UnityEngine;

public class EndWindow : MonoBehaviour
{
    [field: SerializeField] private GameObject EndWindowCanvas { get; set; }
    [field: SerializeField] private GameObject LoseCanvas { get; set; }
    [field: SerializeField] private GameObject WinCanvas { get; set; }
    [field: SerializeField] private AudioClip Lose { get; set; }
    [field: SerializeField] private AudioClip Win { get; set; }

    private const float PauseDelay = 3f;
    private bool _endGame;
    private float _pauseTimer;
    
    public static bool GameEnded { get; private set; }

    private void Awake()
    {
        EndWindowCanvas.SetActive(false);
        LoseCanvas.SetActive(false);
        WinCanvas.SetActive(false);
        Timer.OnTimerEnd += OnTimerEnd;
        Timer.OnWin += OnWin;
    }

    private void Update()
    {
        if (!_endGame)
            return;

        _pauseTimer += Time.deltaTime;
        if (_pauseTimer >= PauseDelay)
        {
            Time.timeScale = 0f;   
            _endGame = false;      
            GameEnded = true;
        }
    }

    private void OnTimerEnd()
    {
        EndWindowCanvas.SetActive(true);
        LoseCanvas.SetActive(true);
        AudioSource.PlayClipAtPoint(Lose, Camera.main.transform.position);
        _endGame = true;
        _pauseTimer = 0f;
        EnableCursor();
    }

    private void OnWin()
    {
        EndWindowCanvas.SetActive(true);
        WinCanvas.SetActive(true);
        AudioSource.PlayClipAtPoint(Win, Camera.main.transform.position);
        _endGame = true;
        _pauseTimer = 0f;
        EnableCursor();
    }

    private void EnableCursor()
    {
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;
    }

    private void OnDisable()
    {
        Timer.OnTimerEnd -= OnTimerEnd;
        Timer.OnWin -= OnWin;
        Time.timeScale = 1f;  
        GameEnded = false;
    }
}