using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.PresetComponents.Roguelike.SectionDivision {

	/// <summary>
	/// 小部屋情報: 分割した各エリアに一つの小部屋を作る
	/// </summary>
	public class RoomData {

		/// <summary>
		/// 実質領域の最低幅
		/// </summary>
		public const int MinRoomWidth = 2;

		/// <summary>
		/// 方向の数
		/// </summary>
		public const int DirectionCount = 4;

		/// <summary>
		/// 壁の高さ
		/// </summary>
		public const int WallHeight = 2;

		/// <summary>
		/// 実質領域の最低高さ
		/// </summary>
		public const int MinRoomHeight = WallHeight + 2;

		/// <summary>
		/// 実質領域の下部余白の最低値
		/// </summary>
		public const int MinRoomBottomMargin = 3;

		/// <summary>
		/// 小部屋理論領域
		/// </summary>
		public RectInt AreaRange;

		/// <summary>
		/// 小部屋実質領域
		/// </summary>
		public RectInt RoomRange;

		/// <summary>
		/// 通路を繋ぐ交点座標
		/// </summary>
		public Vector2Int LinkVector2Int;

		/// <summary>
		/// 部屋から通路を作るときに使う交点座標: 添え字は４方向
		/// </summary>
		public Vector2Int[] CrossVector2Ints = null;

		/// <summary>
		/// 該当方向に通路が作れるかどうか: 添え字は４方向
		/// </summary>
		public bool[] AllowLinks = null;

		/// <summary>
		/// 該当方向に通路が作られたかどうか: 添え字は４方向
		/// </summary>
		public bool[] LinkExists = null;

		/// <summary>
		/// 部屋から作る通路の最大数
		/// </summary>
		public int MaxGateCount;

		/// <summary>
		/// この部屋から作られた通路の数
		/// </summary>
		public int GateCount;

		/// <summary>
		/// コンストラクター
		/// </summary>
		public RoomData(RectInt areaRange) {
			this.AreaRange = areaRange;
			this.CrossVector2Ints = new Vector2Int[DirectionCount];
			this.AllowLinks = new bool[DirectionCount];
			this.LinkExists = new bool[DirectionCount];

			int xWallLeftWidth;
			int xWallRightWidth;
			int yWallTopHeight;
			int yWallBottomHeight;

			// 通路用の余白をランダムに決定する
			do {
				// 左右の余白
				xWallLeftWidth = Random.Range(1, (areaRange.width - MinRoomWidth) / 2);
				xWallRightWidth = Random.Range(1, (areaRange.width - MinRoomWidth) / 2);

				// 上下の余白
				yWallTopHeight = Random.Range(1, (areaRange.height - MinRoomHeight) / 2);
				yWallBottomHeight = MinRoomBottomMargin + Random.Range(0, (areaRange.height - MinRoomHeight) / 2);

				// 実質領域を確定する
				this.RoomRange = new RectInt(
					areaRange.xMin + xWallLeftWidth,
					areaRange.yMin + yWallTopHeight,
					areaRange.width - (xWallLeftWidth + xWallRightWidth),
					areaRange.height - (yWallTopHeight + yWallBottomHeight)
				);
			} while(this.RoomRange.height < MinRoomHeight);        // 壁だけ、もしくは高さ１（床部分）の小部屋を作らないようにする
		}
	}

}
