using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour
{
    public static LevelManager instance;

    [Header("Settings")]
    [SerializeField] private float respawnTime = 1.5f;
    [SerializeField] private float levelTotalTime = 300;
    [SerializeField] private uint approvalGoalScore = 2500;

    [Header("ScoreMultipliers")]
    [SerializeField] private int scorePerDeath = -100;
    [SerializeField] private int scorePerSeconds = 5;
    [SerializeField] private int scorePerKill = 50;
    [SerializeField] private int scorePerCrystal = 100;

    [Header("UI")]
    [SerializeField] private RawImage[] uiBagels = new RawImage[3];
    [SerializeField] private RawImage rejectedBagel;
    [SerializeField] private RawImage approvedBagel;
    [SerializeField] private TextMeshProUGUI deathCountText;
    [SerializeField] private TextMeshProUGUI levelTimerText;
    [SerializeField] private TextMeshProUGUI killCountText;
    [SerializeField] private TextMeshProUGUI crystalCountText;
    [SerializeField] private TextMeshProUGUI deathCountFinal;
    [SerializeField] private TextMeshProUGUI levelTimerFinal;
    [SerializeField] private TextMeshProUGUI killCountFinal;
    [SerializeField] private TextMeshProUGUI crystalCountFinal;
    [SerializeField] private TextMeshProUGUI deathScoreText;
    [SerializeField] private TextMeshProUGUI levelTimerScoreText;
    [SerializeField] private TextMeshProUGUI killScoreText;
    [SerializeField] private TextMeshProUGUI crystalScoreText;
    [SerializeField] private TextMeshProUGUI scoreTotal;
    [SerializeField] private TextMeshProUGUI scoreApproval;
    [SerializeField] private TextMeshProUGUI goalApproval;
    [SerializeField] private Canvas hudCanvas;
    [SerializeField] private Canvas pauseCanvas;
    [SerializeField] private Canvas endingCanvas;
    [SerializeField] private Animator bagelsAnimator;

    private bool levelEnded;
    private uint killCount;
    private uint crystalCount;
    private uint deathCount;
    private float levelTimer;
    private Player player;
    private Checkpoint currentCheckpoint;

    ///////////////////////////////////////////////////////////////////////////////////////////////
    public Checkpoint CurrentCheckpoint
    {
        set { this.currentCheckpoint = value; }
    }

    ///////////////////////////////////////////////////////////////////////////////////////////////
    public float LevelTotalTime
    {
        get { return this.levelTotalTime; }
    }
    
    ///////////////////////////////////////////////////////////////////////////////////////////////
    public float LevelTimer
    {
        get { return this.levelTimer; }
        set
        {
            // Update canvas only when the value change (every second)
            float oldTimer = this.levelTimer;
            this.levelTimer = value;
            if (this.levelTimer <= 0f)
            {
                this.levelTimer = 0f;
                this.levelTimerText.SetText("0");
            }
            else if (Mathf.FloorToInt(this.levelTimer) != Mathf.FloorToInt(oldTimer))
            {
                this.levelTimerText.SetText(levelTimer.ToString("F0"));
            }
        }
    }

    ///////////////////////////////////////////////////////////////////////////////////////////////
    public uint DeathCount
    {
        get { return this.deathCount; }
        set
        {
            this.deathCount = value;
            this.deathCountText.SetText(this.deathCount.ToString());
        }
    }

    ///////////////////////////////////////////////////////////////////////////////////////////////
    public uint KillCount
    {
        get { return this.killCount; }
        set
        {
            this.killCount = value;
            this.killCountText.SetText(this.killCount.ToString());
        }
    }

    ///////////////////////////////////////////////////////////////////////////////////////////////
    public uint CrystalCount
    {
        get { return this.crystalCount; }
        set
        {
            this.crystalCount = value;
            this.crystalCountText.SetText(this.crystalCount.ToString());
        }
    }

    ///////////////////////////////////////////////////////////////////////////////////////////////
    void Awake()
    {
        LevelManager.instance = this;
        this.player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
    }

    ///////////////////////////////////////////////////////////////////////////////////////////////
    void Start()
    {
        Time.timeScale = 1f;

        this.hudCanvas.enabled = true;

        this.LevelTimer = this.levelTotalTime;
        this.DeathCount = this.KillCount = this.CrystalCount = 0;
    }

    ///////////////////////////////////////////////////////////////////////////////////////////////
    void Update()
    {
        if (!this.levelEnded)
        {
            if (Input.GetButtonDown("Pause"))
            {
                this.pauseCanvas.enabled = !this.pauseCanvas.enabled;
                Time.timeScale = (this.pauseCanvas.enabled) ? 0f : 1f;
            }

            if (!this.pauseCanvas.enabled)
            {
                if (Input.GetKeyDown(KeyCode.K) && this.player.gameObject.activeSelf)
                {
                    KillPlayer(false);
                }

                if (this.LevelTimer > 0f)
                {
                    this.LevelTimer -= Time.deltaTime;
                }
            }
        }
    }

    ///////////////////////////////////////////////////////////////////////////////////////////////
    public void KillPlayer(bool isFirstSpawn = false)
    {
        
        if (!isFirstSpawn)
        {
            this.deathCount++;
            this.deathCountText.SetText(this.deathCount.ToString());

            this.player.KillPlayer();
            StartCoroutine(WaitingForRespawn());
        }
        else
        {
            this.currentCheckpoint.ResetSection(this.player, isFirstSpawn);
        }
    }

    ///////////////////////////////////////////////////////////////////////////////////////////////
    public void InitGravityChargeUI()
    {
        this.bagelsAnimator.SetTrigger("Bagel" + this.player.GravityChargeMax);
        UpdateGravityChargeUI();
    }

    ///////////////////////////////////////////////////////////////////////////////////////////////
    public void UpdateGravityChargeUI()
    {
        for (uint i = 0; i < this.player.GravityChargeMax; ++i)
        {
            if (this.uiBagels[i] != null)
            {
                this.uiBagels[i].enabled = ((i + 1) > this.player.GravityChargeCount);
            }
        }
    }

    ///////////////////////////////////////////////////////////////////////////////////////////////
    public void EndLevel()
    {
        Time.timeScale = 0f;
        this.levelEnded = true;

        this.hudCanvas.enabled = false;
        this.pauseCanvas.enabled = false;
        this.endingCanvas.enabled = true;

        this.deathCountFinal.SetText(this.deathCount.ToString());
        this.killCountFinal.SetText(this.killCount.ToString());
        this.levelTimerFinal.SetText(this.levelTimer.ToString("F0"));
        this.crystalCountFinal.SetText(this.crystalCount.ToString());

        int deathScore = (int)this.deathCount * this.scorePerDeath;
        int killScore = (int)this.killCount * this.scorePerKill;
        int timeScore = (int)this.levelTimer * this.scorePerSeconds;
        int crystalScore = (int)this.crystalCount * this.scorePerCrystal;

        this.deathScoreText.SetText(deathScore.ToString());
        this.killScoreText.SetText(killScore.ToString());
        this.levelTimerScoreText.SetText(timeScore.ToString());
        this.crystalScoreText.SetText(crystalScore.ToString());

        int totalScore = deathScore + killScore + timeScore + crystalScore;
        this.scoreTotal.SetText(totalScore.ToString());

        this.approvedBagel.enabled = (totalScore >= this.approvalGoalScore);
        this.rejectedBagel.enabled = !this.approvedBagel.enabled;
        this.scoreApproval.SetText(totalScore.ToString());
        this.goalApproval.SetText(this.approvalGoalScore.ToString());

        StartCoroutine(WaitingForNextLevel());
    }

    ///////////////////////////////////////////////////////////////////////////////////////////////
    public void TeleportPlayer(Vector2 newPosition)
    {
        this.player.OnEnterTeleport(newPosition);
    }

    ///////////////////////////////////////////////////////////////////////////////////////////////
    public void RemoveBagel()
    {
        this.player.GravityChargeMax--;
    }

    ///////////////////////////////////////////////////////////////////////////////////////////////
    IEnumerator WaitingForRespawn()
    {
        yield return new WaitForSeconds(this.respawnTime);
        this.currentCheckpoint.ResetSection(this.player, false);
    }

    ///////////////////////////////////////////////////////////////////////////////////////////////
    IEnumerator WaitingForNextLevel()
    {
        while (!Input.GetButtonDown("Jump"))
        {
            yield return null;
        }

        if (LevelLoader.instance != null)
        {
            this.endingCanvas.enabled = false;
            SoundManager.instance.StopAllSounds();
            LevelLoader.instance.LoadNextLevel();
        }
    }
}
