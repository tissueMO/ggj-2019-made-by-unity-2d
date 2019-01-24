using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.PresetComponents.Roguelike {

	/// <summary>
	/// ダンジョンマップデータ
	/// </summary>
	public class Layer2D {

		/// <summary>
		/// タイルデータ
		/// </summary>
		public enum Tile {
			Invalid = -1,
			None,
			Floor,
			Wall,
		}

		/// <summary>
		/// タイルデータを文字に変換するためのテーブル
		/// </summary>
		public static readonly Dictionary<Tile, string> TileToSymbolMap = new Dictionary<Tile, string>() {
		{Tile.Invalid, "×"},
		{Tile.Floor, "　"},
		{Tile.Wall, "■"},
	};

		/// <summary>
		/// マップサイズ（タイル単位）
		/// </summary>
		private Vector2Int size;

		/// <summary>
		/// 二次元タイルデータ
		/// </summary>
		private Tile[,] tiles = null;

		/// <summary>
		/// コンストラクター
		/// </summary>
		public Layer2D(Vector2Int size) {
			if(0 < size.x && 0 < size.y) {
				this.Initialize(size);
			}
		}

		/// <summary>
		/// マップを指定サイズで初期化します。
		/// </summary>
		/// <param name="size"></param>
		public void Initialize(Vector2Int size) {
			this.size = size;
			this.tiles = new Tile[size.y, size.x];
		}

		/// <summary>
		/// マップサイズを取得します。
		/// </summary>
		/// <returns>マップサイズ</returns>
		private Vector2Int getSize() {
			return this.size;
		}

		/// <summary>
		/// 指定した座標がマップの範囲外であるかどうかを調べます。
		/// </summary>
		/// <param name="position">対象座標</param>
		/// <returns>範囲外であるかどうか</returns>
		public bool IsOutOfRange(Vector2Int position) {
			if(this.tiles == null) {
				return true;
			}
			if(position.x < 0 || this.size.x <= position.x) {
				return true;
			}
			if(position.y < 0 || this.size.y <= position.y) {
				return true;
			}
			return false;
		}

		/// <summary>
		/// 指定した座標のタイルデータを取得します。
		/// </summary>
		/// <param name="position">対象座標</param>
		/// <returns>タイルデータ</returns>
		public Tile GetTile(Vector2Int position) {
			if(this.IsOutOfRange(position)) {
				return Tile.Invalid;
			}
			return this.tiles[position.y, position.x];
		}

		/// <summary>
		/// 指定した座標にタイルを配置します。
		/// </summary>
		/// <param name="position">対象座標</param>
		/// <param name="tile">配置するタイル</param>
		private void setTile(Vector2Int position, Tile tile) {
			if(this.IsOutOfRange(position)) {
				return;
			}

			this.tiles[position.y, position.x] = tile;
		}

		/// <summary>
		/// マップ全体を指定したタイルで塗りつぶします。
		/// </summary>
		/// <param name="tile">配置するタイル</param>
		public void Fill(Tile tile) {
			for(int x = 0; x < this.size.x; x++) {
				for(int y = 0; y < this.size.y; y++) {
					this.setTile(new Vector2Int(x, y), tile);
				}
			}
		}

		/// <summary>
		/// 指定した座標範囲を指定したタイルで塗りつぶします。
		/// </summary>
		/// <param name="range">対象座標範囲</param>
		/// <param name="tile">配置するタイル</param>
		public void FillRect(RectInt range, Tile tile) {
			for(int x = range.xMin; x <= range.xMax; x++) {
				for(int y = range.yMin; y <= range.yMax; y++) {
					this.setTile(new Vector2Int(x, y), tile);
				}
			}
		}

		/// <summary>
		/// 現在のマップをログに書き出します。
		/// </summary>
		public void DumpMapTiles() {
			Debug.Log($"MapSize=(Width, Height)=({this.size.x}, {this.size.y})");
			var dump = new StringBuilder("");
			for(int y = 0; y < this.size.y; y++) {
				for(int x = 0; x < this.size.x; x++) {
					dump.Append($"{Layer2D.TileToSymbolMap[this.tiles[y, x]]}");
				}
				dump.AppendLine();
			}
			Debug.Log(dump.ToString());
		}

	}
}
