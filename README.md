# easy-to-try-view(ETTView)
デバッグ指向のUnity汎用ゲーム基盤

## 導入方法
1. 導入したいUnityプロジェクトを立ち上げて、Window→PackageManager→AddPackageFromGitURLで下記  
https://github.com/CicholDax/easy-to-try-view.git?path=Assets/ETTView  
https://github.com/Cysharp/UniTask.git?path=src/UniTask/Assets/Plugins/UniTask  

## 機能概要
### Reopnable
非同期のライフサイクルイベントを提供します
各フェーズは同一のGameObjectにアタッチされた複数のReopnableがひとつのReopenerに取りまとめられて処理されます
### UIView
画面の構成単位です
UIViewは常にひとつのみがOpenになります
遷移が記憶され、Backメソッドによって前のViewに戻れます
### UIViewPopup
Viewに重ねて表示できるポップアップです
### UIViewState
Viewがとることができる状態です
Viewに属するStateはひとつのみがOpenになります

## 以下、DoTweenProを使用する場合
1. PackageManagerからDoTweenProを導入
2. ettview-dotweenpro.unitypackageをインポート
3. ScriptDefineSynbolsにUNITASK_DOTWEEN_SUPPORTを追加

## メモ
- パッケージキャッシュにシンボリックリンクを貼っちゃうと編集が楽だよ  
mklink /J com.cicholdax.ettview@95f66d72b9 C:\work\RexLabo\easy-to-try-view\Assets\ETTView
