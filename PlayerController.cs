using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using static System.Net.Mime.MediaTypeNames;
using System.Diagnostics;

/// <summary>
/// プレイヤーキャラクターの動作を制御するコンポーネント
/// 移動、ジャンプ、壁スライド、残機管理などの機能を持つ
/// </summary>
public class PlayerController : MonoBehaviour
{
    // 物理演算用コンポーネント
    private Rigidbody2D rigid2d;

    // 基本移動パラメータ
    [Header("基本設定")]
    [SerializeField] private float jumpForce = 500.0f;      // ジャンプ力
    [SerializeField] private float moveSpeed = 5.0f;        // 移動速度
    [SerializeField] private float wallSlideSpeed = 2f;     // 壁スライド時の落下速度
    [SerializeField] private int defaultLives = 3;          // デフォルトの残機数

    // アニメーション制御用
    private Animator animator;

    // UIテキスト（TMProと通常Textの両方をサポート）
    [Header("UI設定")]
    [SerializeField] private TextMeshProUGUI livesTextTMP;     // TextMeshPro形式の残機表示
    [SerializeField] private Text livesTextLegacy;             // レガシーText形式の残機表示
    [SerializeField] private string gameOverSceneName = "GameOverScene";  // ゲームオーバー時に読み込むシーン名

    // モバイル操作用のボタン
    [Header("モバイル操作")]
    [SerializeField] private Button rightButton;    // 右移動ボタン
    [SerializeField] private Button leftButton;     // 左移動ボタン
    [SerializeField] private Button jumpButton;     // ジャンプボタン

    //音響設定
    [Header("音響設定")]
    [SerializeField] private AudioSource audioSource;          // AudioSourceコンポーネント
    [SerializeField] private AudioClip jumpSE;                 // ジャンプ効果音
    [SerializeField][Range(0f, 1f)] private float jumpVolume = 0.5f;  // ジャンプ音の音量

    // ジャンプ制御用の変数
    [Header("ジャンプ制御")]
    [SerializeField] private float minJumpForce = 300.0f;       // 最小ジャンプ力
    [SerializeField] private float maxJumpForce = 500.0f;       // 最大ジャンプ力
    [SerializeField] private float jumpChargeTime = 0.3f;       // 最大ジャンプ力に達するまでの時間

    // ジャンプ状態管理用の変数
    private bool isJumping = false;                             // ジャンプ中フラグ
    private float jumpChargeTimer = 0f;                         // ジャンプチャージタイマー
    private bool jumpButtonDown = false;                        // ジャンプボタンが押されたフラグ

    // 地形との接触状態
    private bool isTouchingWall = false;    // 壁と接触しているか
    private bool isGrounded = false;        // 地面に接触しているか
    private bool isWallSliding = false;     // 壁をスライドダウン中か

    // 移動状態
    private float moveInput = 0; // キー入力（-1, 0, 1）

    // ヘルメット装備管理用
    private PlayerHealth playerHealth;

    // モバイル入力状態
    private bool isRightButtonPressed = false;  // 右ボタンが押されているか
    private bool isLeftButtonPressed = false;   // 左ボタンが押されているか
    private bool isJumpButtonPressed = false;   // ジャンプボタンが押されているか
    private bool wasJumpButtonPressedLastFrame = false;  // 前フレームでジャンプボタンが押されていたか

    // 残機とゲーム状態の管理
    public static int currentLives = -1;    // 現在の残機数（シーン間で共有）
    private bool isRestarting = false;      // リスタート処理中フラグ
    public static bool gameStarted = false; // ゲーム開始フラグ

    void Start()
    {
        // 物理演算コンポーネントの初期化
        rigid2d = GetComponent<Rigidbody2D>();
        rigid2d.freezeRotation = true;  // 回転を固定

        // アニメーターコンポーネントの取得
        animator = GetComponent<Animator>();
        if (animator == null)
        {
            Debug.LogWarning("アニメーターコンポーネントが見つかりません。");
        }

        // PlayerHealthコンポーネントの取得
        playerHealth = GetComponent<PlayerHealth>();
        if (playerHealth == null)
        {
            Debug.LogWarning("PlayerHealthコンポーネントが見つかりません。自動的に追加します。");
            playerHealth = gameObject.AddComponent<PlayerHealth>();
        }

        // 音響システムの初期化
        InitializeAudioSystem();

        // 残機の初期化
        InitializeLives();

        // 残機表示の更新
        UpdateLivesUI();

        // モバイル操作の初期化
        SetupMobileControls();
    }

