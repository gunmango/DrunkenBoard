using UnityEngine;
using UnityEngine.UI;

public class TitleSceneManager : ASceneManager<TitleSceneManager>
{
    [SerializeField] private Button demoStartButton;
    protected override void Start()
    {
        base.Start();
        demoStartButton.onClick.AddListener(()=>GameManager.SceneController.LoadScene(ESceneType.MainGame));
    }
}
