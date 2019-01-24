using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.PresetComponents.Roguelike {

	/// <summary>
	/// ランダムダンジョン自動生成を行います。
	/// 参考: http://2dgames.jp/2015/02/01/dungeon/
	/// </summary>
	public class DangeonGenerator : MonoBehaviour {

		/// <summary>
		/// タイル情報
		/// </summary>
		private Layer2D map;

		/// <summary>
		/// 区画情報
		/// </summary>
		private List<DangeonDivision> divisions;

		/// <summary>
		/// 生成するマップの大きさ
		/// </summary>
		[SerializeField]
		private Vector2Int mapSize;

		/// <summary>
		/// 開始時にマップを生成します。
		/// </summary>
		private void Start() {
			this.map = new Layer2D(this.mapSize);
			this.divisions = new List<DangeonDivision>();

			// すべて歩けないタイルで埋める
			this.map.Fill(Layer2D.Tile.None);

			// 全体区画を作る
			this.createDivision(new RectInt(0, 0, this.mapSize.x, this.mapSize.y));

			// 区画を分割する
			bool isVertical = UnityEngine.Random.Range(0, 100) < 50;
			this.splitDivision(isVertical);

			// 区画に部屋を作る
			this.createRoom();

			// 部屋同士をつなぐ
			this.connectRooms();

			// Unity空間上にタイル配置
			for(int y = 0; y < this.mapSize.y; y++) {
				for(int x = 0; x < this.mapSize.x; x++) {
					switch(this.map.GetTile(new Vector2Int(x, y))) {
						case Layer2D.Tile.Floor:
							// 歩ける場所
							// TODO: プレハブからタイル配置
							break;

						case Layer2D.Tile.Wall:
							// 歩けない壁
							// TODO: プレハブからタイル配置
							break;
					}
				}
			}
		}

		/// <summary>
		/// 指定した領域に区画を生成します。
		/// </summary>
		/// <param name="range"></param>
		private void createDivision(RectInt range) {
			var div = new DangeonDivision();
			div.Outer = range;
		}

		/// <summary>
		/// 区画を分割します。
		/// </summary>
		/// <param name="isVertical">垂直方向に分割するかどうか</param>
		private void splitDivision(bool isVertical) {

		}

		private void createRoom() {

		}

		private void connectRooms() {

		}
	}

}