    void Update()
    {
        // ポーズ中は、これ以降の処理をすべて行わないようにする
        if (PauseSystem.IsGamePaused)
        {
            return; // 処理をここで中断
        }

        // キー入力の受付
        HandleInput();

        // ジャンプ処理
        HandleJumping();

        // その他、フレームごとの更新処理
        UpdateFacingDirection();
        CheckForFallOutOfScreen();
        KeepPlayerInsideScreen();
        UpdateAnimation();
    }

    void FixedUpdate()
    {
        // ポーズ中は物理的な動きを一切行わないようにする
        if (PauseSystem.IsGamePaused)
        {
            rigid2d.linearVelocity = Vector2.zero; // 物理的な動きを停止
            return;
        }

        // 壁スライド状態の判定と処理
        HandleWallSliding();

        // 移動処理
        HandleMovement();
    }

    private void InitializeAudioSystem()
    {
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
            {
                audioSource = gameObject.AddComponent<AudioSource>();
                Debug.Log("AudioSourceコンポーネントを自動追加しました。");
            }
        }
        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 0f;
    }

    public void PlaySE(AudioClip clip, float volume = 1.0f)
    {
        if (clip != null && audioSource != null)
        {
            audioSource.PlayOneShot(clip, volume);
        }
        else
        {
            if (clip == null) Debug.LogWarning("再生しようとした効果音が設定されていません。");
            if (audioSource == null) Debug.LogWarning("AudioSourceが見つかりません。");
        }
    }

    private void PlayJumpSE()
    {
        PlaySE(jumpSE, jumpVolume);
    }

    private void InitializeLives()
    {
        if (!gameStarted)
        {
            currentLives = defaultLives;
            gameStarted = true;
            Debug.Log("ゲーム開始: 残機を初期化 = " + currentLives);
        }
        else if (currentLives < 0)
        {
            currentLives = defaultLives;
            Debug.Log("残機が不正な値のため初期化: " + currentLives);
        }
    }

    private void SetupMobileControls()
    {
        SetupMobileButton(rightButton, "右ボタン",
            onDown: () => isRightButtonPressed = true,
            onUp: () => isRightButtonPressed = false);

        SetupMobileButton(leftButton, "左ボタン",
            onDown: () => isLeftButtonPressed = true,
            onUp: () => isLeftButtonPressed = false);

        SetupMobileButton(jumpButton, "ジャンプボタン",
            onDown: () => isJumpButtonPressed = true,
            onUp: () => isJumpButtonPressed = false);
    }

    private void AddEventTriggerListener(EventTrigger trigger, EventTriggerType eventType, UnityEngine.Events.UnityAction<BaseEventData> action)
    {
        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = eventType;
        entry.callback.AddListener((data) => { action((BaseEventData)data); });
        trigger.triggers.Add(entry);
    }

    private void SetupMobileButton(Button button, string buttonName, System.Action onDown, System.Action onUp)
    {
        if (button == null)
        {
            Debug.LogWarning($"{buttonName}が設定されていません");
            return;
        }
        EventTrigger trigger = button.gameObject.GetComponent<EventTrigger>() ?? button.gameObject.AddComponent<EventTrigger>();
        AddEventTriggerListener(trigger, EventTriggerType.PointerDown, (data) => onDown());
        AddEventTriggerListener(trigger, EventTriggerType.PointerUp, (data) => onUp());
        AddEventTriggerListener(trigger, EventTriggerType.PointerExit, (data) => onUp());
    }

    private void UpdateLivesUI()
    {
        string livesString = "x " + currentLives.ToString();
        if (livesTextTMP != null)
        {
            livesTextTMP.text = livesString;
        }
        else if (livesTextLegacy != null)
        {
            livesTextLegacy.text = livesString;
        }
        else
        {
            Debug.LogWarning("残機表示用のUIテキストが設定されていません。");
        }
    }

    private void HandleInput()
    {
        moveInput = 0;
        if (Input.GetKey(KeyCode.RightArrow) || isRightButtonPressed) moveInput = 1;
        if (Input.GetKey(KeyCode.LeftArrow) || isLeftButtonPressed) moveInput = -1;
    }

    private void HandleMovement()
    {
        // isWallSlidingがtrueの場合、速度はHandleWallSlidingで直接制御される
        if (!isWallSliding)
        {
            rigid2d.linearVelocity = new Vector2(moveInput * moveSpeed, rigid2d.linearVelocity.y);
        }
    }

    private void HandleWallSliding()
    {
        // 壁に向かってキー入力がされているかを判定
        bool isPushingWall = (transform.localScale.x > 0 && moveInput > 0) || (transform.localScale.x < 0 && moveInput < 0);

        // 壁スライドの条件：壁に触れていて、地面に居なくて、壁に向かって入力がある
        if (isTouchingWall && !isGrounded && isPushingWall)
        {
            isWallSliding = true;

            // 壁スライド中の速度を制御
            // 上昇中(velocity.y > 0)は、y速度はそのままに、x速度のみ0にする
            // 落下中(velocity.y <= 0)は、y速度を-wallSlideSpeedに、x速度を0にする
            float yVelocity = rigid2d.linearVelocity.y > 0 ? rigid2d.linearVelocity.y : -wallSlideSpeed;
            rigid2d.linearVelocity = new Vector2(0, yVelocity);
        }
        else
        {
            isWallSliding = false;
        }
    }

    private void HandleJumping()
    {
        bool jumpInputDown = Input.GetKeyDown(KeyCode.Space) || (isJumpButtonPressed && !wasJumpButtonPressedLastFrame);
        bool jumpInputHeld = Input.GetKey(KeyCode.Space) || isJumpButtonPressed;
        bool jumpInputUp = Input.GetKeyUp(KeyCode.Space) || (!isJumpButtonPressed && wasJumpButtonPressedLastFrame);

        if (jumpInputDown && isGrounded)
        {
            jumpButtonDown = true;
            jumpChargeTimer = 0f;
            isJumping = true;
        }

        if (jumpButtonDown && jumpInputHeld && isJumping)
        {
            jumpChargeTimer += Time.deltaTime;
            jumpChargeTimer = Mathf.Clamp(jumpChargeTimer, 0f, jumpChargeTime);
        }

        // ジャンプボタンが離された、または押され続けていない、かつボタンが押された状態だった場合
        if (jumpButtonDown && (jumpInputUp || !jumpInputHeld))
        {
            // isJumpingフラグがtrue（＝地面でジャンプが開始された）の場合のみジャンプを実行
            if (isJumping)
            {
                float jumpPower = Mathf.Lerp(minJumpForce, maxJumpForce, jumpChargeTimer / jumpChargeTime);
                rigid2d.AddForce(Vector2.up * jumpPower);
                PlayJumpSE();
            }
            jumpButtonDown = false;
            jumpChargeTimer = 0f;
            isJumping = false;
        }
        wasJumpButtonPressedLastFrame = isJumpButtonPressed;
    }

    private void UpdateFacingDirection()
    {
        // 壁スライド中、または空中で壁に触れている間は向きを変えない
        // 地面にいれば壁際でも向きを変えられる
        bool canChangeDirection = !isWallSliding && (!isTouchingWall || isGrounded);

        if (moveInput != 0 && canChangeDirection)
        {
            transform.localScale = new Vector3(moveInput, 1, 1);
        }
    }

    private void CheckForFallOutOfScreen()
    {
        if (transform.position.y < -10 && !isRestarting)
        {
            LoseLife();
        }
    }

    private void UpdateAnimation()
    {
        if (animator != null)
        {
            bool isMoving = moveInput != 0;
            animator.SetBool("IsMoving", isMoving && isGrounded);
            animator.SetBool("IsJumping", !isGrounded);
            animator.SetBool("IsWallSliding", isWallSliding);
            if (playerHealth != null)
            {
                bool hasHelmet = playerHealth.HasHelmet();
                animator.SetBool("HasHelmet", hasHelmet);
            }
        }
    }

    public void LoseLife()
    {
        if (isRestarting) return;
        isRestarting = true;
        currentLives--;
        Debug.Log("残機が減りました: " + currentLives);
        if (currentLives <= 0)
        {
            Debug.Log("ゲームオーバー");
            LoadGameOverScene();
        }
        else
        {
            RestartCurrentScene();
        }
    }

    public void LoadGameOverScene()
    {
        currentLives = defaultLives;
        gameStarted = false;
        Debug.Log("ゲームオーバーシーン読み込み: " + gameOverSceneName);
        if (DoesSceneExist(gameOverSceneName))
        {
            SceneManager.LoadScene(gameOverSceneName);
        }
        else
        {
            Debug.LogError("シーン '" + gameOverSceneName + "' がビルド設定に見つかりません。現在のシーンを再起動します。");
            RestartCurrentScene();
        }
    }

    private bool DoesSceneExist(string sceneName)
    {
        for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
        {
            string scenePath = SceneUtility.GetScenePathByBuildIndex(i);
            string sceneNameFromBuild = System.IO.Path.GetFileNameWithoutExtension(scenePath);
            if (sceneNameFromBuild == sceneName)
            {
                return true;
            }
        }
        return false;
    }

    public void RestartCurrentScene()
    {
        string currentSceneName = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(currentSceneName);
        Debug.Log("シーンを再起動: " + currentSceneName);
    }

    void KeepPlayerInsideScreen()
    {
        if (Camera.main == null)
        {
            Debug.LogWarning("メインカメラが見つかりません。プレイヤーを画面内に保てません。");
            return;
        }
        Vector3 screenPosition = Camera.main.WorldToViewportPoint(transform.position);
        if (screenPosition.x < 0) screenPosition.x = 0;
        if (screenPosition.x > 1) screenPosition.x = 1;
        if (screenPosition.y > 1) screenPosition.y = 1;
        transform.position = Camera.main.ViewportToWorldPoint(screenPosition);
    }

    public void LoadNextScene()
    {
        int nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;
        if (nextSceneIndex < SceneManager.sceneCountInBuildSettings)
        {
            SceneManager.LoadScene(nextSceneIndex);
        }
        else
        {
            Debug.Log("これ以上のステージはありません");
        }
    }

    private bool IsGrounded()
    {
        return Mathf.Abs(rigid2d.linearVelocity.y) < 0.1f;
    }

    void OnCollisionStay2D(Collision2D collision)
    {
        isGrounded = false;
        isTouchingWall = false;
        foreach (ContactPoint2D contact in collision.contacts)
        {
            // 接点が下方向なら地面と判定
            if (contact.normal.y > 0.5f)
            {
                isGrounded = true;
                isTouchingWall = false; // 地面にいるなら壁にはいない
                break; // 地面にいることが確定したら、壁の判定はしない
            }
            // 接点が横方向なら壁と判定
            else if (Mathf.Abs(contact.normal.x) > 0.5f)
            {
                isTouchingWall = true;
            }
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        isGrounded = false;
        isTouchingWall = false;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Helmet") && playerHealth != null)
        {
            playerHealth.EquipHelmet();
            Destroy(other.gameObject);
        }
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        isRestarting = false;
        Debug.Log("シーン「" + scene.name + "」がロードされました。現在の残機: " + currentLives);
    }

    #region MobileInputMethods
    public void OnRightButtonDown()
    {
        isRightButtonPressed = true;
    }

    public void OnRightButtonUp()
    {
        isRightButtonPressed = false;
    }

    public void OnLeftButtonDown()
    {
        isLeftButtonPressed = true;
    }

    public void OnLeftButtonUp()
    {
        isLeftButtonPressed = false;
    }

    public void OnJumpButtonDown()
    {
        isJumpButtonPressed = true;
    }

    public void OnJumpButtonUp()
    {
        isJumpButtonPressed = false;
    }
    #endregion
}