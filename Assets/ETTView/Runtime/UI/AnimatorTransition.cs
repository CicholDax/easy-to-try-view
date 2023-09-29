using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;

namespace ETTView.UI
{
	public class AnimatorTransition : Reopnable
	{
		[SerializeField] Animator _animator;
		[SerializeField] string _openStateName = "open";
		[SerializeField] string _closeStateName = "close";
		[SerializeField] float _fadeDuration = 1;

		public override async UniTask Opening(CancellationToken token)
		{
			await base.Opening(token);

			_animator.speed = 1;
			_animator.CrossFade(_openStateName, _fadeDuration);

			await UniTask.WaitUntil(() => _animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1);
		}

		public override async UniTask Closing()
		{
			await base.Closing();

			_animator.speed = 1;
			_animator.CrossFade(_closeStateName, _fadeDuration);

			await UniTask.WaitUntil(() => _animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1);
		}
	}
}