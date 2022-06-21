# easy-to-try-view(ETTView)
デバッグ指向のUnity汎用ゲーム基盤

### 導入方法
1. Windows PowerShellから下記でGitHubCliをインストール  
winget install --id GitHub.cli

2. コマンドプロンプトから下記コマンドでGitアカウントにログイン  
gh auth login  
もしかしたらSSHでログインする必要があるかも

3. 導入したいUnityプロジェクトを立ち上げて、Window→PackageManager→AddPackageFromGitURLで下記  
https://github.com/RexLabo/easy-to-try-view.git?path=Assets/ETTView  
https://github.com/Cysharp/UniTask.git?path=src/UniTask/Assets/Plugins/UniTask  

4. ScriptDefineSynbolsにUNITASK_DOTWEEN_SUPPORTを追加

#### 以下、DoTweenProを使用する場合

1. PackageManagerからDoTweenProを導入
2. ettview-dotweenpro.unitypackageをインポート

### メモ
- 依存としてUniTaskがうまく追加できない
- パッケージキャッシュにシンボリックリンクを貼っちゃうと編集が楽だよ
mklink /J com.rexlabo.ettview@0084c994dd C:\work\RexLabo\easy-to-try-view\Assets\ETTView
