using UnityEngine;

/// <summary>
/// プレイヤーを追跡するカメラの制御を行うコンポーネント
/// </summary>
public class CameraController : MonoBehaviour
{
    [SerializeField] private GameObject player;       // 追跡するプレイヤーオブジェクト
    [SerializeField] private float minY = 0f;         // カメラのY座標の最小値（これより下にはカメラが移動しない）

    /// <summary>
    /// 初期化処理
    /// </summary>
    private void Start()
    {
        // プレイヤーオブジェクトが設定されていない場合は名前で検索
        if (player == null)
        {
            player = GameObject.Find("Player");

            // プレイヤーが見つからない場合はログを出力
            if (player == null)
            {
                Debug.LogWarning("プレイヤーオブジェクトが見つかりませんでした。カメラの追跡対象を設定してください。");
            }
        }
    }

    /// <summary>
    /// 毎フレーム実行される更新処理
    /// </summary>
    private void Update()
    {
        // プレイヤーが存在しない場合は何もしない
        if (player == null) return;

        // プレイヤーの現在位置を取得
        Vector3 playerPos = player.transform.position;

        // カメラの現在位置を取得
        Vector3 currentPosition = transform.position;

        // Y座標が最小値より下にならないように制限
        float newY = Mathf.Max(playerPos.y, minY);

        // カメラの位置を更新（X,Z座標は変更せず、Y座標のみ更新）
        transform.position = new Vector3(
            currentPosition.x,
            newY,
            currentPosition.z
        );
    }
}