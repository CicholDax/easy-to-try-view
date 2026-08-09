# ETTView サンプル刷新プラン：単一ショーケースへの集約

> 目的：散らかった複数サンプルを、ETTView のコア機能を **1 本のシーンで通しで見せる**「Showcase」に作り変える。
> 実行方法：**easy-to-try-view を開いた Unity Editor に MCP を接続**し、下記手順を編集スクリプト（`Unity_RunCommand`）で流してシーン／プレハブを生成する。手書き YAML は使わない（壊れやすいため）。

---

## 1. ゴールとスコープ

ETTView の中核を 1 シーンで体験できるショーケースを作る。デモ対象：

| 機能 | 見せ方 |
|---|---|
| `Reopener` ライフサイクル | 画面上に現在 Phase / History 深さをライブ表示 |
| `UIView`（常に1つだけOpen・履歴・Back） | Root → Detail への前進と Back（ESC/ボタン） |
| `UIViewPopup` | Root からメッセージポップアップを開く／閉じる |
| `UIViewState`（Viewの状態・状態履歴） | Root内で State A / B を切替＆State Back |
| forward / rewind トランジション | Detail 遷移時にコードtweenで前進・巻き戻しを出し分け |
| `CreateFromResources<T>` ワークフロー | Detail・Popup を Resources プレハブから生成 |

**非対象（別サンプルとして整理 or 削除、§7 で決定）**：Inertia（物理慣性・UI無関係）、Reenact（UserData保存/復元）。

---

## 2. 現状の問題（監査結果）

- **文字化け**：`Samples~/SampleView.cs`（`"�m�F"...`）、`ViewSample/ViewSampleView.cs` のコメント等。ファイルが壊れたエンコーディングで保存されている。
- **`async void Update()`**：`AView`/`BView`（毎フレーム前提で `LoadSceneAsync` を await）。アンチパターン。
- **旧 API**：`CommonPopup` が `UnityEngine.UI.Text`（TMP 未使用）、`AView`/`BView`/`CubeController` が旧 `Input.GetKeyDown`。
- **死にコード／空実装**：`ViewSampleView.Update`（コメントアウトのみ）、`CubeController.Opening`（base 呼ぶだけ）、未使用 `using System.Collections` 多数。
- **構成のバラつき**：`Samples~` 直下にルート散乱ファイル（`SampleScene.unity` / `SampleView.cs` / `ReenactDebug.*`）＋ `InertiaSample` / `ReenactSample` / `SampleGame` / `ViewSample` が混在。導線が不明瞭。
- **サマリコメント不足**：各スクリプトに `/// <summary>` が無く、サンプルとしての説明性が低い。

---

## 3. ターゲット構成（Before → After）

**方針：サンプルごとに独立サブフォルダ＋専用 asmdef に分割**し、Package Manager から個別 Import できる形にする。ルート一括の `Sample.asmdef` は廃止。

```
Samples~/                         ← 配布時は隠す（末尾~）。開発時のみ「Samples」にリネームして可視化（§3.5）
  Showcase/                       ★新規：メインショーケース（独立サンプル）
    Showcase.asmdef               ★新規：Showcase 専用アセンブリ（参照: ETTView / Unity.TextMeshPro / Unity.InputSystem / UniTask）
    ShowcaseScene.unity
    ShowcaseView.cs
    DetailView.cs
    MessagePopup.cs
    ShowcaseState.cs
    CodeScaleTransition.cs
    ShowcaseUiBuilder.cs          … UI生成ユーティリティ（コードでbutton/text組み立て）
    Resources/
      DetailView.prefab
      MessagePopup.prefab
  SampleGame/                     … Boot+Game の実戦例（独立サンプル）。残す
    SampleGame.asmdef             ★新規：SampleGame 専用アセンブリ（旧 Sample.asmdef 参照を移設）
    （既存の Boot/Game 一式）
  (削除) Sample.asmdef            … ルート一括asmdefは廃止。サンプルごとに分割
  (削除) SampleScene.unity / SampleView.cs（文字化け）
  (削除) ViewSample/*             … Showcase に統合
  (削除) ReenactDebug.unity / ReenactDebugController.cs … 重複デバッグ
  (削除) InertiaSample/*          … §7=案A：削除
  (削除) ReenactSample/*          … §7=案A：Showcaseへ吸収検討 or 削除
```

**`package.json` の `samples` はサンプル単位に分割**（個別 Import 可能化）。`path` は配布状態＝`Samples~/...` を指す：

