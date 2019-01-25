using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.PresetComponents.Roguelike.Interface {

	/// <summary>
	/// 生成されるマップの基底クラス
	/// </summary>
	public class GeneratedMapBase {

		/// <summary>
		/// 生成されたタイルの情報
		/// </summary>
		public enum GeneratedMapTile {
			Floor,          // 床: 通行OK
			Wall,           // 壁: 通行NG
			Ceil,           // 天井: 通行NG
		}

		/// <summary>
		/// 方向
		/// </summary>
		public enum Direction {
			Right,
			Bottom,
			Left,
			Top,
			Count
		}

		/// <summary>
		/// ダンジョンサイズ
		/// </summary>
		public RectInt DungeonRect;

		/// <summary>
		/// タイルデータ
		/// </summary>
		public GeneratedMapTile[,] TileData {
			get;
			private set;
		}

		/// <summary>
		/// 初期化処理
		/// </summary>
		public virtual void Initialize(Vector2Int size) {
			// 暫定的にすべて天井にする
			this.DungeonRect = new RectInt(Vector2Int.zero, size);
			this.TileData = new GeneratedMapTile[this.DungeonRect.width, this.DungeonRect.height];
			for(int x = 0; x < this.DungeonRect.width; x++) {
				for(int y = 0; y < this.DungeonRect.height; y++) {
					this.TileData[x, y] = GeneratedMapTile.Ceil;
				}
			}
		}

		/// <summary>
		/// 起点から指定した方向に一つ進んだ座標を返します。
		/// </summary>
		public Vector2Int GetNextDirectionPosition(Vector2Int startPos, GeneratedMapBase.Direction dir) {
			Vector2Int nextTile = startPos;

			switch(dir) {
				case GeneratedMapBase.Direction.Right:
					nextTile = new Vector2Int(nextTile.x + 1, nextTile.y);
					break;
				case GeneratedMapBase.Direction.Bottom:
					nextTile = new Vector2Int(nextTile.x, nextTile.y + 1);
					break;
				case GeneratedMapBase.Direction.Left:
					nextTile = new Vector2Int(nextTile.x - 1, nextTile.y);
					break;
				case GeneratedMapBase.Direction.Top:
					nextTile = new Vector2Int(nextTile.x, nextTile.y - 1);
					break;
			}

			return nextTile;
		}

		/// <summary>
		/// 現在のタイルデータを文字列に変換する
		/// </summary>
		public string TileDataToString() {
			string buf = "";

			// 一行ごとに生成する
			for(int y = 0; y < this.DungeonRect.height; y++) {
				for(int x = 0; x < this.DungeonRect.width; x++) {
					switch(this.TileData[x, y]) {
						case GeneratedMapTile.Floor:
							buf += "　";
							break;
						case GeneratedMapTile.Wall:
							buf += "壁";
							break;
						case GeneratedMapTile.Ceil:
							buf += "■";
							break;
					}
				}
				buf += "\r\n";
			}

			return buf;
		}

	}

}
