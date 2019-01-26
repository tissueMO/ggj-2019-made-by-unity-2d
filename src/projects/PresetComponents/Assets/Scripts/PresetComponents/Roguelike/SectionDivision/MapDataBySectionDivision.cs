using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Scripts.PresetComponents.Roguelike.Interface;
using Assets.Scripts.Roguelike;
using UnityEngine;

namespace Assets.Scripts.PresetComponents.Roguelike.SectionDivision {

	/// <summary>
	/// ダンジョン全体の情報
	/// </summary>
	public class MapDataBySectionDivision : GeneratedMapBase {

		/// <summary>
		/// 分割の最低幅
		/// </summary>
		public const int MinSplitWidth = 8;

		/// <summary>
		/// 分割の最低高さ
		/// </summary>
		public const int MinSplitHeight = 10;

		/// <summary>
		/// 最低分割率
		/// </summary>
		public const int MinSplitRate = 10;

		/// <summary>
		/// 最高分割率
		/// </summary>
		public const int MaxSplitRate = 40;

		/// <summary>
		/// 複雑度の最大値
		/// </summary>
		public const int ComplexMaxLevel = 10;

		/// <summary>
		/// 小部屋リスト
		/// </summary>
		public List<RoomData> Rooms = new List<RoomData>();

		/// <summary>
		/// 分割線リスト
		/// </summary>
		public List<RoomLink> Links = new List<RoomLink>();

		/// <summary>
		/// プレイヤー１のオブジェクト
		/// </summary>
		[SerializeField]
		public GameObject player1;

		/// <summary>
		/// プレイヤー１のタイル位置
		/// </summary>
		private Vector2Int player1Position;

		/// <summary>
		/// コンストラクター
		/// </summary>
		public MapDataBySectionDivision(Vector2Int mapSize, GameObject player1) {
			this.Initialize(mapSize);
			this.player1 = player1;
		}

		/// <summary>
		/// このダンジョンを初期化します。
		/// </summary>
		public override void Initialize(Vector2Int size) {
			base.Initialize(size);

			// 唯一の小部屋を持つダンジョンにする
			this.Rooms.Clear();
			this.Rooms.Add(
				new RoomData(
					new RectInt(1, 1, this.DungeonRect.width - 1 * 2, this.DungeonRect.height - 1 * 2)
				)
			);

			// 分割線を初期化
			this.Links.Clear();
		}

		/// <summary>
		/// プレイヤー１の初期位置をランダムに決定します。
		/// </summary>
		public void SetAutoPlayerPosition() {
			int roomIndex = UnityEngine.Random.Range(0, this.Rooms.Count);
			var room = this.Rooms[roomIndex];
			this.player1Position = new Vector2Int(
				UnityEngine.Random.Range(room.RoomRange.xMin + 1, room.RoomRange.xMax - 1),
				UnityEngine.Random.Range(room.RoomRange.yMin + 1, room.RoomRange.yMax - 1)
			);

			// 画面上に表示
			this.player1.GetComponent<Player1>().Initialize(this);
			this.player1.transform.position = new Vector3(
				this.player1Position.x,
				-this.player1Position.y,
				0
			);
			this.player1.SetActive(true);
		}

		/// <summary>
		/// プレイヤー１のタイル位置を取得します。
		/// </summary>
		/// <returns>プレイヤー１のタイル位置</returns>
		public Vector2Int GetPlayerPosition() {
			return this.player1Position;
		}

