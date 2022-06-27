# easy-to-try-view(ETTView)
デバッグ指向のUnity汎用ゲーム基盤

### 導入方法
1. 導入したいUnityプロジェクトを立ち上げて、Window→PackageManager→AddPackageFromGitURLで下記  
https://github.com/CicholDax/easy-to-try-view.git?path=Assets/ETTView  
https://github.com/Cysharp/UniTask.git?path=src/UniTask/Assets/Plugins/UniTask  

#### 以下、DoTweenProを使用する場合

1. PackageManagerからDoTweenProを導入
2. ettview-dotweenpro.unitypackageをインポート
3. ScriptDefineSynbolsにUNITASK_DOTWEEN_SUPPORTを追加

### メモ
- 依存としてUniTaskがうまく追加できない
- パッケージキャッシュにシンボリックリンクを貼っちゃうと編集が楽だよ  
mklink /J com.rexlabo.ettview@0084c994dd C:\work\RexLabo\easy-to-try-view\Assets\ETTView
