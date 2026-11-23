# 1151VR-HW5-414261383-陳博楷

## Unity 專案
- 建議 Unity Editor：2022.3 LTS（可使用相容的 2022.3 版本）
- 專案場景：`Assets/Scenes/MainScene.unity`
- 程式碼：`Assets/Scripts/`

## 執行方式
1. 將此資料夾解壓縮。
2. 在 Unity Hub 選擇 **Add / Open**，指定本資料夾。
3. 開啟 `Assets/Scenes/MainScene.unity`。
4. 按 Play。
5. 按 **Space** 或滑鼠左鍵，訓練家會原地做出舉手、拋球動作，精靈球沿拋物線飛向野生怪物並播放收服動畫。
6. 按 **R** 可重置場景。

## 場景內容
場景由 `WorldBuilder.cs` 在執行時自動建立，包含草地、樹木、遠方起伏山丘、固定位置的訓練家、野生 3D 生物、精靈球、主攝影機與方向光。所有模型均以 Unity 內建 3D primitive 組合，不需額外下載素材。

## GitHub 繳交
- Repository 名稱請使用：`1151VR-HW5-414261383-陳博楷`
- Repository visibility 設為 **Public**
- 根目錄已附 `.gitignore`，會排除 Unity Library、Temp、Build 等不應上傳的快取資料夾。
- 上傳前請先在 Unity 開啟並測試一次，再將整個專案資料夾推送至 GitHub。
