using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Scripts.PresetComponents.Roguelike;
using Assets.Scripts.PresetComponents.Roguelike.SectionDivision;
using UnityEngine;

namespace Assets.Scripts.Roguelike {

	/// <summary>
	/// ゲーム制御クラス
	/// </summary>
	public class GameMaster : MonoBehaviour {

		/// <summary>
		/// プレイヤー１ターンあたりの使用可能な時間秒数
		/// </summary>
		public const float TurnSecLimit = 3f;

		/// <summary>
		/// マップ、プレイヤー、エネミー、アイテム、ギミック情報
		/// </summary>
		public MapManager mapManager;

		/// <summary>
		/// プレイヤーのターンであるかどうか
		/// </summary>
		private bool isPlayerTurn;

		/// <summary>
		/// ターン変わったかどうか
		/// </summary>
		private bool turnChanged;

		private float turnStartTime;

		/// <summary>
		/// プレイヤー１のオブジェクト
		/// </summary>
		public Player1 Player1Instance {
			get {
				return this.mapManager.player1.GetComponent<Player1>();
			}
		}

		/// <summary>
		/// エネミーのオブジェクトリスト
		/// </summary>
		public List<GameObject> Enemies {
			get {
				return ((MapDataBySectionDivision)this.mapManager.map).Enemies;
			}
		}

		/// <summary>
		/// 初期配置
		/// </summary>
		private void Start() {
			// 最初はプレイヤーのターン
			this.isPlayerTurn = true;
		}

		/// <summary>
		/// 毎フレームでゲーム全体を制御
		/// </summary>
		private void Update() {
			if(this.mapManager.map == null || ((MapDataBySectionDivision)this.mapManager.map).Enemies == null) {
				return;
			}

			if(this.isPlayerTurn) {
				// プレイヤーの入力＆移動を有効化
				if(!this.turnChanged) {
					this.turnChanged = true;

					// プレイヤーのターンに切り替え
					this.Player1Instance.enabled = true;
					this.Player1Instance.m_IsMove = true;

					// 敵の行動を無効化
					foreach(var enemy in this.Enemies) {
						enemy.GetComponent<Enemy>().enabled = false;
						enemy.GetComponent<Enemy>().m_IsMove = false;
						enemy.GetComponent<Enemy>().m_InputReturnWaitFlag = false;
					}
					this.turnChanged = true;
					Debug.Log("プレイヤーのターン");
				} else {
					// 終了判定
					if(!this.Player1Instance.enabled) {
						this.turnChanged = false;
						this.isPlayerTurn = false;
					}
				}
			} else {
				if(!this.turnChanged) {
					this.turnChanged = true;
					this.turnStartTime = Time.time;

					// エネミーのターンに切り替え
					this.Player1Instance.enabled = false;
					this.Player1Instance.m_IsMove = false;

					// 敵の行動を有効化
					foreach(var enemy in this.Enemies) {
						enemy.GetComponent<Enemy>().enabled = true;
						enemy.GetComponent<Enemy>().StartTurn();
					}
					Debug.Log("エネミーのターン");
				} else {
					// 終了判定
					bool ng = false;
					foreach(var enemy in this.Enemies) {
						if(enemy.GetComponent<Enemy>().enabled == true) {
							ng = true;
							break;
						}
					}

					// 強制終了タイマー
					if(Time.time - this.turnStartTime >= 2.0f) {
						ng = false;
					}

					if(!ng) {
						// 終了判定
						this.turnChanged = false;
						this.isPlayerTurn = true;
					}
				}
			}
		}

	}

}
