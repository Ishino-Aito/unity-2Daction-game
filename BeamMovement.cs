using UnityEngine;

/// <summary>
/// ビームの基本的な動作と性質を管理するコンポーネント
/// </summary>
public class BeamMovement : MonoBehaviour
{
    [Header("ビーム設定")]
    [Tooltip("ビームの移動速度")]
    [SerializeField] private float speed = 5f;

    [Tooltip("ビームが与えるダメージ量")]
    [SerializeField] private float damageAmount = 100f;

    [Tooltip("画面外判定の閾値（この値より下に行くとビームオブジェクトが破棄される）")]
    [SerializeField] private float destroyThreshold = -10f;

    /// <summary>
    /// 初期化処理
    /// </summary>
    private void Start()
    {
        // ビームにタグを設定
        gameObject.tag = "Beam";
    }

    /// <summary>
    /// 毎フレーム実行される処理
    /// </summary>
    private void Update()
    {
        // ビームを下方向に移動させる
        MoveBeam();

        // 画面外判定
        CheckIfOffScreen();
    }

    /// <summary>
    /// ビームを移動させる処理
    /// </summary>
    protected virtual void MoveBeam()
    {
        transform.Translate(Vector2.down * speed * Time.deltaTime);
    }

    /// <summary>
    /// 画面外に出たかどうかをチェックし、出ていれば破棄する
    /// </summary>
    private void CheckIfOffScreen()
    {
        if (transform.position.y < destroyThreshold)
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// ビームのダメージ量を取得する
    /// </summary>
    /// <returns>ダメージ量</returns>
    public virtual float GetDamageAmount()
    {
        return damageAmount;
    }
}