		/// <summary>
		/// このダンジョンのランダム生成を実行します。
		/// </summary>
		/// <param name="complexLevel">複雑度: 分割数に関与</param>
		/// <returns>正常に生成できたかどうか</returns>
		public bool DoGenerate(int complexLevel) {
			if(MapDataBySectionDivision.ComplexMaxLevel < complexLevel) {
				Debug.LogWarning($"複雑度は {MapDataBySectionDivision.ComplexMaxLevel} 以内で設定して下さい。");
				return false;
			}

			// 何回分割するかを概算する
			int fy = this.DungeonRect.width * this.DungeonRect.height;
			int count = 0;
			while(fy > RoomData.MinRoomWidth * RoomData.MinRoomHeight) {
				fy /= 2;
				count++;
			}

			// 1. 分割線を作る
			int splitCount = count * complexLevel / MapDataBySectionDivision.ComplexMaxLevel;
			int lastIndex = 0;
			for(int n = 0; n < splitCount; n++) {
				if(this.RandomSplitArea(lastIndex, UnityEngine.Random.Range(MapDataBySectionDivision.MinSplitRate, MapDataBySectionDivision.MaxSplitRate))) {
					// 分割に成功したら次の分割を試行する
					lastIndex++;
				}
			}

			// 2. 分割してできた各小部屋について、分割線に向かう通路を生成する
			foreach(var room in this.Rooms) {
				// 小部屋内で通路の起点を作る
				room.LinkVector2Int = new Vector2Int(
					room.RoomRange.x + UnityEngine.Random.Range(0, room.RoomRange.width),
					room.RoomRange.y + UnityEngine.Random.Range(RoomData.WallHeight, room.RoomRange.height)
				);

				// 起点から４方向に向かって分割線があるかどうかをチェックする: 範囲は分割線を含めた小部屋理論領域
				room.MaxGateCount = 0;
				var checkRange = new RectInt(
					new Vector2Int(room.AreaRange.x - 1/*境界線分*/, room.AreaRange.y - 1/*境界線分*/),
					new Vector2Int(room.AreaRange.width + 1 * 2 + 1/*境界線分*/, room.AreaRange.height + 1 * 2 + 1/*境界線分*/)
				);
				for(int d = 0; d < (int)Direction.Count; d++) {
					Vector2Int nextTile = room.LinkVector2Int;

					while(true) {
						// 次の座標に進む
						nextTile = this.GetNextDirectionPosition(nextTile, (Direction)d);
						if(!checkRange.Contains(nextTile) || !this.DungeonRect.Contains(nextTile)) {
							// 次の座標が範囲外になったら分割線がないと判断する
							room.AllowLinks[d] = false;
							break;
						} else if(this.TileData[nextTile.x, nextTile.y] == GeneratedMapTile.Aisle) {
							// 次の座標が通路になっている場合は分割線であると判断する
							room.AllowLinks[d] = true;
							room.CrossVector2Ints[d] = nextTile;
							room.MaxGateCount++;
							break;
						}
					}
				}

				// 分割線に向かう通路をランダムに生成する
				room.GateCount = UnityEngine.Random.Range(1, room.MaxGateCount + 1);
				int generatedGateCount = 0;
				while(generatedGateCount < room.GateCount && room.GateCount <= room.MaxGateCount) {
					// 方向をランダムに決定する
					int dir = UnityEngine.Random.Range(0, (int)Direction.Count);
					if(!room.AllowLinks[dir]) {
						// その方向に分割線がなければやり直し
						continue;
					} else if(room.LinkExists[dir]) {
						// その方向の通路を既に作っていたらやり直し
						continue;
					}

					// 分割線への接続本数を更新する
					foreach(var link in this.Links) {
						if(link.Contains(room.CrossVector2Ints[dir])) {
							link.CrossCount++;
							break;
						}
					}

					// 通路生成: 起点から分割線との交点までをすべて床にする
					generatedGateCount++;
					room.LinkExists[dir] = true;
					switch((Direction)dir) {
						case Direction.Right:
							for(int x = room.LinkVector2Int.x; x < room.CrossVector2Ints[dir].x; x++) {
								this.TileData[x, room.LinkVector2Int.y] = GeneratedMapTile.Aisle;
							}
							break;

						case Direction.Left:
							for(int x = room.LinkVector2Int.x; room.CrossVector2Ints[dir].x < x; x--) {
								this.TileData[x, room.LinkVector2Int.y] = GeneratedMapTile.Aisle;
							}
							break;

						case Direction.Bottom:
							for(int y = room.LinkVector2Int.y; y < room.CrossVector2Ints[dir].y; y++) {
								this.TileData[room.LinkVector2Int.x, y] = GeneratedMapTile.Aisle;
							}
							break;

						case Direction.Top:
							for(int y = room.LinkVector2Int.y; room.CrossVector2Ints[dir].y < y; y--) {
								this.TileData[room.LinkVector2Int.x, y] = GeneratedMapTile.Aisle;
							}
							break;
					}
				}
			}

			// 3. 分割線に正しく接続できているか調べる
			foreach(var link in this.Links) {
				//if(link.CrossCount < 2) {
				if(link.CrossCount < 1) {
					// 接続本数が２本未満だと、孤立した小部屋ができてしまうのでやり直し
					this.Initialize(new Vector2Int(this.TileData.GetLength(0), this.TileData.GetLength(1)));
					return false;
				}
			}

			// 4. 分割線の両端を削る: 分割線の検証方向以外の方向に床がなければ逐次削っていく
			foreach(var link in this.Links) {
				switch(link.SplitType) {
					case RoomLink.SplitDirection.Horizontal:
						// 上下分割: 横線
						this.TrimLink(link, Direction.Right);
						this.TrimLink(link, Direction.Left);
						break;

					case RoomLink.SplitDirection.Vertical:
						// 左右分割: 縦線
						this.TrimLink(link, Direction.Bottom);
						this.TrimLink(link, Direction.Top);
						break;
				}
			}

			// 5. 各通路の上部を壁にする
			foreach(var link in this.Links) {
				switch(link.SplitType) {
					case RoomLink.SplitDirection.Horizontal:
						// 横線は上部がすべて壁になる

						for(int x = link.Start.x; x <= link.End.x; x++) {
							if(this.TileData[x, link.Start.y] != GeneratedMapTile.Aisle) {
								// 削られた通路部分は処理しない
								continue;
							}

							// 壁にしようとしている部分が既に床にされていないか検証する
							bool wallNG = false;
							for(int n = 1; n <= RoomData.WallHeight; n++) {
								if(this.TileData[x, link.Start.y - n] == GeneratedMapTile.Floor
								|| this.TileData[x, link.Start.y - n] == GeneratedMapTile.Aisle) {
									wallNG = true;
									break;
								}
							}
							if(wallNG) {
								// 既に床にされているときは塞がない
								continue;
							}

							// 壁化実行
							for(int n = 1; n <= RoomData.WallHeight; n++) {
								this.TileData[x, link.Start.y - n] = GeneratedMapTile.Wall;
							}
						}
						break;

					case RoomLink.SplitDirection.Vertical:
						// 縦線は一番上だけを壁にする

						for(int y = link.Start.y; y <= link.End.y; y++) {
							if(this.TileData[link.Start.x, y] != GeneratedMapTile.Aisle) {
								// 削られた通路部分は処理しない
								continue;
							}

							// 壁にしようとしている部分が既に床にされていないか検証する
							bool wallNG = false;
							for(int n = 1; n <= RoomData.WallHeight; n++) {
								if(this.TileData[link.Start.x, y - n] == GeneratedMapTile.Floor
								|| this.TileData[link.Start.x, y - n] == GeneratedMapTile.Aisle) {
									wallNG = true;
									break;
								}
							}
							if(wallNG) {
								// 既に床にされているときは塞がない
								break;
							}

							// 壁化実行
							for(int n = 1; n <= RoomData.WallHeight; n++) {
								this.TileData[link.Start.x, y - n] = GeneratedMapTile.Wall;
							}
							break;
						}
						break;
				}
			}

			// 6. 各小部屋の床と壁を作る
			foreach(var room in this.Rooms) {
				// 6-1. 各小部屋内部の床と壁を作る
				for(int x = room.RoomRange.xMin; x < room.RoomRange.xMax; x++) {
					for(int y = room.RoomRange.yMin, n = 0; y < room.RoomRange.yMax; y++, n++) {
						if(n < RoomData.WallHeight && (!room.LinkExists[(int)Direction.Top] || room.LinkVector2Int.x != x)) {
							// 上２マスは壁にする: ただし通路は塞がない
							this.TileData[x, y] = GeneratedMapTile.Wall;
						} else {
							this.TileData[x, y] = GeneratedMapTile.Floor;
						}
					}
				}

				// 6-2. 横線の通路に上部に壁を作る
				for(int d = 0; d < (int)Direction.Count; d++) {
					if(!room.LinkExists[d]) {
						// 通路がない場合は処理しない
						continue;
					}
					switch((Direction)d) {
						case Direction.Right:
							for(int x = room.RoomRange.xMax; x <= room.AreaRange.xMax; x++) {
								for(int n = 1; n <= RoomData.WallHeight; n++) {
									this.TileData[x, room.LinkVector2Int.y - n] = GeneratedMapTile.Wall;
								}
							}
							break;

						case Direction.Left:
							for(int x = room.AreaRange.xMin; x < room.RoomRange.xMin; x++) {
								for(int n = 1; n <= RoomData.WallHeight; n++) {
									this.TileData[x, room.LinkVector2Int.y - n] = GeneratedMapTile.Wall;
								}
							}
							break;
					}
				}
			}

			// 7. 各通路と各小部屋の外周に天井（＝枠）を付ける
			// 小部屋の外周に天井を付ける
			foreach(var room in this.Rooms) {
				var rect = room.RoomRange;

				for(int x = rect.xMin - 1; x < rect.xMax + 1; x++) {
					if(0 <= x && 0 <= rect.yMin - 1 && this.TileData[x, rect.yMin - 1] == GeneratedMapTile.None) {
						this.TileData[x, rect.yMin - 1] = GeneratedMapTile.Ceil;
						Debug.Log($"Ceil: {x}, {rect.yMin - 1}");
					}
					if(0 <= x && rect.yMax < this.DungeonRect.height && this.TileData[x, rect.yMax] == GeneratedMapTile.None) {
						this.TileData[x, rect.yMax] = GeneratedMapTile.Ceil;
						Debug.Log($"Ceil: {x}, {rect.yMax}");
					}
				}
				for(int y = rect.yMin - 1; y < rect.yMax + 1; y++) {
					if(0 <= y && 0 <= rect.xMin - 1 && this.TileData[rect.xMin - 1, y] == GeneratedMapTile.None) {
						this.TileData[rect.xMin - 1, y] = GeneratedMapTile.Ceil;
						Debug.Log($"Ceil: {rect.xMin - 1}, {y}");
					}
					if(0 <= y && rect.xMax < this.DungeonRect.width && this.TileData[rect.xMax, y] == GeneratedMapTile.None) {
						this.TileData[rect.xMax, y] = GeneratedMapTile.Ceil;
						Debug.Log($"Ceil: {rect.xMax}, {y}");
					}
				}
			}
			// 通路の外周に天井を付ける
			foreach(var link in this.Links) {
				RectInt rect;
				int baseX = -1;
				int baseY = -1;

				if(link.SplitType == RoomLink.SplitDirection.Horizontal) {
					baseY = link.Start.y;
					rect = new RectInt(
						link.Start.x,
						baseY - RoomData.WallHeight,
						link.End.x - link.Start.x,
						1 + RoomData.WallHeight
					);
				} else {
					baseX = link.Start.x;
					rect = new RectInt(
						link.Start.x,
						link.Start.y,
						1,
						1 + link.End.y - link.Start.y
					);
				}

				for(int x = rect.xMin - 1; x < rect.xMax + 1; x++) {
					if(link.SplitType == RoomLink.SplitDirection.Horizontal) {
						if(this.TileData[x, baseY] == GeneratedMapTile.None) {
							continue;
						}
					}

					if(0 <= x && 0 <= rect.yMin - 1 && this.TileData[x, rect.yMin - 1] == GeneratedMapTile.None) {
						this.TileData[x, rect.yMin - 1] = GeneratedMapTile.Ceil;
						Debug.Log($"Ceil: {x}, {rect.yMin - 1}");
					}
					if(0 <= x && rect.yMax < this.DungeonRect.height && this.TileData[x, rect.yMax] == GeneratedMapTile.None) {
						this.TileData[x, rect.yMax] = GeneratedMapTile.Ceil;
						Debug.Log($"Ceil: {x}, {rect.yMax}");
					}
				}
				for(int y = rect.yMin - 1; y < rect.yMax + 1; y++) {
					if(link.SplitType == RoomLink.SplitDirection.Vertical) {
						if(this.TileData[baseX, y] == GeneratedMapTile.None) {
							continue;
						}
					}

					if(0 <= y && 0 <= rect.xMin - 1 && this.TileData[rect.xMin - 1, y] == GeneratedMapTile.None) {
						this.TileData[rect.xMin - 1, y] = GeneratedMapTile.Ceil;
						Debug.Log($"Ceil: {rect.xMin - 1}, {y}");
					}
					if(0 <= y && rect.xMax < this.DungeonRect.width && this.TileData[rect.xMax, y] == GeneratedMapTile.None) {
						this.TileData[rect.xMax, y] = GeneratedMapTile.Ceil;
						Debug.Log($"Ceil: {rect.xMax}, {y}");
					}
				}
			}

			// 8. プレイヤー１を配置
			this.SetAutoPlayerPosition();

			// 正常に生成された
			return true;
		}

