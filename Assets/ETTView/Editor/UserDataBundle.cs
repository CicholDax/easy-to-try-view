using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using ETTView.Data;

namespace ETTView.Editor
{
	[CreateAssetMenu(fileName = "UserDataBundle", menuName = "Data/UserDataBundle")]
	public class UserDataBundle : ScriptableObject
	{
		//外部に依存しちゃってるので治す TODO
		//[SerializeField] UserCommonData _userCommonData;
		//[SerializeField] UserLevelData _userLevelData;
		//[SerializeField] UserGameData _userGameData;

	
		[SerializeField] List<UserData> _userDatas;

		[SettingsProvider]
		public static SettingsProvider CreatProvider()
		{
			var provider = new SettingsProvider("Project/UserDataBundle", SettingsScope.Project)
			{
				label = "ユーザーデータ書き換え",
				guiHandler = searchContext =>
				{
					UserDataBundle bundle = CreateInstance<UserDataBundle>();
					//bundle._userCommonData = UserData.LoadFromPrefs<UserCommonData>();
					//bundle._userLevelData = UserData.LoadFromPrefs<UserLevelData>();
					//bundle._userGameData = UserData.LoadFromPrefs<UserGameData>();

					var so = new SerializedObject(bundle);

					//SetializedObjectのイテレータを取得
					var iter = so.GetIterator();
					//最初の一個はスクリプトへの参照なのでスキップ
					iter.NextVisible(true);
					//残りのプロパティをすべて描画する
					while (iter.NextVisible(false))
					{
						// 描画
						EditorGUILayout.PropertyField(iter, true);
					}
					so.ApplyModifiedProperties();

					if (GUILayout.Button("初期化"))
					{
						//bundle._userCommonData = new UserCommonData();
						//bundle._userLevelData = new UserLevelData();
						//bundle._userGameData = new UserGameData();
					}

					//保存
					//bundle._userCommonData.SaveToPrefs();
					//bundle._userGameData.SaveToPrefs();
					//bundle._userLevelData.SaveToPrefs();
				}
			};
			return provider;
		}
	}
}