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
		/// プレイヤーが入場したときにエンディングシーンへ遷移します。
		/// </summary>
		/// <param name="other">接触したオブジェクト</param>
		public void OnTriggerEnter(Collider other) {
			if(other.tag != "Player") {
				return;
			}

			GameObject.Find("FadeCanvas").GetComponent<Fade>().FadeIn(1.0f, new Action(() => {
				UnityEngine.SceneManagement.SceneManager.LoadScene("Scenes/EndScene");
			}));
		}

	}

}
