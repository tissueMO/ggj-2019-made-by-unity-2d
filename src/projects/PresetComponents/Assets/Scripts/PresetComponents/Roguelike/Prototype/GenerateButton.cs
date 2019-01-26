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
		/// タイル配置管理オブジェクト
		/// </summary>
		[SerializeField]
		private TileGenerator.TileGenerator tileGenerator;

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
		/// プレイヤー１のオブジェクト
		/// </summary>
		[SerializeField]
		private GameObject player1;

		/// <summary>
		/// 開始後、即座にマップを生成するかどうか
		/// </summary>
		[SerializeField]
		private bool generateMapImmediately = false;

		/// <summary>
		/// 開始時に自動的にマップを生成します。
		/// </summary>
		public void Update() {
			if(this.enabled && this.generateMapImmediately) {
				this.enabled = false;
				this.OnClick();
			}
		}

		/// <summary>
		/// 生成実行
		/// </summary>
		public void OnClick() {
			IMapGenerator logic = this.logicChanger.Logic;
			var map = logic.DoGenerate(this.mapSize, this.complexLevel, this.player1);
			if(map != null) {
				// マップタイルを配置
				Debug.Log(map.TileDataToString());
				this.tileGenerator.GenerateTiles(map);
			}
		}

	}

}
