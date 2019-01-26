using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.PresetComponents.Roguelike {

	/// <summary>
	/// ゴールに達したときに発動するイベントを定義します。
	/// </summary>
	public class Goal : MonoBehaviour {

		/// <summary>
		/// 遷移中かどうか
		/// </summary>
		private bool isActivated = false;

		/// <summary>
		/// 配置されているタイル座標
		/// </summary>
		private static Vector2Int position;

		/// <summary>
		/// 初期化処理
		/// </summary>
		/// <param name="position">配置する位置</param>
		public static void Initialize(Vector2Int position) {
			Goal.position = position;
		}

		/// <summary>
		/// プレイヤーが入場したときにエンディングシーンへ遷移します。
		/// </summary>
		public void ExitIfTrigger(Vector2Int targetPosition) {
			if(!this.isActivated && targetPosition.x == Goal.position.x && targetPosition.y == Goal.position.y) {
				GameObject.Find("FadeCanvas").GetComponent<Fade>().FadeIn(1.0f, new Action(() => {
					UnityEngine.SceneManagement.SceneManager.LoadScene("Scenes/EndScene");
				}));
			}
		}

	}

}
