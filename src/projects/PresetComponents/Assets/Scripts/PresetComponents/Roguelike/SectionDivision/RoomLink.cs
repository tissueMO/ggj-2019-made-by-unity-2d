using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.PresetComponents.Roguelike.SectionDivision {

	/// <summary>
	/// 通路情報: 分割線
	/// </summary>
	public class RoomLink {

		/// <summary>
		/// 分割方向の種類
		/// </summary>
		public enum SplitDirection {
			Horizontal,     // 上下分割
			Vertical,       // 左右分割
		}

		/// <summary>
		/// 分割方向
		/// </summary>
		public SplitDirection SplitType;

		/// <summary>
		/// 左端・上端
		/// </summary>
		public Vector2Int Start;

		/// <summary>
		/// 右端・下端
		/// </summary>
		public Vector2Int End;

		/// <summary>
		/// この分割線に接続している通路の数
		/// </summary>
		public int CrossCount = 0;

		/// <summary>
		/// 指定した座標がこの分割線の上にあるかどうかを調べる
		/// </summary>
		public bool Contains(Vector2Int pos) {
			return new RectInt(this.Start.x, this.Start.y, this.End.x + 1, this.End.y + 1).Contains(pos);
		}

	}

}
