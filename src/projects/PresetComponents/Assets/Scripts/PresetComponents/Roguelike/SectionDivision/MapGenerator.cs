using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.PresetComponents.Roguelike.Interface;
using UnityEngine;


namespace Assets.Scripts.PresetComponents.Roguelike.SectionDivision {

	/// <summary>
	/// ダンジョン生成クラス
	/// 参考元：http://misw.jp/archives/835
	/// </summary>
	public class MapGenerator : IMapGenerator {

		/// <summary>
		/// [ゴリ押し] 作り直しできる限界回数
		/// </summary>
		public const int MaxRebuildCount = 2000;

		/// <summary>
		/// 指定したサイズに収まるランダムダンジョンを生成します。
		/// </summary>
		public GeneratedMapBase DoGenerate(Vector2Int mapSize, int complexLevel, GameObject player1, GameObject tileContainer) {
			var map = new MapDataBySectionDivision(mapSize, player1, tileContainer);

			int i;
			for(i = 1; !map.DoGenerate(complexLevel) && i <= MapGenerator.MaxRebuildCount; i++) {
				// 正常に生成されるまで繰り返す
				Debug.Log("ダンジョンの生成に失敗しました。再試行します... [" + i.ToString() + " 回目]");
			}

			if(i > MaxRebuildCount) {
				// 作り直しの限界数に達した場合は中止する
				Debug.LogError("ランダム生成中に何らかの問題があったため、処理を中断します。\r\n生成オプションを確認して下さい。");
				return null;
			}

			return map;
		}

	}
}
