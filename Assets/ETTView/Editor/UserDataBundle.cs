using System;
using System.Linq;
using System.Reflection;
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
		public class DummyUserData : UserData
		{
			[SerializeField,HideInInspector]string _name;
			public string Name { set { _name = value; } }
		}

		[SerializeReference] List<UserData> _userDatas = new List<UserData>();

		static Type[] _cashUserDataTypes;

		[SettingsProvider]
		public static SettingsProvider CreatProvider()
		{
			var provider = new SettingsProvider("Project/UserDataBundle", SettingsScope.Project)
			{
				label = "ETTView/ユーザーデータ書き換え",
				guiHandler = searchContext =>
				{
					//アセンブリで定義されている型をすべて取得してそこからUserDataを継承しているクラスを探す
					if (_cashUserDataTypes == null || _cashUserDataTypes.Length <= 0)
					{
						_cashUserDataTypes = AppDomain.CurrentDomain.GetAssemblies()
						.OrderBy(o => o.FullName)
						.SelectMany(o => o.GetTypes())
						.Where(o => o.IsPublic)
						.OrderBy(o => o.Name)
						.Where(o =>
						{
							return o.IsSubclassOf(typeof(UserData));
						})
						.ToArray();
					}

					UserDataBundle bundle = CreateInstance<UserDataBundle>();

					//PlayerPrefasからユーザーデータの呼び出し
					foreach (var userDataType in _cashUserDataTypes)
					{
						bundle._userDatas.Add(new DummyUserData() { Name = userDataType.Name });
						bundle._userDatas.Add(UserData.LoadFromPrefs(userDataType, ()=> { return Activator.CreateInstance(userDataType) as UserData; }));
					}

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
						bundle._userDatas = new List<UserData>();
					}

					if(GUILayout.Button("型キャッシュ削除（UserData型追加したら押してね）"))
					{
						_cashUserDataTypes = null;
					}

					//保存
					foreach (var userData in bundle._userDatas)
					{
						userData.SaveToPrefs();
					}
				}
			};
			return provider;
		}
	}
}