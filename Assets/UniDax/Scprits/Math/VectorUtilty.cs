using UnityEngine;

namespace UniDax.Math
{
	public static class VectorUtilty
	{
		/// <summary>
		/// 法線に対する反射ベクトルを求める
		/// </summary>
		/// <param name="vec">入力</param>
		/// <param name="nor">法線</param>
		/// <returns></returns>
		public static Vector3 WallReflect(this Vector3 vec, Vector3 nor)
		{
			return (vec - (nor * Vector3.Dot(vec, nor) * 2)).normalized;
		}

		/// <summary>
		/// 法線に対する壁ずりベクトルを求める
		/// </summary>
		/// <param name="vec">入力</param>
		/// <param name="nor">法線</param>
		/// <returns></returns>
		public static Vector3 WallScratch(this Vector3 vec, Vector3 nor)
		{
			return (vec - (nor * Vector3.Dot(vec, nor))).normalized;
		}
	}
}