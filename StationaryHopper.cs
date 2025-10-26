using System.Collections;
using UnityEngine;

/// <summary>
/// その場で定期的にジャンプを繰り返す敵のクラス（PlayerControllerの接地判定を移植）
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
public class StationaryHopper : MonoBehaviour
{
    [Header("ジャンプ設定")]
    [SerializeField] [Tooltip("ジャンプの強さ")] private float jumpForce = 7.0f;
    [SerializeField] [Tooltip("ジャンプする頻度（秒）")] private float jumpInterval = 1.5f;

    // 内部変数
    private Rigidbody2D rb;
    private bool isGrounded = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            Debug.LogError("StationaryHopper: Rigidbody2Dが見つかりません！");
            enabled = false;
            return;
        }

        // X軸（水平）方向の移動と回転を完全に固定
        rb.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezeRotation;

        // ジャンプ処理のコルーチンを開始
        StartCoroutine(HopRoutine());
    }

    /// <summary>
    /// 定期的にジャンプを実行するコルーチン
    /// </summary>
    private IEnumerator HopRoutine()
    {
        while (true)
        {
            // 指定した間隔だけ待機
            yield return new WaitForSeconds(jumpInterval);

            // もし地面にいたらジャンプする
            if (isGrounded)
            {
                rb.linearVelocity = new Vector2(0, rb.linearVelocity.y); // ジャンプ前に慣性をリセット
                rb.AddForce(new Vector2(0, jumpForce), ForceMode2D.Impulse);
            }
        }
    }

    /// <summary>
    /// 地面と接触している間、継続的に呼び出される
    /// </summary>
    void OnCollisionStay2D(Collision2D collision)
    {
        isGrounded = false;
        foreach (ContactPoint2D contact in collision.contacts)
        {
            // 接点が下方向なら地面と判定
            if (contact.normal.y > 0.5f)
            {
                isGrounded = true;
                break;
            }
        }
    }

    /// <summary>
    /// 地面から離れた瞬間に呼び出される
    /// </summary>
    void OnCollisionExit2D(Collision2D collision)
    {
        isGrounded = false;
    }
}