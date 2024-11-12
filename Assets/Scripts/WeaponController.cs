using UnityEngine;

public class WeaponController : MonoBehaviour
{
    [SerializeField] private Transform barrel;

    [Header("Ammo")]
    [SerializeField] private int currentAmmo;
    [SerializeField] private int maxAmmo;
    [SerializeField] private bool infiniteAmmo;

    [Header("Performance")]
    [SerializeField] private float bulletSpeed;
    [SerializeField] private float shootRate;
    [SerializeField] private int damage;

    private ObjectPool objectPool;
    private float lastShootTime;

    private bool isPlayer;

    private void Awake()
    {
        //Check if I am a Player
        isPlayer = GetComponent<PlayerMovement>() != null;

        //get objectPool component
        objectPool = GetComponent<ObjectPool>();
    }

    /// <summary>
    /// check if is possible to shoot
    /// </summary>
    /// <returns>bool</returns>
    public bool CanShoot()
    {
        //Check shootRate
        if (Time.time - lastShootTime >= shootRate)
        {
            //Check Ammo
            if (currentAmmo >0 || infiniteAmmo)
            {
                return true;
            }
        }

        return false;

    }

    /// <summary>
    /// Handle Weapon Shoot
    /// </summary>
    public void Shoot()
    {
        //update last Shoot Time
        lastShootTime = Time.time;

        //reduce the Ammo
        if (!infiniteAmmo) currentAmmo--;

        //Get a new bullet
        GameObject bullet = objectPool.GetGameObject();

        //Locate the ball at the barrel position
        bullet.transform.position = barrel.position;
        bullet.transform.rotation = barrel.rotation;
        //assign damage to Bullet
        bullet.GetComponent<BulletController>().Damage = damage;

        //Give velocity to Bullet 
        


        if (isPlayer)
        {
            Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));

            RaycastHit hit;
            Vector3 targetPoint;

            if (Physics.Raycast(ray, out hit))
                targetPoint = hit.point;
            else
                targetPoint = ray.GetPoint(5);

            bullet.GetComponent<Rigidbody>().linearVelocity = (targetPoint - barrel.position) * bulletSpeed;

        }
        else
        {
            bullet.GetComponent<Rigidbody>().linearVelocity = barrel.forward * bulletSpeed;
        }
    }


}
