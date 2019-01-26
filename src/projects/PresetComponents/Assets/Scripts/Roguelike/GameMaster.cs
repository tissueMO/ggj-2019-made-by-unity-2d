using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Scripts.PresetComponents.Roguelike;
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
		private MapManager mapManager;

		/// <summary>
		/// プレイヤーのターンであるかどうか
		/// </summary>
		private bool isPlayerTurn;

		/// <summary>
		/// プレイヤー１のオブジェクト
		/// </summary>
		public Player1 Player1Instance {
			get {
				return this.mapManager.player1.GetComponent<Player1>();
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
			if(this.isPlayerTurn) {
				// プレイヤーのターン
				
			} else {
				// エネミーのターン

			}
		}

	}

}
