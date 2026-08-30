# easy-to-try-view (ETTView)

デバッグしやすい Unity 向けのビュー基盤。非同期ライフサイクルと履歴付きの画面管理を中心に、ビュー・ポップアップ・ステートを素早く組み合わせて検証できます。

## 必要パッケージ
- [Cysharp/UniTask](https://github.com/Cysharp/UniTask) — すべての非同期処理に利用します。

## 導入方法
1. Unity の `Window > Package Manager > Add package from git URL...` から以下を追加。
   - `https://github.com/CicholDax/easy-to-try-view.git?path=Assets/ETTView`
   - `https://github.com/Cysharp/UniTask.git?path=src/UniTask/Assets/Plugins/UniTask`
2. DoTween Pro を併用する場合は、`ettview-dotweenpro.unitypackage` をインポートし、`Player Settings > Script Define Symbols` に `UNITASK_DOTWEEN_SUPPORT` を追加してください。

## コアコンセプト
### 非同期ライフサイクル (Reopenable / Reopener)
- `Reopenable` は `Loading → Preopening → Opening → Opened → Closing → Closed` のフェーズを `UniTask` で処理する抽象基底クラスです。【F:Assets/ETTView/Runtime/Reopenable.cs†L7-L94】
- 実際のフェーズ進行は `Reopener` が担当し、同一 GameObject 上の複数 `Reopenable` を並列実行でまとめます。`Open/Close` はフェーズ完了まで待機するので、呼び出し側は安定した状態を前提にできます。【F:Assets/ETTView/Runtime/Reopener.cs†L17-L191】

### プレハブ連携 (ReopenablePrefab / BackableReopenablePrefab)
- `ReopenablePrefab` は `Resources` から同名のプレハブを生成し、クローズ時にプレハブ由来なら自動破棄できます。`CloseAndDestroyIfNeeded` で破棄を統一できます。【F:Assets/ETTView/Runtime/UI/ReopenablePrefab.cs†L1-L37】
- `BackableReopenablePrefab` は `UIViewManager.Back` をラップし、ビュー・ポップアップ・ステートから戻る操作を簡潔に呼び出せます。【F:Assets/ETTView/Runtime/UI/BackableReopenablePrefab.cs†L1-L23】

### 画面管理 (UIView / UIViewManager)
- `UIView` は単一アクティブな画面を表し、`UIViewManager` が履歴 `_history` を管理します。登録時に既存ビューをクローズして常に 1 つだけを Open に保ちます。【F:Assets/ETTView/Runtime/UI/UIView.cs†L12-L137】【F:Assets/ETTView/Runtime/UI/UIViewManager.cs†L19-L123】
- `Back()` はポップアップ → ステート → ビューの順で戻り処理を試行し、履歴が 1 つなら RootView としてクローズを防ぎます。【F:Assets/ETTView/Runtime/UI/UIViewManager.cs†L144-L227】
- インデックス 0 のシーンを自動ロードし、起動済みビューを割り込ませて履歴を整える Editor 実行時の補助も用意されています。【F:Assets/ETTView/Runtime/UI/UIViewManager.cs†L49-L88】

### ステート管理 (UIViewState)
- `UIViewState` は同一 View 内の状態をスタック管理し、`AwaitCloseState` が `true` なら前ステートのクローズ完了を待ってから遷移します。【F:Assets/ETTView/Runtime/UI/UIViewState.cs†L9-L42】【F:Assets/ETTView/Runtime/UI/UIView.cs†L97-L219】

### ポップアップ (UIViewPopup)
- `UIViewPopup` は現在の View に登録され、`CanBackPopup` を条件に `Back()` 経由で最後に開いたポップアップをクローズできます。【F:Assets/ETTView/Runtime/UI/UIViewPopup.cs†L7-L18】【F:Assets/ETTView/Runtime/UI/UIView.cs†L65-L195】

## ユーティリティ
- **AnimatorTransition**: 開閉時に Animator のステートをクロスフェードし、再生完了まで待機します。【F:Assets/ETTView/Runtime/UI/AnimatorTransition.cs†L7-L33】
- **CanvasGroupReflector**: ビューのフェーズに合わせて `interactable` / `blocksRaycasts` を自動で切り替えます。【F:Assets/ETTView/Runtime/UI/CanvasGroupReflector.cs†L8-L28】
- **SafeArea**: `Screen.safeArea` とアスペクト比から余白を計算し、`RectTransform` のオフセットを調整します（横長 Android 端末への補正含む）。【F:Assets/ETTView/Runtime/UI/SafeArea.cs†L6-L82】

## よくある使い方のヒント
- ビュー切り替えは `UIView` を継承したプレハブを `Resources` に置き、`CreateFromResources<YourView>()`（`ReopenablePrefab` 継承メソッド）で生成して `Open()` するだけで履歴に登録されます。【F:Assets/ETTView/Runtime/UI/ReopenablePrefab.cs†L17-L37】【F:Assets/ETTView/Runtime/UI/UIView.cs†L154-L171】
- 戻る操作は Input の `Escape` をデフォルトにしつつ、`UIView.IsBackInput` をオーバーライドすれば端末固有キーにも対応できます。【F:Assets/ETTView/Runtime/UI/UIView.cs†L167-L195】
- 画面破棄とシーン管理は `_isSceneTopView` フラグで制御され、シーン直下のビューが閉じられた場合は所属シーンごとアンロードされます。【F:Assets/ETTView/Runtime/UI/UIView.cs†L17-L269】

## 開発メモ
- パッケージキャッシュにシンボリックリンクを作成すると編集が容易です（例: `mklink /J com.cicholdax.ettview@95f66d72b9 C:\work\RexLabo\easy-to-try-view\Assets\ETTView`）。