		/// <summary>
		/// 分割方向をランダムに決定します。
		/// </summary>
		static private RoomLink.SplitDirection GetRandomSplitDirection() {
			if(UnityEngine.Random.Range(0, 100) < 50) {
				return RoomLink.SplitDirection.Horizontal;
			} else {
				return RoomLink.SplitDirection.Vertical;
			}
		}

		/// <summary>
		/// 指定した小部屋を分割します。
		/// </summary>
		/// <param name="baseRoomIndex">分割する小部屋のインデックス</param>
		/// <param name="splRate">分割率 (0~100)</param>
		/// <returns>分割に成功しかどうか</returns>
		public bool RandomSplitArea(int baseRoomIndex, int splRate) {
			int splVector2Int;
			var newArea = new RectInt();
			var newLink = new RoomLink();

			// 分割元の小部屋で辺の長い方を見て分割する: 極端に狭い部屋が生成されすぎないようにする
			if(this.Rooms[baseRoomIndex].AreaRange.width > this.Rooms[baseRoomIndex].AreaRange.height) {
				// 横長のときは左右分割
				newLink.SplitType = RoomLink.SplitDirection.Vertical;
			} else {
				// 縦長の時は上下分割
				newLink.SplitType = RoomLink.SplitDirection.Horizontal;
			}

			// 与えられた小部屋を分割する
			switch(newLink.SplitType) {
				case RoomLink.SplitDirection.Horizontal:
					// 上下分割: 横線

					// 分割線の設定
					splVector2Int = this.Rooms[baseRoomIndex].AreaRange.height * splRate / 100;
					if(splVector2Int < MapDataBySectionDivision.MinSplitHeight) {
						// 細かすぎて分割できない場合は処理しない
						return false;
					}
					newLink.Start = new Vector2Int(
						this.Rooms[baseRoomIndex].AreaRange.x,
						this.Rooms[baseRoomIndex].AreaRange.y + splVector2Int
					);
					newLink.End = new Vector2Int(
						this.Rooms[baseRoomIndex].AreaRange.x + this.Rooms[baseRoomIndex].AreaRange.width - 1,
						this.Rooms[baseRoomIndex].AreaRange.y + splVector2Int
					);

					// 分割して新しくできた小部屋の設定
					newArea.position = new Vector2Int(
						this.Rooms[baseRoomIndex].AreaRange.x,
						this.Rooms[baseRoomIndex].AreaRange.y + splVector2Int + 1
					);
					newArea.size = new Vector2Int(
						this.Rooms[baseRoomIndex].AreaRange.width,
						this.Rooms[baseRoomIndex].AreaRange.height - splVector2Int - 1
					);

					// 分割元の小部屋の再設定
					this.Rooms[baseRoomIndex] = new RoomData(new RectInt(
						this.Rooms[baseRoomIndex].AreaRange.position,
						new Vector2Int(this.Rooms[baseRoomIndex].AreaRange.width, splVector2Int - 1)
					));
					break;

				case RoomLink.SplitDirection.Vertical:
					// 左右分割: 縦線

					// 分割線の設定
					splVector2Int = this.Rooms[baseRoomIndex].AreaRange.width * splRate / 100;
					if(splVector2Int < MapDataBySectionDivision.MinSplitWidth) {
						// 細かすぎて分割できない場合は処理しない
						return false;
					}
					newLink.Start = new Vector2Int(
						this.Rooms[baseRoomIndex].AreaRange.x + splVector2Int,
						this.Rooms[baseRoomIndex].AreaRange.y
					);
					newLink.End = new Vector2Int(
						this.Rooms[baseRoomIndex].AreaRange.x + splVector2Int,
						this.Rooms[baseRoomIndex].AreaRange.y + this.Rooms[baseRoomIndex].AreaRange.height - 1
					);

					// 分割して新しくできた小部屋の設定
					newArea.position = new Vector2Int(this.Rooms[baseRoomIndex].AreaRange.x + splVector2Int + 1, this.Rooms[baseRoomIndex].AreaRange.y);
					newArea.size = new Vector2Int(this.Rooms[baseRoomIndex].AreaRange.width - splVector2Int - 1, this.Rooms[baseRoomIndex].AreaRange.height);

					// 分割元の小部屋の再設定
					this.Rooms[baseRoomIndex] = new RoomData(new RectInt(
						this.Rooms[baseRoomIndex].AreaRange.position,
						new Vector2Int(splVector2Int - 1, this.Rooms[baseRoomIndex].AreaRange.height)
					));
					break;
			}

			// 分割して新しくできた小部屋を追加する
			this.Rooms.Add(new RoomData(newArea));

			// 分割線を追加する
			this.Links.Add(newLink);

			// 分割線の部分を暫定的に床にする
			switch(newLink.SplitType) {
				case RoomLink.SplitDirection.Horizontal:
					for(int x = newLink.Start.x; x <= newLink.End.x; x++) {
						this.TileData[x, newLink.Start.y] = GeneratedMapTile.Aisle;
					}
					break;

				case RoomLink.SplitDirection.Vertical:
					for(int y = newLink.Start.y; y <= newLink.End.y; y++) {
						this.TileData[newLink.Start.x, y] = GeneratedMapTile.Aisle;
					}
					break;
			}
			return true;
		}

