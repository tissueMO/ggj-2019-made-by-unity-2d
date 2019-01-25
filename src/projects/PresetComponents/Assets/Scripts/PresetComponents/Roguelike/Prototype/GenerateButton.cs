using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Scripts.PresetComponents.Roguelike.Interface;
using UnityEngine;

namespace Assets.Scripts.PresetComponents.Roguelike {

	/// <summary>
	/// 現在のダンジョン生成ロジックで実際にマップを生成
	/// </summary>
	public class GenerateButton : MonoBehaviour {

		/// <summary>
		/// ロジック管理オブジェクト
		/// </summary>
		[SerializeField]
		private LogicChanger logicChanger;

		/// <summary>
		/// 生成するマップサイズ
		/// </summary>
		[SerializeField]
		private Vector2Int mapSize = new Vector2Int(50, 50);

		/// <summary>
		/// 複雑度
		/// </summary>
		[SerializeField]
		private int complexLevel = 5;

		/// <summary>
		/// 生成実行
		/// </summary>
		public void OnClick() {
			IMapGenerator logic = this.logicChanger.Logic;
			var map = logic.DoGenerate(this.mapSize, this.complexLevel);
			if(map != null) {
				Debug.Log(map.TileDataToString());
			}
		}

	}

}
