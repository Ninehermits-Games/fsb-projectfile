using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DeathMenu : MonoBehaviour {

    

    public virtual void Retry() {
        FindObjectOfType<GameManager>().ResetGame();
    }

    

}
