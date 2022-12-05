using UnityEngine;

/// <summary>
/// Will handle giving ammo to the character when they enter the trigger.
/// </summary>
public class AmmoCollectible : MonoBehaviour 
{
    private RubyController rubyController;
    public GameObject PickUpEffect;
    public AudioClip Collectable;

    void OnTriggerEnter2D(Collider2D other)
    {
        RubyController controller = other.GetComponent<RubyController>();

        if (controller != null)
        {
            Instantiate(PickUpEffect, transform.position + Vector3.up * 0.5f, Quaternion.identity);
            Destroy(gameObject);
        }
    }
}