using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.PresetComponents.Card {

	/// <summary>
	/// カードの基底クラスです。
	/// カードの効果とIDを振るためには、このクラスを継承して下さい。
	/// </summary>
	public class Card : MonoBehaviour {

		/// <summary>
		/// カードが表示されてから移動が完了するまでの時間秒数
		/// </summary>
		public const float StartMoveSec = 5.0f;

		/// <summary>
		/// カードが半回転する時間秒数
		/// </summary>
		public const float TurnHalfSec = 0.5f;

		/// <summary>
		/// カードが最初に表示される位置
		/// </summary>
		public static readonly Vector2 StartPosition = Vector2.zero;

		/// <summary>
		/// カード裏面のスプライト（初期状態）
		/// </summary>
		[SerializeField]
		private Sprite backSprite;

		/// <summary>
		/// カード表面のスプライト
		/// </summary>
		[SerializeField]
		private Sprite frontSprite;

		/// <summary>
		/// カードが表示されてから移動する先の座標
		/// </summary>
		public Vector2 destPosition;

		/// <summary>
		/// デフォルトで表向きにするかどうか
		/// </summary>
		[SerializeField]
		private bool defaultOpenMode = false;

		/// <summary>
		/// オープンしているかどうか
		/// </summary>
		[SerializeField]
		private bool _isOpened = false;

		/// <summary>
		/// このカードのIDを返します。
		/// </summary>
		/// <returns></returns>
		public virtual int GetCardID() {
			return -1;
		}

		/// <summary>
		/// カードの効果を発動させます。
		/// </summary>
		public virtual void DoCard() {
		}

		/// <summary>
		/// 初期状態で表面から裏面に切り替えます。
		/// </summary>
		private void Awake() {
			this._isOpened = this.defaultOpenMode;
			if(this.defaultOpenMode) {
				return;
			}

			// 表面のスプライトを保管して切り替える
			this.frontSprite = this.GetComponent<SpriteRenderer>().sprite;
			this.GetComponent<SpriteRenderer>().sprite = this.backSprite;
		}

		/// <summary>
		/// 配置時に指定された座標へ移動させます。
		/// </summary>
		private void Start() {
			iTween.MoveTo(
				this.gameObject,
				iTween.Hash(
					"x", this.destPosition.x,
					"y", this.destPosition.y,
					"time", Card.StartMoveSec,
					"oncomplete", new Action<object>((arg) => {
						this.Open(new Action(() => {
							this.Close();
						}));
					})
				)
			);
		}

		/// <summary>
		/// 表にめくります。
		/// </summary>
		/// <param name="callback">完了後に呼ばれるコールバック</param>
		public void Open(Action callback = null) {
			this._isOpened = true;

			// 裏が完全に隠れたときにスプライトを差し替えて回転継続する
			var backClosed = new Action<object>((arg) => {
				this.GetComponent<SpriteRenderer>().sprite = this.frontSprite;

				this.transform.eulerAngles = new Vector3(0, 270f, 0);
				iTween.RotateTo(
					this.gameObject,
					iTween.Hash(
						"x", 0,
						"y", 360f,
						"z", 0,
						"time", Card.TurnHalfSec,
						"easeType", iTween.EaseType.easeOutCubic,
						"oncomplete", new Action<object>((arg2) => {
							this.transform.eulerAngles = new Vector3(0, 0, 0);
							callback?.Invoke();
						})
					)
				);
			});

			// 回転開始
			iTween.RotateTo(
				this.gameObject,
				iTween.Hash(
					"x", 0,
					"y", 90f,
					"z", 0,
					"time", Card.TurnHalfSec,
					"easeType", iTween.EaseType.easeOutCubic,
					"oncomplete", backClosed
				)
			);
		}

		/// <summary>
		/// 裏にめくります。
		/// </summary>
		/// <param name="callback">完了後に呼ばれるコールバック</param>
		public void Close(Action callback = null) {
			this._isOpened = false;

			// 表が完全に隠れたときにスプライトを差し替えて回転継続する
			var frontClosed = new Action<object>((arg) => {
				this.GetComponent<SpriteRenderer>().sprite = this.backSprite;

				this.transform.eulerAngles = new Vector3(0, 270f, 0);
				iTween.RotateTo(
					this.gameObject,
					iTween.Hash(
						"x", 0,
						"y", 360f,
						"z", 0,
						"time", Card.TurnHalfSec,
						"easeType", iTween.EaseType.easeOutCubic,
						"oncomplete", new Action<object>((arg2) => {
							this.transform.eulerAngles = new Vector3(0, 0, 0);
							callback?.Invoke();
						})
					)
				);
			});

			// 回転開始
			iTween.RotateTo(
				this.gameObject,
				iTween.Hash(
					"x", 0,
					"y", 90f,
					"z", 0,
					"time", Card.TurnHalfSec,
					"easeType", iTween.EaseType.easeOutCubic,
					"oncomplete", frontClosed
				)
			);
		}

		/// <summary>
		/// このカードが表になっているかどうかを返します。
		/// </summary>
		/// <returns>表になっているかどうか</returns>
		public bool IsOpened() {
			return this._isOpened;
		}

	}
}
