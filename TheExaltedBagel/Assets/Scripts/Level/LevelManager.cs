using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour
{
    public static LevelManager instance;

    [Header("Settings")]
    [SerializeField] private float deathScorePenalty = 1000f;
    [SerializeField] private float respawnTime = 1.5f;
    [SerializeField] private float levelTotalTime = 300;

    [Header("UI")]
    [SerializeField] private RawImage[] uiBagels = new RawImage[3];
    [SerializeField] private TextMeshProUGUI deathCountText;
    [SerializeField] private TextMeshProUGUI levelTimerText;
    [SerializeField] private TextMeshProUGUI killCountText;
    [SerializeField] private TextMeshProUGUI crystalCountText;

    private uint killCount = 0;
    private uint crystalCount = 0;
    private uint deathCount = 0;
    private float levelTimer = 0;
    private float score = 0f;
    private Player player;
    private Checkpoint currentCheckpoint;

    ///////////////////////////////////////////////////////////////////////////////////////////////
    public Checkpoint CurrentCheckpoint
    {
        set { this.currentCheckpoint = value; }
    }

    ///////////////////////////////////////////////////////////////////////////////////////////////
    public float Score
    {
        get { return this.score; }
        set { this.score = value; }
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
        this.levelTimer = this.levelTotalTime;
        this.deathCount = this.killCount = this.crystalCount = 0;

        this.deathCountText.SetText(this.deathCount.ToString());
        this.killCountText.SetText(this.killCount.ToString());
        this.crystalCountText.SetText(this.crystalCount.ToString());
        this.levelTimerText.SetText(this.levelTimer.ToString("F0"));
    }

    ///////////////////////////////////////////////////////////////////////////////////////////////
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.K) && this.player.gameObject.activeSelf)
        {
            KillPlayer(false);
        }

        if (this.levelTimer > 0f)
        {
            this.levelTimer -= Time.deltaTime;
            if (this.levelTimer <= 0f)
            {
                this.levelTimer = 0f;
                this.levelTimerText.SetText("0");
            }
            else if (Mathf.FloorToInt(this.levelTimer) != Mathf.FloorToInt(this.levelTimer + Time.deltaTime))
            {
                this.levelTimerText.SetText(levelTimer.ToString("F0"));
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

            this.score -= this.deathScorePenalty;
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
    public void CrystalPicked()
    {
        this.crystalCount++;
        this.crystalCountText.SetText(this.crystalCount.ToString());
    }

    ///////////////////////////////////////////////////////////////////////////////////////////////
    public void EnemyKilled()
    {
        this.killCount++;
        this.killCountText.SetText(this.killCount.ToString());
    }

    ///////////////////////////////////////////////////////////////////////////////////////////////
    IEnumerator WaitingForRespawn()
    {
        yield return new WaitForSeconds(this.respawnTime);
        this.currentCheckpoint.ResetSection(this.player, false);
    }
}
