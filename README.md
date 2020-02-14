# ADX2 for Unity Recorder
ADX2 for Unityで再生中の音を録音するエディタ拡張

![ADX2UnityRecoder](ADX2UnityRecoder.png)

## 動作確認環境
Unity 2019.3.1f1 + ADX2 LE SDK 2.10.05

Unity 2018.4.13f1 + ADX2 (CRIWARE SDK for Unity 3.00.04)

## 使用方法
ゲームを再生開始し、任意のタイミングで「Start Recording」ボタンを押してください。

## Unity Recorderとの連携方法
1. CriAtomRecorderEditorWindow.csのOnEnableメソッドのコメントアウトを外す
2. CriAtomRecorderウィンドウを開き直す
3. Unity Recorderも一緒に起動するのでゲームを再生開始する
4. 任意のタイミングでUnity Recorder側の録画開始ボタンを押す
5. 任意のタイミングでUnity Recorder側の録画停止ボタンを押す