```json
"samples": [
  { "displayName": "Showcase",
    "description": "View遷移/Back・Popup・State・コードトランジションを1シーンで通し見せ",
    "path": "Samples~/Showcase" },
  { "displayName": "Sample Game (Boot + Game)",
    "description": "Boot から Game を起動する最小実戦構成",
    "path": "Samples~/SampleGame" }
]
```

> なぜ asmdef をサンプルごとに：サブフォルダ単体を Import した際、ルートの asmdef は付いてこないため、専用 asmdef が無いと利用者の Assembly-CSharp にスクリプトが混ざる。独立 Import を成立させるには各サンプル配下に asmdef が要る。

## 3.5. 開発フロー：`Samples~` の `~` の扱い（重要）

末尾 `~` フォルダは **どのプロジェクトでも Unity のアセットパイプラインから無視される**（`Assets/` 配下でも `Packages/` 配下でも、パッケージ自身の開発プロジェクトでも同じ）。＝コンパイル・Play・エディタ編集が一切できない。一方、`~` は**公開パッケージで Import を正しく機能させる（opt-in・二重展開/asmdef衝突の回避）ために実質必須**。

そこで「**可視で開発 → 隠して配布**」で運用する：

1. **開発時**：`Assets/ETTView/Samples~` を **`Assets/ETTView/Samples`（`~`除去）にリネーム**して可視化。これでコンパイル・生成・Play 検証・エディタ編集が可能になる。
2. **リリース時**：`Samples` を **`Samples~` に戻す**（＝publish 手順）。`package.json` の `path` は常に `Samples~/...` 表記のまま（可視化中は Import ボタンが一時的に空振りするだけで問題なし）。

この運用は MCP 接続先が easy-to-try-view / kraken どちらでも同じ（`~` の可視/不可視が唯一の分岐点）。今回は **easy-to-try-view（ETTView本体）を開いて MCP 接続**し、`Samples`（可視）で作業する。

---

## 4. Showcase シーン設計（`ShowcaseScene.unity`）

ルート構成（builder で生成）：

- `Main Camera`
- `EventSystem`（`InputSystemUIInputModule` = 新Input System。ETTView が `Keyboard.current` 前提のため）
- `Canvas`（Screen Space - Overlay + `CanvasScaler` + `GraphicRaycaster`）
  - `ShowcaseView`（`UIView`, **`_isSceneTopView = true`**）
    - 見出し / 説明テキスト（TMP）
    - ライブ状態表示テキスト（Phase / History 数 / CurrentState）
    - ボタン群：
      - **Open Detail →** … `DetailView` を `CreateFromResources` して開く（前進遷移）
      - **Show Popup** … `MessagePopup.Create(...)`（ポップアップ）
      - **State A / State B** … `UIViewState` を切替（状態履歴）
    - `StateA` / `StateB`（子 GameObject、各 `UIViewState` + 見分け用パネル色）

UI は **`ShowcaseUiBuilder` のコードで生成**（TMP text / Button を動的作成）してプレハブ内の細かい配線を最小化。ただし Detail と Popup は「Resources から生成」ワークフローを見せるためプレハブ化する。

### プレハブ
- `Resources/DetailView.prefab`：`DetailView`(`UIView`) + 「← Back」ボタン（`BackForget`）。`CodeScaleTransition` を `_forwardTransitions` に登録し前進/巻き戻しの出し分けを見せる。
- `Resources/MessagePopup.prefab`：`MessagePopup`(`UIViewPopup`) + タイトル/本文(TMP) + OK(閉じる)。`CommonPopup` を置換（TMP化・文字化け解消）。

---

## 5. スクリプト仕様（新規・クリーン・要約コメント付き）

すべて `/// <summary>` 付与、未使用 using 除去、`async void Update` 不使用。

- **`ShowcaseView : UIView`**
  - `OnOpenDetail()`：`DetailView.Create(transform.parent)`（履歴に積まれ、現在Viewは自動Close）。
  - `OnShowPopup()`：`MessagePopup.Create(transform, "確認", "ポップアップ本文です")`。
  - `OnStateA()/OnStateB()`：対象 `UIViewState.Open()`（`Preopning` で `View.RegistState` される）。
  - `Update()`（同期のみ）：`Phase` / `History.Count()` / `CurrentState` をテキストへ反映。Back は `UIViewManager` が ESC を拾うので自前実装不要。