		/// <summary>
		/// 指定した進行方向で通路を削ります。
		/// </summary>
		/// <param name="link">対象とする通路</param>
		/// <param name="trimDirection">削っていく方向 (東向きの場合は左端が削られます)</param>
		public void TrimLink(RoomLink link, Direction trimDirection) {
			// 始点・終点・固定軸を決定する
			int start = 0;
			int end = 0;
			int pivot = 0;
			int step = 0;

			switch(trimDirection) {
				case Direction.Right:
					start = link.Start.x;
					end = link.End.x;
					pivot = link.Start.x;
					step = 1;
					break;

				case Direction.Left:
					start = link.End.x;
					end = link.Start.x;
					pivot = link.Start.y;
					step = -1;
					break;

				case Direction.Bottom:
					start = link.Start.y;
					end = link.End.y;
					pivot = link.Start.x;
					step = 1;
					break;

				case Direction.Top:
					start = link.End.y;
					end = link.Start.y;
					pivot = link.Start.x;
					step = -1;
					break;
			}

			// 逐次削っていく
			for(int p = start; (0 < step) ? (start <= p && p <= end) : (end <= p && p <= start); p += step) {
				bool cutFlag = true;

				// 次のタイル位置をセットする
				Vector2Int checkTilePos = Vector2Int.zero;
				switch(trimDirection) {
					case Direction.Right:
					case Direction.Left:
						checkTilePos = new Vector2Int(p, pivot);
						break;

					case Direction.Bottom:
					case Direction.Top:
						checkTilePos = new Vector2Int(pivot, p);
						break;
				}

				// ３方向チェック
				for(int d = 0; d < RoomData.DirectionCount; d++) {
					if(d == (int)trimDirection) {
						// 進行方向はチェックしない
						continue;
					}

					Vector2Int checkTileNextPos = this.GetNextDirectionPosition(checkTilePos, (GeneratedMapBase.Direction)d);
					if(this.TileData[checkTileNextPos.x, checkTileNextPos.y] == GeneratedMapTile.Floor
					|| this.TileData[checkTileNextPos.x, checkTileNextPos.y] == GeneratedMapTile.Aisle) {
						cutFlag = false;
						break;
					}
				}
				if(cutFlag) {
					// 現在のタイルを削る
					this.TileData[checkTilePos.x, checkTilePos.y] = GeneratedMapTile.None;
				}
			}
		}

	}

}
