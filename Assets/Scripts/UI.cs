using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI : MonoBehaviour
{
    public static UI singleton;

    [SerializeField] public GameObject fallUI;
    [SerializeField] public TextMeshProUGUI fallTimerText;

    [SerializeField] public GameObject sunUI;
    [SerializeField] public Image crosshair;
    
    void Start()
    {
        singleton = this;

        fallUI.SetActive(false);
    }
}
