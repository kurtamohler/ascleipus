using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.EventSystems;  
using UnityEngine;

public class StartButtonController : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public GameObject sceneLight;

    // Start is called before the first frame update
    void Start()
    {
        // sceneLight = GameObject.Find("DefaultDirectionalLight");
        sceneLight.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        sceneLight.SetActive(true);
    }
    
    public void OnPointerExit(PointerEventData eventData)
    {
        sceneLight.SetActive(false);
    }

    public void LoadFirstLevel() {
        UnityEngine.SceneManagement.SceneManager.LoadScene("Level1");
    }
}
