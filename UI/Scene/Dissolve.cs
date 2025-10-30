using UnityEngine;
using UnityEngine.UI;

public class Dissolve : MonoBehaviour
{
    private RawImage screenImage;

    private float ChangeSpeed = 2f;

    private void Start()
    {
        screenImage = GetComponent<RawImage>();
        screenImage.texture = ScreenManager.Instance.ScreenTexture;

        if(screenImage.texture == null)
        {
            gameObject.SetActive(false);
        }

        screenImage.color = Color.white;
    }

    private void Update()
    {
        screenImage.color = Color.Lerp(screenImage.color, new Color(1, 1, 1, 0),ChangeSpeed * Time.deltaTime);
        if(screenImage.color.a <= 0.01f)
        {
            gameObject.SetActive(false);
        }
    }
}
