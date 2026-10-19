using UnityEngine;

public class Water : MonoBehaviour
{

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            //other.transform.position = new Vector3(-6.278319f, 6.58479f, 59.46635f);
            CharacterController cc = other.GetComponent<CharacterController>();
            if (cc)
            {
                cc.enabled = false;
                other.transform.position = new Vector3(-6.278319f, 6.58479f, 59.46635f);
                cc.enabled = true;
            }
        }

    }
}