- **`DetailView : UIView`**
  - `public static UniTask<DetailView> Create(Transform parent) => CreateFromResources<DetailView>(parent);`
  - `OnBack()` → `BackForget()`（前のViewへ）。ESC でも戻れる（`IsBackInput` 既定）。
- **`MessagePopup : UIViewPopup`**
  - `CommonPopup` の後継。`Create(Transform, string title, string body)`。`_title/_body` は TMP。
- **`ShowcaseState : UIViewState`**
  - 見た目切替のみの薄いラッパ。必要なら開閉時にラベル更新。
- **`CodeScaleTransition : Reopenable`**
  - DOTween/Animator 不要のスケール tween。`Opening` で 0.85→1、`Closing` で 1→0.9 + フェード。Kraken 側 `UITween` と同型（`unscaledDeltaTime`、ease-out-back）。ライブラリ内蔵の `AnimatorTransition` に対する「コードだけ版」の実例。
- **`ShowcaseUiBuilder`（static）**
  - `Text(parent, ...)` / `Button(parent, label, onClick)` などの生成ヘルパ。builder スクリプトからも実行時からも使える。

---

## 6. ビルド手順（MCP 接続後に実行）

> 前提：easy-to-try-view を開いた Unity に MCP 接続済み。各ステップは `Unity_RunCommand` の `IRunCommand` スクリプトで実行し、`result.RegisterObjectCreation` 等で登録する。

1. **スクリプト配置**：§5 の .cs を `Samples~/Showcase/` に作成（Write）。`Sample.asmdef` の対象内に入るので参照解決を確認（`Refresh` → console error 0）。
2. **Resources プレハブ生成**（RunCommand）：
   - 空 GameObject に各コンポーネント（`UIView`/`UIViewPopup` 等 + `Reopener` は RequireComponent で自動、`CanvasGroup` + 既存 `CanvasGroupReflector`）を付与し、`ShowcaseUiBuilder` で子UIを生成 → `PrefabUtility.SaveAsPrefabAsset` で `Samples~/Showcase/Resources/*.prefab` へ保存 → シーンの仮objは破棄。
3. **ShowcaseScene 生成**（RunCommand）：
   - `EditorSceneManager.NewScene` → Camera/EventSystem/Canvas/`ShowcaseView` を構築、State A/B 子を付与、ボタン `onClick` を `ShowcaseView` のメソッドへ `UnityEventTools.AddPersistentListener` で配線 → `EditorSceneManager.SaveScene` で `Samples~/Showcase/ShowcaseScene.unity` 保存。
4. **Build Settings**：`ShowcaseScene` を index 0 に（`UIViewManager` が index0 起動を前提にするため）。SampleGame 用 Boot は別途。
5. **旧サンプル削除**：§3 の (削除) 群を `AssetDatabase.DeleteAsset`。
6. **検証**：Play して Detail 前進→ESC で Back、Popup 開閉、State A/B 切替、状態表示が動くことを目視。console error 0。

> シーン/プレハブ生成を「実行時コード生成」で完結させる案も可（プレハブ0個）。その場合 `CreateFromResources` デモは Detail だけ残す等、要調整。まずは上記のプレハブ併用案を基本線とする。

---

## 7. 要決定事項（実行前に確認したい）

1. **Inertia / Reenact サンプルの扱い**
   - 案A（推奨）：UI ショーケースに集中。Reenact は Showcase に「保存/復元」ボタンとして吸収、Inertia は削除。
   - 案B：Reenact / Inertia は独立サンプルとして名前と導線だけ整えて残す。
2. **トランジション実装**：`CodeScaleTransition`（依存なし・推奨）／ 既存 `AnimatorTransition`（要 Animator 設定）／ DOTween 版（`ETTView - DoTweenPro`）のどれを主に見せるか。
3. **SampleGame（Boot+Game）**：残す想定（実戦例として有用）。不要なら削除。

---

## 8. 片付け（品質修正の明細）

- 文字化けファイルは新規スクリプトで置換（旧 `SampleView`/`ViewSample*`/`CommonPopup` は削除）。
- `async void Update` は廃止（Back はマネージャ任せ、遷移は button コールバックから `Forget()`）。
- 旧 `Text`→`TextMeshProUGUI`、旧 `Input`→ 使用箇所自体を撤去（ESC は `IsBackInput` に集約）。
- 全新規スクリプトに `/// <summary>`、フィールドは `_` プレフィクス、未使用 using 除去。

