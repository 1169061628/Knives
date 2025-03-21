using UnityEngine;
using UnityEngine.UI;

public class GameStart : MonoBehaviour
{
    GameObject canvas;
    GameObject content, levelSelect;
    GamePanel panel = null;
    // Start is called before the first frame update
    void Start()
    {
        ResManager.InitALlResPath();
        canvas = GameObject.Find("Canvas");
        levelSelect = Util.GetGameObject(canvas, "levelSelect");
        content = Util.GetGameObject(levelSelect, "content");

        panel ??= new(Util.GetGameObject(canvas, "GamePanel"));
        panel.Start();
        var pre = Util.GetGameObject(content, "pre");
        for (int i = 1; i <= 80; i++)
        {
            int iii = i;
            var go = Util.NewObjToParent(pre, content);
            go.name = "item" + i;
            Util.GetComponent<Text>(go, "index").text = i.ToString();
            go.GetComponent<Button>().onClick.AddListener(() =>
            {
                levelSelect.SetActive(false);
                Debug.LogError($"进战斗关卡{iii}");


                panel.OnOpen(iii);
            });
        }
    }

    // Update is called once per frame
    void Update()
    {
        panel?.Update();
    }

    void LateUpdate()
    {
        panel?.LateUpdate();
    }
    void FixedUpdate()
    {
        panel?.FixedUpdate();
    }
}
