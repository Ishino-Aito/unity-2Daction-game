using UnityEngine;

/// <summary>
/// 敵がプレイヤーに接触した時にダメージを与えるコンポーネント
/// </summary>
public class EnemyDamage : MonoBehaviour
{
    [SerializeField] private float damageAmount = 34f;    // プレイヤーに与えるダメージ量
    [SerializeField] private bool showDebugLog = true;    // デバッグログを表示するかどうか

    /// <summary>
    /// 他のコライダーと衝突した時に呼び出されるメソッド
    /// </summary>
    /// <param name="collision">衝突情報</param>
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // 衝突したオブジェクトがプレイヤーかどうかを確認
        if (collision.gameObject.CompareTag("Player"))
        {
            // プレイヤーの体力管理コンポーネントを取得
            PlayerHealth playerHealth = collision.gameObject.GetComponent<PlayerHealth>();

            // 体力管理コンポーネントが存在する場合はダメージを適用
            if (playerHealth != null)
            {
                // プレイヤーにダメージを与える
                playerHealth.ApplyDirectDamage(damageAmount);

                // デバッグログを表示する設定の場合はログを出力
                if (showDebugLog)
                {
                    Debug.Log($"{damageAmount}ダメージをプレイヤーに与えました");
                }
            }
            else
            {
                // プレイヤーにPlayerHealthコンポーネントがない場合は警告を表示
                Debug.LogWarning("プレイヤーオブジェクトにPlayerHealthコンポーネントが見つかりません");
            }
        }
    }
}