---

### 参考（確認済みの実 API）
- 前進：新 View を `CreateFromResources`/生成 → `Preopning` で `UIViewManager.Regist` が走り他Viewを自動Close。
- 戻る：`BackForget()` / `UIViewManager.Back()`、既定は ESC（`UIView.IsBackInput`）。`BackToTargetView<T>()` で指定Viewまで戻る。
- 状態：`UIViewState.Preopning` が `View.RegistState(this)`。戻りは `Back` が `TryBackState` を呼ぶ。
- ポップアップ：`UIViewPopup.Opening` が `RegistPopup`、`Back` は最後のPopupから閉じる。
- 破棄条件：`_isSceneTopView` or `_fromPrefab` の View は Close 時 Destroy（`IsDestroyWhenClosed`）。

---

## 9. 進捗と残作業（MCP 再接続後に再開する用）

> 更新日：2026-08-10（追記）。**§6 の生成・検証まで完了**。easy-to-try-view を開いた Unity に MCP 接続し、シーン／プレハブ生成・旧サンプル削除・サンプル分割・Play 検証を実施済み（console error 0）。残るは §手順7（`Samples~` へ戻す publish 手順）と git コミットのみ（ユーザー確認待ち）。

### 9-0. 今回完了した内容（2026-08-10 セッション）

- **可視化**：`Samples~` の中身を可視 `Samples/` へ移動（`~` 除去）。Showcase 6 スクリプトのロード確認。
- **asmdef 分割**：`Showcase/Showcase.asmdef`・`SampleGame/SampleGame.asmdef` を新設、ルート `Sample.asmdef` を削除。両アセンブリ compile OK。
- **旧サンプル削除（案A）**：`ViewSample` / `InertiaSample` / `ReenactSample` / `ReenactDebug.*` / ルート `SampleScene.unity`・`SampleView.cs` / ルート `Resources`（Cube）を削除。※すべて git 追跡下（`Samples~/...` 旧パス）で復元可能。
- **TMP Essentials 導入**：この開発プロジェクトに未導入だったため `TMP_PackageResourceImporter.ImportResources(true,false,false)` で導入（`Assets/TextMesh Pro/` 追加、`LiberationSans SDF` 既定フォント）。※配布パッケージ（`Assets/ETTView`）外なので配布物には含まれない。
- **プレハブ生成**：`Resources/MessagePopup.prefab`（`_title`/`_body` TMP・OK→`OnClickClose`）、`Resources/DetailView.prefab`（`CodeScaleTransition` を `_forwardTransitions` に登録・Back→`OnClickBack`）。
- **シーン生成**：`ShowcaseScene.unity`（Camera / EventSystem(`InputSystemUIInputModule`) / Canvas / `ShowcaseView`(`_isSceneTopView=true`) / 状態表示テキスト / 4 ボタン配線 / `StateA`・`StateB` 子（初期 Closed・alpha0））。Build Settings index0 に設定。
- **Play 検証（目視・スクショ）**：Root→Detail 前進（`CreateFromResources`＋スケール遷移）→Back、Popup 開閉、State A 切替（状態表示更新）まで動作確認。console error 0。
- **英語表記へ統一**：既定フォントに CJK グリフが無く日本語が tofu 表示になるため、**表示テキストを英語化**（`ShowcaseView.cs` の Popup 文言、Scene / DetailView の説明文）。コード内のコメント／`<summary>` は日本語のまま。
- **package.json**：`samples` を Showcase / SampleGame の 2 エントリに分割（`path` は `Samples~/...` 表記）。

### 9-0-1. 未実施（ユーザー確認待ち）

- **§手順7 publish**：`Assets/ETTView/Samples` → `Samples~` へリネームして隠す（＋ローカル Build Settings の ShowcaseScene 参照を外す）。※隠すと Unity から不可視になり以降の Play 検証不可。
- **git コミット**：生成物（Showcase 一式・asmdef・package.json）・旧サンプル削除をまとめてコミット。

> 更新日：2026-08-10（旧メモ）。コード側（§5）は完了。シーン／プレハブ生成（§6）・旧サンプル削除・サンプル分割（§3）は Unity Editor への MCP 接続が前提のため未実施。手書き YAML は使わない方針は継続。

