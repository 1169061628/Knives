using UnityEngine;
using UnityEngine.UI;

public class GameStart : MonoBehaviour
{
    GameObject canvas;
    GameObject content, levelSelect;
    GameScene scene;
    // Start is called before the first frame update
    void Start()
    {
        ResManager.InitALlResPath();
        canvas = GameObject.Find("Canvas");
        levelSelect = Util.GetGameObject(canvas, "levelSelect");
        content = Util.GetGameObject(levelSelect, "content");
        var pre = Util.GetGameObject(content, "pre");
        for (int i = 1; i <= 80; i++)
        {
            int iii = i;
            var go = Util.NewObjToParent(pre, content);
            go.name = "item" + i;
            Util.GetComponent<Text>(go, "index").text = i.ToString();
            go.GetComponent<Button>().onClick.AddListener(() =>
            {
                if (scene == null)
                {
                    scene = new();
                    scene.Init();
                }
                levelSelect.SetActive(false);
                Debug.LogError($"进战斗关卡{iii}");
            });
        }
    }

    // Update is called once per frame
    void Update()
    {
        scene?.Update();
    }

    void LateUpdate()
    {
        scene?.LateUpdate();
    }
    void FixedUpdate()
    {
        scene?.FixedUpdate();
    }
}
