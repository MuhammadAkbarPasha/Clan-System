using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class ButtonSceneLoader : MonoBehaviour
{
    [SerializeField] private string sceneName;

    private Button button;

    private void Start()
    {
        if(button == null)
            button = GetComponent<Button>();

        button.onClick.AddListener(()=> LoadScene());
    }

    private void OnDestroy()
    {
        button.onClick.RemoveAllListeners();
    }

    public void LoadScene()
    {
        if (FakePlayerHandler.Instance.PlayerData == null)
        {
            Debug.LogWarning("Player data is empty! Can't continue with testing...");
            return;
        }

        AsyncOperation asyncOp = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);
        Debug.Log("Begin loading scene: " + sceneName);

        System.Action<AsyncOperation> callback = null;
        callback = (op) =>
        {
            Debug.Log("Successfully loaded scene:" + sceneName);
            asyncOp.completed -= callback;
        };

        asyncOp.completed += callback;
    }
}
