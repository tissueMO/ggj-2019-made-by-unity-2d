using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.PresetComponents.Roguelike.Interface {

	/// <summary>
	/// ダンジョンを生成できるインターフェースです。
	/// </summary>
	public interface IMapGenerator {

		/// <summary>
		/// 生成を実行します。
		/// </summary>
		/// <param name="mapSize">生成時のマップサイズ</param>
		/// <param name="complexLevel">複雑度 0～100</param>
		/// <param name="player1">プレイヤー１のオブジェクト</param>
		/// <returns>生成したマップ</returns>
		GeneratedMapBase DoGenerate(Vector2Int mapSize, int complexLevel, GameObject player1);

	}

}
