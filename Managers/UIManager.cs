using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    private Stack<UIPopup> openPopups = new Stack<UIPopup>();
    private Queue<UIPopup> pendingPopups = new Queue<UIPopup>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            CloseLastOpenedPopup();
        }
    }

    public void OpenPopup(UIPopup popup)
    {
        if (popup != null)
        {
            popup.Open();
            openPopups.Push(popup);
        }
    }

    public void ClosePopup(UIPopup popup)
    {
        if (popup != null && openPopups.Contains(popup))
        {
            popup.Close();
            openPopups.Pop();

            if(pendingPopups.Count > 0)
            {
                OpenPopup(pendingPopups.Dequeue());
            }
        }
    }

    public void OpenPopupWithFade(UIPopup popup)
    {
        popup.FadePopupInAndOut();
    }

    private void CloseLastOpenedPopup()
    {
        if(openPopups.Count > 0)
        {
            ClosePopup(openPopups.Peek());
        }
    }

    public void CloseAllOpenPopups()
    {
        while(openPopups.Count > 0)
        {
            ClosePopup(openPopups.Peek());
        }
    }

    public void ReservePopup(UIPopup popup)
    {
        if(popup != null)
        {
            pendingPopups.Enqueue(popup);
        }
    }
}
