#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

public class AutoSaveWindow : EditorWindow
{
    private float idleTime = 0f; // マウスが動いていない時間
    private float idleThreshold = 60f; // 保存までの閾値（秒）
    private Vector3 lastMousePosition; // 前回のマウス位置
    private bool isAutoSaveEnabled = true; // 自動保存有効/無効（デフォルト: 有効）
    private float lastCheckTime = 0f; // 前回のチェック時刻

    [MenuItem("ペペロンチーノ/AutoSave")]
    public static void ShowWindow()
    {
        var window = GetWindow<AutoSaveWindow>("AutoSave Settings");
        window.minSize = new Vector2(300, 200);
    }

    private void OnEnable()
    {
        // マウス位置の初期化
        lastMousePosition = Input.mousePosition;
        lastCheckTime = Time.realtimeSinceStartup;
    }

    private void OnGUI()
    {
        GUILayout.Label("AutoSave Settings", EditorStyles.boldLabel);

        // 自動保存の有効/無効
        isAutoSaveEnabled = EditorGUILayout.Toggle("Enable AutoSave", isAutoSaveEnabled);

        // 保存までの閾値設定
        idleThreshold = EditorGUILayout.FloatField("Idle Threshold (seconds)", idleThreshold);

        if (GUILayout.Button("Force Save Scene"))
        {
            SaveCurrentScene();
        }

        GUILayout.Space(10);
        EditorGUILayout.HelpBox("This tool saves the current scene when the mouse is idle for the specified threshold.", MessageType.Info);
    }

    private void Update()
    {
        if (isAutoSaveEnabled)
        {
            CheckMouseActivity();

            if (idleTime >= idleThreshold)
            {
                SaveCurrentScene();
                idleTime = 0f; // 保存後にタイマーをリセット
            }
        }
    }

    private void CheckMouseActivity()
    {
        float currentTime = Time.realtimeSinceStartup;
        float deltaTime = currentTime - lastCheckTime; // 実際の経過時間を計測
        lastCheckTime = currentTime;

        if (Input.mousePosition != lastMousePosition)
        {
            idleTime = 0f; // マウスが動いたらタイマーをリセット
            lastMousePosition = Input.mousePosition;
        }
        else
        {
            idleTime += deltaTime; // 動いていない時間を正確に計測
        }
    }

    private void SaveCurrentScene()
    {
        string scenePath = EditorSceneManager.GetActiveScene().path;

        if (!string.IsNullOrEmpty(scenePath))
        {
            EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene());
            Debug.Log($"Scene saved: {scenePath}");
        }
        else
        {
            Debug.LogWarning("Scene path is invalid. Unable to save.");
        }
    }
}
#endif
