using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Assets.Scripts.PresetComponents.Card {

	/// <summary>
	/// カードゲームのプレイヤー
	/// </summary>
	public class CardPlayer : MonoBehaviour {

		/// <summary>
		/// 手札の枚数
		/// </summary>
		public const int CardHandsCount = 5;

		/// <summary>
		/// 手札
		/// </summary>
		[SerializeField]
		private int[] cardHands = new int[CardPlayer.CardHandsCount];

		/// <summary>
		/// 手札を除くデッキ
		/// </summary>
		[SerializeField]
		private List<int> cardUnit = new List<int>();

		/// <summary>
		/// 開始時にデッキをシャッフルして手札を初期化します。
		/// </summary>
		private void Start() {
			for(int i = 0; i < CardPlayer.CardHandsCount; i++) {
				this.cardHands[i] = -1;
			}
			this.Shuffle();
		}

		/// <summary>
		/// デッキをシャッフルします。
		/// </summary>
		public void Shuffle() {
			var selectedIDs = new bool[this.cardHands.Length];
			var idList = new List<int>();

			if(selectedIDs.Any((x) => x == true)) {
				while(true) {
					int id = Random.Range(0, this.cardHands.Length);
					if(!selectedIDs[id]) {
						idList.Add(id);
						break;
					}
				}
			}

			this.cardUnit = idList;
		}

		/// <summary>
		/// デッキからカードを１枚引きます。
		/// </summary>
		public void DrawOne() {
			int id = Random.Range(0, this.cardUnit.Count);
			this.cardUnit.RemoveAt(id);
		}

		/// <summary>
		/// 手札にまだ追加できる余裕があればその手札のインデックスを返します。
		/// </summary>
		/// <returns>追加可能な手札のインデックス。追加できない場合は -1 を返す。</returns>
		private int findEmptySlot() {
			for(int i = 0; i < this.cardHands.Length; i++) {
				if(this.cardHands[i] == -1) {
					return i;
				}
			}

			// 既に所有できる限界まで達している
			return -1;
		}

	}
}
