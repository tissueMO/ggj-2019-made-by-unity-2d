using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.PresetComponents.Roguelike {

	/// <summary>
	/// ダンジョンマップの区画データ
	/// </summary>
	public class DangeonDivision {

		/// <summary>
		/// 区画領域
		/// </summary>
		public RectInt Outer;

		/// <summary>
		/// 区画内の部屋領域
		/// </summary>
		public RectInt Room;

	}

}
