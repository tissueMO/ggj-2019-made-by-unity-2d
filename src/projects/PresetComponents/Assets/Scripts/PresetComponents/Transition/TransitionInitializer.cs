using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// デフォルトの状態だとシーン遷移直後 1F のみ画面が見えてしまうため、
/// シーンに入った直後に制御してトランジションでフェードインするようにします。
/// </summary>
public class TransitionInitializer : MonoBehaviour {

	/// <summary>
	/// フェード時間
	/// </summary>
	public const float FadeSec = 1.0f;

	/// <summary>
	/// フェードを行うプレハブ
	/// </summary>
	[SerializeField]
	private Fade fadeCanvas;

	/// <summary>
	/// シーン遷移直後に画面を隠すためのオブジェクト
	/// </summary>
	[SerializeField]
	private GameObject blackout;

	/// <summary>
	/// 初期状態でフェードアウトされた状態にします。
	/// </summary>
	private void Awake() {
		this.blackout.SetActive(true);
	}

	/// <summary>
	/// シーン開始直後にフェードインします。
	/// </summary>
	private void Start() {
		// 先にフェードで隠しきってから開始する
		this.fadeCanvas.FadeIn(0, () => {
			this.blackout.SetActive(false);
			this.fadeCanvas.FadeOut(TransitionInitializer.FadeSec);
		});
	}

}
