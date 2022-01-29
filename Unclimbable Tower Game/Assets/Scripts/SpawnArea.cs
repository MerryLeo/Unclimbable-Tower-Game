// Disables this game object when the player leaves its collider

using UnityEngine;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Material))]
public class SpawnArea : MonoBehaviour
{
    const string playerTag = "Player";
    GameObject playerObj;
    Timer timer;
    Material mat;

    // Start is called before the first frame update
    void Start()
    {
        timer = GameObject.Find("Timer").GetComponentInChildren<Timer>();
        playerObj = GameObject.Find("Player");
        mat = GetComponent<Renderer>().sharedMaterial;
    }

    // Update is called once per frame
    void Update()
    {
        float dst = Vector3.Distance(playerObj.transform.position, gameObject.transform.position);
        mat.SetFloat("_Opacity", dst);
    }

    void OnTriggerExit(Collider other) 
    {
        if (other.tag == playerTag)
        {
            gameObject.SetActive(false);
        }
    }
}
