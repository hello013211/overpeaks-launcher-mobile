using UnityEngine;

public class NPCController : MonoBehaviour
{
    private string npcName;
    private Transform player;
    private bool isSpawned;
    private Color originalColor;

    public void Initialize(string name, Transform playerTransform)
    {
        npcName = name;
        player = playerTransform;
        originalColor = new Color(0.9f, 0.65f, 0.32f);
        GetComponent<Renderer>().material.color = originalColor;
        isSpawned = true;
    }

    private void Update()
    {
        if (!isSpawned || player == null)
            return;

        Vector3 toPlayer = player.position - transform.position;
        if (toPlayer.magnitude < 5f)
        {
            transform.rotation = Quaternion.LookRotation(new Vector3(toPlayer.x, 0f, toPlayer.z));
        }
    }

    public void Interact()
    {
        if (player == null)
            return;

        float distance = Vector3.Distance(transform.position, player.position);
        if (distance <= 3f)
        {
            GameLauncher.Instance.UpdateStatus($"{npcName}: Welcome to Overpeaks! The world is offline-safe and ready for adventure.");
        }
        else
        {
            GameLauncher.Instance.UpdateStatus($"{npcName}: Come closer and talk to me.");
        }
    }
}