> **判明した重要事実（2026-08-10 の調査）**
> - `Samples~` は末尾 `~` のため **どのプロジェクトでも Unity から不可視**（コンパイル/生成/Play/編集 不可）。→ 検証には §3.5 の「可視化（`Samples` にリネーム）」が必須。
> - 前回セッションの MCP 接続先は **easy-to-try-view ではなく kraken** だった（`C:/work/kraken`）。kraken は ETTView を `file:../../easy-to-try-view/Assets/ETTView` のローカル参照＝`Packages/com.cicholdax.ettview` として利用。今回は **easy-to-try-view 本体を開いて接続**する。
> - Unity エディタが**非アクティブ（バックグラウンド）だと外部追加ファイルの取り込み/コンパイルが走らない**（`AssetDatabase.Refresh()` / メニュー Refresh も空振り）。作業中は Unity ウィンドウを前面（アクティブ）に保つこと。

### 9-1. 完了済み（コミット前・作業ツリーにあり）

- 新規スクリプト（`Assets/ETTView/Samples~/Showcase/`、`.cs` + `.cs.meta`）
  - `ShowcaseView.cs`（guid `d1a27a47fb9e4580a535281471e83f2e`）
  - `DetailView.cs`（guid `0a0ae0dbe67b471fb3304a00f31c9178`）
  - `MessagePopup.cs`（guid `213d41b9965a4a6b9f555b13c3d0d24a`）
  - `ShowcaseState.cs`（guid `f6f0c5ea17f6453da066b0f5199e8c72`）
  - `CodeScaleTransition.cs`（guid `67bca585422840f3a108f7802bc1fb35`）
  - `ShowcaseUiBuilder.cs`（guid `ef0e41d12f2a45738bb9ed5d6cc975a7`）
  - `Showcase.meta`（フォルダ guid `796fdf1aa90b476eb4a5ff3b0c9f6cda`）
- `Sample.asmdef`（ルート一括）に `Unity.TextMeshPro` / `Unity.InputSystem` 参照を追加済み。**ただし新方針ではこの一括 asmdef は廃止し、サンプルごとの asmdef に分割する（§3）。** → 残作業で `Showcase.asmdef` / `SampleGame.asmdef` を新設し、ルート `Sample.asmdef` は削除する。
- `package.json` の samples は現状 `path: "Samples~"` の1エントリ。**新方針では Showcase / SampleGame の個別エントリに分割する（§3）。**
- §7 の方針：案A（UIショーケース集中）／`CodeScaleTransition` 主軸／SampleGame 残す、でコードを用意。**削除はまだ一切していない**。

> 注意：コード側の実装差分により、当初プランから以下を調整済み。
> - `ShowcaseView.Update` は `internal` メンバ（`CurrentState`/`LastPopup`）が別アセンブリから触れないため、公開 API（`StateHistory`/`History`/`ExistPopup()`）で状態表示を実装。
> - 各スクリプトの namespace は `ETTView.Samples.Showcase`。`ShowcaseState : UIViewState`（UIViewState はグローバル名前空間）。
> - `ShowcaseView` のボタン用公開メソッド：`OnOpenDetail()` / `OnShowPopup()` / `OnStateA()` / `OnStateB()`。
> - `DetailView.OnClickBack()`、`MessagePopup.OnClickClose()` が Back 用。

### 9-2. 残作業（easy-to-try-view を開いて MCP 接続後に実行）

> 生成系はすべて `Unity_RunCommand`（`IRunCommand`）で実行。**作業中は Unity ウィンドウを前面（アクティブ）に保つ**（非アクティブだと取り込みが走らない）。開発中のパスは可視化後の `Assets/ETTView/Samples/...`（`~` 無し）。

**手順0：可視化（`~` 除去）＋接続確認**
- `Assets/ETTView/Samples~` を **`Assets/ETTView/Samples` にリネーム**（§3.5）。git 未追跡なのでファイルシステム移動でよい。
- `AssetDatabase.Refresh()` → `IsValidFolder("Assets/ETTView/Samples/Showcase")=true`、Showcase 6スクリプトの型がロードされ、console error 0 を確認。

**手順0.5：サンプル分割（asmdef＋package.json）**
- `Assets/ETTView/Samples/Showcase/Showcase.asmdef` を新設（参照: `ETTView` / `Unity.TextMeshPro` / `Unity.InputSystem` / UniTask GUID）。
- `Assets/ETTView/Samples/SampleGame/SampleGame.asmdef` を新設（旧 `Sample.asmdef` の参照を移設）。
- ルート `Sample.asmdef` を削除。
- `package.json` の `samples` を Showcase / SampleGame の個別エントリに分割（§3 の JSON）。`path` は `Samples~/...` 表記のまま。
- Refresh → 各サンプルが自前アセンブリでコンパイルされ error 0 を確認。

