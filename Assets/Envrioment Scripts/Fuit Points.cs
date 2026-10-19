using UnityEngine;

public class FuitPoints : MonoBehaviour
{
    public PlayerController player;

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            player.PointCollected();
            Destroy(gameObject);
        }
    }
}
