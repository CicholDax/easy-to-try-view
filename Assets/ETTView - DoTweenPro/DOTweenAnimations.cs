using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Cysharp.Threading.Tasks;

namespace ETTView
{
	//DOTweenをまとめて再生
	[System.Serializable]
	public class DOTweenAnimations
	{
		[System.Serializable]
		public class AnimationList
		{
			[SerializeField]
			List<DOTweenAnimation> parallel;

			public List<DOTweenAnimation> Parallel { get { return parallel; } }
		}

		[SerializeField]
		List<AnimationList> _animeLists;

		public bool IsEmpty()
		{
			return _animeLists.Count <= 0;
		}

		public async UniTask PlayForward()
		{
			foreach (var animeList in _animeLists)
			{
				List<UniTask> tasks = new List<UniTask>();
				foreach (var anime in animeList.Parallel)
				{
					anime.CreateTween(false, false);
					anime.tween.PlayForward();
					tasks.Add(anime.tween.AsyncWaitForCompletion().AsUniTask());
				}

				await UniTask.WhenAll(tasks);
			}
		}

		public async UniTask SmoothRewind()
		{
			var list = new List<AnimationList>(_animeLists);
			list.Reverse();
			foreach (var animeList in list)
			{
				List<UniTask> tasks = new List<UniTask>();
				foreach (var anime in animeList.Parallel)
				{
					anime.CreateTween(false, false);
					anime.tween.SmoothRewind();
					tasks.Add(anime.tween.AsyncWaitForRewind().AsUniTask());
				}

				await UniTask.WhenAll(tasks);
			}
		}
	}
}