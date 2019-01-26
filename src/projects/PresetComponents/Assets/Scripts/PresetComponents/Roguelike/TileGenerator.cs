using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Scripts.PresetComponents.Roguelike.Interface;
using UnityEngine;

namespace Assets.Scripts.PresetComponents.Roguelike.TileGenerator {

	/// <summary>
	/// タイル設定に基づいて画面上にタイルを配置します。
	/// </summary>
	public class TileGenerator : MonoBehaviour {

		/// <summary>
		/// タイル番号とオブジェクトのマッピング
		/// </summary>
		private Dictionary<GeneratedMapBase.GeneratedMapTile, GameObject> tileObjectMap;

		/// <summary>
		/// 床タイル
		/// </summary>
		private GameObject tileFloor;

		/// <summary>
		/// 通路タイル
		/// </summary>
		private GameObject tileAisle;

		/// <summary>
		/// 壁タイル
		/// </summary>
		private GameObject tileWall;

		/// <summary>
		/// 天井タイル
		/// </summary>
		private GameObject tileCeil;

		/// <summary>
		/// タイルを格納する親オブジェクト
		/// </summary>
		private GameObject tileContainer;

		/// <summary>
		/// 生成して配置したオブジェクトのリスト
		/// </summary>
		private List<GameObject> generatedTiles;

		/// <summary>
		/// 必要な材料を読み込みます。
		/// </summary>
		public void Start() {
			this.tileContainer = GameObject.Find("MapTiles");
			this.tileFloor = Resources.Load<GameObject>("Prefabs/Map/TileFloor");
			this.tileAisle = Resources.Load<GameObject>("Prefabs/Map/TileAisle");
			this.tileWall = Resources.Load<GameObject>("Prefabs/Map/TileWall");
			this.tileCeil = Resources.Load<GameObject>("Prefabs/Map/TileCeil");
			this.tileObjectMap = new Dictionary<GeneratedMapBase.GeneratedMapTile, GameObject>() {
				{ GeneratedMapBase.GeneratedMapTile.None, null },
				{ GeneratedMapBase.GeneratedMapTile.Floor, this.tileFloor },
				{ GeneratedMapBase.GeneratedMapTile.Aisle, this.tileAisle },
				{ GeneratedMapBase.GeneratedMapTile.Ceil, this.tileCeil },
				{ GeneratedMapBase.GeneratedMapTile.Wall, this.tileWall },
			};
			this.generatedTiles = new List<GameObject>();
		}

		/// <summary>
		/// タイルを配置します。
		/// </summary>
		/// <param name="generatedMap">生成されたマップオブジェクト</param>
		public void GenerateTiles(GeneratedMapBase generatedMap) {
			// 以前のマップを破棄する
			if(this.generatedTiles != null) {
				foreach(var obj in this.generatedTiles) {
					if(obj != null) {
						GameObject.Destroy(obj);
					}
				}
			}
			this.generatedTiles = new List<GameObject>();

			// 指定されたマップで描画する
			for(int x = 0; x < generatedMap.DungeonRect.width; x++) {
				for(int y = 0; y < generatedMap.DungeonRect.height; y++) {
					if(this.tileObjectMap[generatedMap.TileData[x, y]] == null) {
						continue;
					}
					this.generatedTiles.Add(GameObject.Instantiate<GameObject>(
						this.tileObjectMap[generatedMap.TileData[x, y]],
						new Vector3(x, -y, 0),
						new Quaternion(),
						this.tileContainer.transform
					));
				}
			}
		}

	}

}
