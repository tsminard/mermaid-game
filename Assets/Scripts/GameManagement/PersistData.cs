using UnityEngine;

// script to handle persisting data between scenes
public class PersistData : MonoBehaviour
{
    private void Awake()
    {
        DontDestroyOnLoad(transform.gameObject);
    }


}
