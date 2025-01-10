using System;
using UnityEngine;

namespace ETTView.Math
{
	public abstract class Inertia<T>
	{
		//このフレームで足される速度（加速度）
		protected T _frameSpeed;
		//速度（秒速）
		protected T _speed;

		public virtual T Speed
		{
			get { return _speed; }
			set { _speed = value; }
		}

		//加算
		protected abstract T Add(T a, T b);

		//実数倍
		protected abstract T Mag(T a, float value);

		//位置制限用
		protected virtual T PosAdjust(T pos)
		{
			return pos;
		}

		protected virtual void FrameSpeedAdjust()
		{
		}

		//加速
		public void AddSpeed(T add)
		{
			_frameSpeed = Add(_frameSpeed, add);
		}

		//現在値に反映
		public T Reflect(T pos, float decayValue)
		{
			FrameSpeedAdjust();
            Speed = Add(Speed, _frameSpeed);
            Speed = Mag(Speed, 1.0f - decayValue * Time.deltaTime);
			_frameSpeed = default(T);
			return PosAdjust(Add(pos, Mag(Speed, Time.deltaTime)));
		}
	}
	//比較可能な慣性値
	public abstract class ComparableInertia<T> : Inertia<T> where T : struct, IComparable
	{
		public T? Max { get; set; } = null;
		public T? Min { get; set; } = null;
		protected override T PosAdjust(T pos)
		{
			if (Max != null && pos.CompareTo(Max) > 0)
			{
				pos = (T)Max;
			}
			if (Min != null && pos.CompareTo(Min) < 0)
			{
				pos = (T)Min;
			}
			return pos;
		}
	}

	//ベクトル型
	public class VectorInertia : Inertia<Vector3>
	{
		//加速度制限
		public float? MaxMagnitude { get; set; } = null;
        public float? MinMagnitude { get; set; } = null;
        public Rect? LimitRect { get; set; } = null;
		public Action<float> OnOverhang{ get; set; } = null;

		Vector3 _dir = Vector3.up;

        public override Vector3 Speed
		{
			get => base.Speed;
			set
			{
				if (value != Vector3.zero)
					_dir = value.normalized;
				base.Speed = value;
			}
		}

        protected override void FrameSpeedAdjust()
		{
            //長さが指定より超えてたらそろえる
            if (MaxMagnitude != null && _frameSpeed.magnitude > MaxMagnitude)
			{
				_frameSpeed = _frameSpeed.normalized * (float)MaxMagnitude;
			}

			//長さが指定より短かったらそろえる
			if(MinMagnitude != null && _frameSpeed.magnitude < MinMagnitude)
			{
				_frameSpeed = _dir * (float)MinMagnitude;
			}
		}

		protected override Vector3 PosAdjust(Vector3 pos)
		{
			if (LimitRect != null)
			{
				if (LimitRect.Value.xMin > pos.x)
				{
					pos.x = LimitRect.Value.xMin;
					var mag = _speed.magnitude;
					_speed = _speed.WallReflect( Vector3.right ) * mag;
				}
				if (LimitRect.Value.xMax < pos.x)
				{
					pos.x = LimitRect.Value.xMax;
					var mag = _speed.magnitude;
					_speed = _speed.WallReflect(Vector3.right) * mag;
				}
				if (LimitRect.Value.yMin > pos.y)
				{
					pos.y = LimitRect.Value.yMin;
					var mag = _speed.magnitude;
					_speed = _speed.WallReflect(Vector3.up) * mag;
				}
				if (LimitRect.Value.yMax < pos.y)
				{
					pos.y = LimitRect.Value.yMax;
					var mag = _speed.magnitude;
					_speed = _speed.WallReflect(Vector3.down) * mag;
				}
			}

			return pos;
		}

		protected override Vector3 Add(Vector3 a, Vector3 b)
		{
			return a + b;
		}

		protected override Vector3 Mag(Vector3 a, float value)
		{
			return a * value;
		}

		/// <summary>
		/// 慣性体の衝突
		/// </summary>
		/// <returns></returns>
		public void Collision(Vector3 currentPos, Vector3 targetPos, VectorInertia target)
		{
			var c = (targetPos - currentPos).normalized;   //衝突軸ベクトル
			var dot = Vector3.Dot(Speed - target.Speed, c);
			Speed = c * dot + Speed;
			target.Speed = c * dot + target.Speed;
		}

	}
	//実数型慣性値
	public class FloatInertia : ComparableInertia<float>
	{
		protected override float Add(float a, float b)
		{
			return a + b;
		}
		protected override float Mag(float a, float value)
		{
			return a * value;
		}
	}
}