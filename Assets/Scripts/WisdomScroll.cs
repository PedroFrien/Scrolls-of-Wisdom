using UnityEngine;
using UnityEngine.UI;

public class WisdomScroll : MonoBehaviour, IInteractable
{
    [SerializeField] Sprite ScrollImage;

    private GameObject ScrollUI;
    private Image WisdomImage;

    private void Start()
    {
        GameObject mainCanvas = GameObject.Find("PlayerUI");
        ScrollUI = mainCanvas.transform.Find("ScrollUI").gameObject;
        WisdomImage = ScrollUI.transform.Find("WisdomImage").gameObject.GetComponent<Image>();
    }


    public void OnInteract()
    {
        ScrollUI.SetActive(true);
        WisdomImage.sprite = ScrollImage;

        GameManager.Instance.SetCursorActive(true);
    }
}
