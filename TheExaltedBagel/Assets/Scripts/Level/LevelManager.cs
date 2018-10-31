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

    [Header("UI")]
    [SerializeField] private RawImage[] uiBagels = new RawImage[3];
    [SerializeField] private TextMeshProUGUI deathCountText;
    [SerializeField] private TextMeshProUGUI levelTimerText;
    [SerializeField] private TextMeshProUGUI killCountText;
    [SerializeField] private TextMeshProUGUI crystalCountText;
    [SerializeField] private Canvas pauseCanvas;

    private float gameTimeScale;
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
        instance = this;
        this.player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
    }

    ///////////////////////////////////////////////////////////////////////////////////////////////
    void Start()
    {
        this.LevelTimer = this.levelTotalTime;
        this.DeathCount = this.KillCount = this.CrystalCount = 0;

        this.gameTimeScale = Time.timeScale;
    }

    ///////////////////////////////////////////////////////////////////////////////////////////////
    void Update()
    {
        if (Input.GetButtonDown("Pause"))
        {
            this.pauseCanvas.enabled = !this.pauseCanvas.enabled;
            Time.timeScale = (this.pauseCanvas.enabled) ? 0f : this.gameTimeScale;
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

    ///////////////////////////////////////////////////////////////////////////////////////////////
    public void KillPlayer(bool isFirstSpawn)
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
    public void UpdateGravityChargeUI(uint chargeCount, uint chargeMax)
    {
        for (uint i = 0; i < chargeMax; ++i)
        {
            if (this.uiBagels[i] != null)
            {
                this.uiBagels[i].enabled = ((i + 1) > chargeCount);
            }
        }
    }

    ///////////////////////////////////////////////////////////////////////////////////////////////
    IEnumerator WaitingForRespawn()
    {
        yield return new WaitForSeconds(this.respawnTime);
        this.currentCheckpoint.ResetSection(this.player, false);
    }
}
