using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Scripts.PresetComponents.Roguelike.Interface;
using Assets.Scripts.PresetComponents.Roguelike.SectionDivision;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.PresetComponents.Roguelike {

	/// <summary>
	/// ダンジョン生成ロジック変更
	/// </summary>
	public class LogicChanger : MonoBehaviour {

		/// <summary>
		/// 現在選択中のロジック
		/// </summary>
		public IMapGenerator Logic {
			get {
				return LogicChanger.Logics[this.logicIndex];
			}
		}

		/// <summary>
		/// 現在選択中のロジックのインデックス
		/// </summary>
		private int logicIndex;

		/// <summary>
		/// 生成ロジックインデックス
		/// </summary>
		public static readonly List<IMapGenerator> Logics = new List<IMapGenerator>() {
			new MapGenerator(),
		};

		/// <summary>
		/// ロジック変更時
		/// </summary>
		/// <param name="index">新しいインデックス</param>
		public void OnChange(int index) {
			var lastIndex = this.GetComponent<Dropdown>().value;
			this.logicIndex = (0 <= index && index < LogicChanger.Logics.Count) ? index : lastIndex;
			this.GetComponent<Dropdown>().value = this.logicIndex;
		}

	}

}
