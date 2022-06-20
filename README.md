# uni-dax
Unityプロダクトの汎用ゲーム基盤

### 導入方法
1. Windows PowerShellから下記でGitHubCliをインストール  
winget install --id GitHub.cli

2. コマンドプロンプトから下記コマンドでGitアカウントにログイン  
gh auth login

3. 導入したいUnityプロジェクトを立ち上げて、Window→PackageManager→AddPackageFromGitURLで下記  
https://github.com/CicholDax/uni-dax.git?path=Assets/UniDax

### メモ
DoTweenProが必要  
DoTweenProがUPM対応したらpackage.jsonに依存として追加する
https://github.com/Demigiant/dotween/issues/395
