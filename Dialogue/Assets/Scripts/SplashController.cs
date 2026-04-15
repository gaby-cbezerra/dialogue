using System.Collections;
using UnityEngine;

public class SplashController : MonoBehaviour
{
    private IEnumerator Start()
    {
        yield return new WaitForSeconds(2f);

        GameManager.Instance.RequestLoadSceneByName("MainMenu");
    }
}