**手順1：`Showcase/Resources/MessagePopup.prefab` 生成**
- 空 GameObject に `MessagePopup`（`UIViewPopup`）+ `Reopener`(RequireComponentで自動) + `CanvasGroup` + `CanvasGroupReflector` を付与。
- `ShowcaseUiBuilder.Text` で `_title` / `_body`（TMP）を生成し `MessagePopup` の SerializeField に代入。OK ボタンを `ShowcaseUiBuilder.Button` で作り `onClick` を `OnClickClose` に配線（`UnityEventTools.AddPersistentListener`）。
- `PrefabUtility.SaveAsPrefabAsset` で `Assets/ETTView/Samples/Showcase/Resources/MessagePopup.prefab` に保存 → 仮 obj 破棄。

**手順2：`Showcase/Resources/DetailView.prefab` 生成**
- 空 GameObject に `DetailView`（`UIView`）+ `Reopener` + `CanvasGroup` + `CanvasGroupReflector` + `CodeScaleTransition` を付与。
- `CodeScaleTransition` を `DetailView._forwardTransitions` に登録（前進/巻き戻しの出し分けを見せる）。
- 「← Back」ボタンを生成し `onClick` を `OnClickBack` に配線。見出しテキスト（TMP）も任意で配置。
- `Assets/ETTView/Samples/Showcase/Resources/DetailView.prefab` に保存。

**手順3：`Showcase/ShowcaseScene.unity` 生成**
- `EditorSceneManager.NewScene` で新規シーン。
- `Main Camera`、`EventSystem`（`InputSystemUIInputModule`）、`Canvas`（Screen Space-Overlay + `CanvasScaler` + `GraphicRaycaster`）を構築。
- `Canvas` 下に `ShowcaseView`（`UIView`, **`_isSceneTopView = true`** + `CanvasGroup` + `CanvasGroupReflector`）を配置。
  - 見出し／説明テキスト、ライブ状態表示テキスト（`ShowcaseView._statusText` に代入）を `ShowcaseUiBuilder` で生成。
  - ボタン群：Open Detail→`OnOpenDetail` / Show Popup→`OnShowPopup` / State A→`OnStateA` / State B→`OnStateB` を `AddPersistentListener` で配線。
  - 子 `StateA` / `StateB`（各 `ShowcaseState`(`UIViewState`) + 見分け用パネル色 + Label 設定）を配置し、`ShowcaseView._stateA` / `_stateB` に代入。State は初期 Closed にするため各 `Reopener.enabled=false`。
- TMP フォントは配布耐性のため **TMP パッケージ既定フォント**を参照（利用者Assets依存の Essentials guid を避ける）。
- `Assets/ETTView/Samples/Showcase/ShowcaseScene.unity` に `EditorSceneManager.SaveScene`。

**手順4：Build Settings**
- `ShowcaseScene` を index 0 に設定（`UIViewManager` が index0 起動前提）。※これは**検証用のローカル設定**。配布物には含めない（利用者は Import 後に自分で追加）。

**手順5：検証（Play）**
- Detail 前進 → ESC / ← Back で戻る、Popup 開閉、State A/B 切替＆State Back、状態表示テキストが更新される、console error 0 を目視（`Unity_Camera_Capture` でスクショ確認）。

**手順6：旧サンプル削除（`AssetDatabase.DeleteAsset`）** ※非可逆、実行前に最終確認
- `Samples/SampleScene.unity` / `SampleView.cs`（文字化け）、`Samples/ViewSample/*`、`Samples/ReenactDebug.unity` / `ReenactDebugController.cs`、`Samples/InertiaSample/*`、（案A）`Samples/ReenactSample/*`。各 `.meta` も同時に。
- ルート `Resources/` が Showcase から不要なら整理。

**手順7：`Samples~` へ戻す（publish 手順）**
- 検証・整理が済んだら `Assets/ETTView/Samples` を **`Assets/ETTView/Samples~` にリネーム**して隠す。
- ローカル Build Settings の ShowcaseScene 参照は外す（配布物に残さない）。
- git にコミット（生成プレハブ/シーン + asmdef + package.json 分割 + 旧サンプル削除）。
