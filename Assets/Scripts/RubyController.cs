using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;


public class RubyController : MonoBehaviour
{
    public float speed = 3.0f;
    
    public int maxHealth = 5;
    
    public GameObject projectilePrefab;
    
    public GameObject Ruby;

    private Enemy Enemy;
    
    public AudioClip throwSound;
    public AudioClip hitSound;
    public AudioClip dialogboxSound;

    public TextMeshProUGUI ScoreText;
    public TextMeshProUGUI AmmoText;
    public GameObject winTextObject;
    public GameObject loseTextObject;
    public GameObject JambiTextObject;

    public AudioClip musicClipOne;
    public AudioClip musicClipTwo;
    public AudioClip musicClipThree;

    public AudioClip Coin;

    public AudioSource musicSource;

    public static int currentScore;

    public int Score;
    public int Ammo;

    public GameObject HitParticleEffect;
    
    public int health { get { return currentHealth; }}
    int currentHealth;
    
    public float timeInvincible = 2.0f;
    bool isInvincible;
    float invincibleTimer;
    
    Rigidbody2D rigidbody2d;
    float horizontal;
    float vertical;
    
    Animator animator;
    Vector2 lookDirection = new Vector2(1,0);
    
    AudioSource audioSource;
    
    // Start is called before the first frame update
    void Start()
    {
        rigidbody2d = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        
        currentHealth = maxHealth;

        audioSource = GetComponent<AudioSource>();

        Score = 0;

        Ammo = 4;

        winTextObject.SetActive(false);
        loseTextObject.SetActive(false);
        JambiTextObject.SetActive(false);

        musicSource.clip = musicClipOne;
        musicSource.Play();
        musicSource.loop = true;
    }

    // Update is called once per frame
    void Update()
    {
        AmmoText.text = "Ammo: " + Ammo.ToString();
        horizontal = Input.GetAxis("Horizontal");
        vertical = Input.GetAxis("Vertical");

        Score = currentScore;
        SetScoreText(0);
        
        Vector2 move = new Vector2(horizontal, vertical);
        
        if(!Mathf.Approximately(move.x, 0.0f) || !Mathf.Approximately(move.y, 0.0f))
        {
            lookDirection.Set(move.x, move.y);
            lookDirection.Normalize();
        }
        
        animator.SetFloat("Look X", lookDirection.x);
        animator.SetFloat("Look Y", lookDirection.y);
        animator.SetFloat("Speed", move.magnitude);
        
        if (isInvincible)
        {
            invincibleTimer -= Time.deltaTime;
            if (invincibleTimer < 0)
                isInvincible = false;
        }
        
        if(Input.GetKeyDown(KeyCode.C))
        {
           if(Ammo >= 1) 
           {
            Launch();
            Ammo = Ammo - 1;
           }
        }
        
        if (Input.GetKeyDown(KeyCode.X))
        {
            RaycastHit2D hit = Physics2D.Raycast(rigidbody2d.position + Vector2.up * 0.2f, lookDirection, 1.5f, LayerMask.GetMask("NPC"));
            if (hit.collider != null)
            {
                NonPlayerCharacter character = hit.collider.GetComponent<NonPlayerCharacter>();
                if (character != null)
                {
                    character.DisplayDialog();
                    audioSource.PlayOneShot(dialogboxSound);

                      if (Score == 4)
                {
                     SceneManager.LoadScene("Scene2");
                }
                }
            }
        }

        if (Input.GetKey(KeyCode.R))
        {
            if (currentHealth == 0)

            {
              SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex); // this loads the currently active scene

            }
        }

        if (Input.GetKey(KeyCode.R))
        {
            if (Score == 8)

            {
              SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex); // this loads the currently active scene

            }
        }

        if (Input.GetKey(KeyCode.Q))
        {
              
              musicSource.clip = musicClipTwo;
              musicSource.Play();
              musicSource.loop = false;
        }
    }

    public void SetScoreText(int amount)
    {
        ScoreText.text = "Fixed: " + Score.ToString();

         if(Score == 4)
         {
            JambiTextObject.SetActive(true);
         }

         if(Score == 8)
        {
            winTextObject.SetActive(true);
            speed = 0.0f;
        }
    }
    
    void FixedUpdate()
    {
        Vector2 position = rigidbody2d.position;
        position.x = position.x + speed * horizontal * Time.deltaTime;
        position.y = position.y + speed * vertical * Time.deltaTime;

        rigidbody2d.MovePosition(position);
    }

   private void OnCollisionEnter2D(Collision2D collision)
    {
       if (collision.collider.tag == "CollectibleAmmo")
        {
            Ammo += 4;
            PlaySound(Coin);
            Destroy(collision.collider.gameObject);
        }

       if (collision.collider.tag == "CollectibleSpeed")
        {
            speed += 1;
            Destroy(collision.collider.gameObject);
        }
    }

    public void ChangeHealth(int amount)
    {
        if (amount < 0)
        {
            if (isInvincible)
                return;
            
            isInvincible = true;
            invincibleTimer = timeInvincible;

            animator.SetTrigger("Hit");
            audioSource.PlayOneShot(hitSound);

            Instantiate(HitParticleEffect, transform.position + Vector3.up * 0.5f, Quaternion.identity);
            
            PlaySound(hitSound);

            if (currentHealth == 0)
        { 
            animator.SetTrigger("Hit");
            audioSource.PlayOneShot(hitSound);

            Instantiate(HitParticleEffect, transform.position + Vector3.up * 0.5f, Quaternion.identity);
        
            PlaySound(hitSound);
            loseTextObject.SetActive(true);
            speed = 0.0f;
            isInvincible = true;
            invincibleTimer = timeInvincible;
            timeInvincible = 1000000;
            musicSource.clip = musicClipOne;
            musicSource.Stop();
            musicSource.clip = musicClipThree;
            musicSource.Play();
            musicSource.loop = true;
        }
     }

        
        
        currentHealth = Mathf.Clamp(currentHealth + amount, 0, maxHealth);
        
        UIHealthBar.Instance.SetValue(currentHealth / (float)maxHealth);
    }


    void Launch()
    {
        GameObject projectileObject = Instantiate(projectilePrefab, rigidbody2d.position + Vector2.up * 0.5f, Quaternion.identity);

        Projectile projectile = projectileObject.GetComponent<Projectile>();
        projectile.Launch(lookDirection, 300);

        animator.SetTrigger("Launch");
        
        PlaySound(throwSound);
    } 
    
    public void PlaySound(AudioClip clip)
    {
        audioSource.PlayOneShot(clip);
    }
}