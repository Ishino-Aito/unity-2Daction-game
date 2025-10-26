using UnityEngine;

/// <summary>
/// 敵キャラクターを左右に移動させ、壁で反転し、プレイヤーを検知して追跡するクラス
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
public class EnemyMovement : MonoBehaviour
{
    [Header("移動設定")]
    [SerializeField]
    [Tooltip("敵の移動速度")]
    private float moveSpeed = 2.0f;

    [Header("索敵設定")]
    [SerializeField]
    [Tooltip("プレイヤーを検知する範囲")]
    private float detectionRadius = 5.0f;

    // 内部変数
    private Rigidbody2D rb;
    private int direction = -1; // 1: 右, -1: 左
    private bool isPlayerDetected = false;
    private const float FALL_DEATH_Y = -20f;
    private string playerTag = "Player";
    private Transform playerTransform; // プレイヤーのTransformをキャッシュする

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            Debug.LogError("EnemyMovement: Rigidbody2Dが見つかりません！");
            enabled = false;
            return;
        }

        if (rb.bodyType == RigidbodyType2D.Static)
        {
            Debug.LogError("EnemyMovement: Rigidbody2DのBody TypeがStaticです。Dynamicに変更してください。");
            enabled = false;
            return;
        }

        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        UpdateFacingDirection();
        Debug.Log("EnemyMovement: 初期化完了。");
    }

    void Update()
    {
        // プレイヤー検知
        CheckForPlayer();

        // 落下死の判定
        if (transform.position.y < FALL_DEATH_Y)
        {
            Debug.Log("EnemyMovement: 落下したためオブジェクトを破棄します。");
            Destroy(gameObject);
        }
    }

    void FixedUpdate()
    {
        // プレイヤーを検知していない場合は、水平方向の動きを止める
        if (!isPlayerDetected)
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            return;
        }

        // プレイヤーを検知している場合のみ移動
        if (moveSpeed > 0)
        {
            rb.linearVelocity = new Vector2(direction * moveSpeed, rb.linearVelocity.y);
        }
    }

    /// <summary>
    /// 周囲にプレイヤーがいるか確認する
    /// </summary>
    private void CheckForPlayer()
    {
        // プレイヤーのTransformがキャッシュされていなければ探す
        if (playerTransform == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag(playerTag);
            if (player != null)
            {
                playerTransform = player.transform;
            }
        }

        // プレイヤーが見つからない場合は検知しない
        if (playerTransform == null)
        {
            isPlayerDetected = false;
            return;
        }

        // プレイヤーとの距離を計算
        float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);

        // 設定した範囲内にプレイヤーがいるかを判定
        isPlayerDetected = distanceToPlayer <= detectionRadius;
    }

    /// <summary>
    /// 衝突を検知したときに呼び出される
    /// </summary>
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // プレイヤーを検知していない場合は反転しない
        if (!isPlayerDetected) return;

        Debug.Log("EnemyMovement: " + collision.gameObject.name + " と衝突しました。");
        // 衝突した点の法線ベクトルをチェック
        foreach (ContactPoint2D contact in collision.contacts)
        {
            // 水平方向の衝突（壁との衝突）か判定
            if (Mathf.Abs(contact.normal.x) > 0.5f)
            {
                direction *= -1;
                UpdateFacingDirection();
                Debug.Log("EnemyMovement: 壁に衝突。方向を反転します。新しい方向: " + direction);
                break;
            }
        }
    }

    /// <summary>
    /// 移動方向に応じてキャラクターの向きを更新する
    /// </summary>
    private void UpdateFacingDirection()
    {
        transform.localScale = new Vector3(direction, 1, 1);
    }

    /// <summary>
    /// Unityエディタ上で索敵範囲を視覚的に表示する
    /// </summary>
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}