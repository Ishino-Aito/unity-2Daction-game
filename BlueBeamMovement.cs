using UnityEngine;

/// <summary>
/// 青いビーム特有の動作と性質を管理するコンポーネント
/// BeamMovementクラスを継承して使用
/// </summary>
public class BlueBeamMovement : BeamMovement
{
    /// <summary>
    /// 初期化処理
    /// </summary>
    private void Start()
    {
        // 青いビーム用のタグを設定
        gameObject.tag = "Blue Beam";
    }

}