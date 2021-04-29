﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class Ranged : Weapon
{
    public RangedAnimation rangedAnimation;
    public AudioClip FiringSound;
    public AudioClip ReloadSound;

    [SerializeField] protected int ammoCapacity;
    [SerializeField] protected int damage;
    [SerializeField] protected float energyCost;
    [SerializeField] protected float reloadEnergyCost;
    [SerializeField] protected float projectileVelocity;

    [SerializeField] protected Transform projectile = null;
    [SerializeField] protected Transform projectileSpawnPoint = null;

    private CameraStateController CSC;
    private int currentAmmoCount;
    protected bool attacking;

    protected AmmoDisplay ammoDisplay;
    protected Transform projectileParent;

    private bool Started = false;

    public void AttemptReload(bool force = false)
    {
        Assert.IsFalse(EnemyWeapon);
        if (force)
        {
            Reload(false);
        }
        else if (currentAmmoCount != ammoCapacity)
        {
            if (energy.Drain_ES(false, reloadEnergyCost))
            {
                Reload(true);
            }
        }
    }

    private void Reload(bool anim)
    {
        if (anim)
        {
            animationUpdater.ReloadAnimation();
        }
        GetComponent<AudioSource>().PlayOneShot(ReloadSound);
        currentAmmoCount = ammoCapacity;
        ammoDisplay.Reload();
    }

    public bool isMagazineEmpty()
    {
        return currentAmmoCount == 0;
    }

    ///////////////////////////////////////////////////////////////////////

    public Ranged()
    {
        weaponType = WeaponType.Ranged;
    }

    public override void StartWeapon(bool EnemyWeapon_in = false, EnemyTemplateMaster ETM_in = null, HumanoidWeaponExpertise humanoidWeaponExpertise = HumanoidWeaponExpertise.Adept)
    {
        base.StartWeapon(EnemyWeapon_in, ETM_in, humanoidWeaponExpertise);
        EnemyWeapon = EnemyWeapon_in;
        Started = true;
        if (EnemyWeapon_in)
        {
            projectileParent = GameObject.Find("EnemyProjectiles").transform;
        }
        else
        {
            CSC = FindObjectOfType<CameraStateController>();
            ammoDisplay = GameObject.Find("AmmoDisplay").GetComponent<AmmoDisplay>();
            currentAmmoCount = ammoCapacity;
            projectileParent = GameObject.Find("PlayerProjectiles").transform;
        }
    }

    private void OnEnable()
    {
        if (Started & !EnemyWeapon)
        {
            ammoDisplay.Setup(ammoCapacity, currentAmmoCount);
        }
    }

    private void OnDisable()
    {
        if (Started && ammoDisplay)
        {
            ammoDisplay.Setup(0, 0);
        }
    }

    protected int ReturnFinalDamage(float input)
    {
        if (EnemyWeapon)
        {
            return (int)(input);
        }
        else
        {
            return (int)(input * (1 + stats.ReturnDamageMult(DS)));
        }
    }

    protected Transform fireProjectile(float angle_in, bool use_ammo = true)
    {
        if (use_ammo && !EnemyWeapon)
        {
            UseAmmoFunc();
        }

        GetComponent<AudioSource>().PlayOneShot(FiringSound);

        Transform tempTrans = Instantiate(projectile, projectileParent);
        tempTrans.position = projectileSpawnPoint.position;
        tempTrans.eulerAngles = new Vector3(0f, (angle_in * Mathf.Rad2Deg), 0f);
        tempTrans.GetComponentInChildren<Rigidbody>().velocity = new Vector3(Mathf.Sin(angle_in) * projectileVelocity, 0f, Mathf.Cos(angle_in) * projectileVelocity);

        return tempTrans;
    }

    protected float CustomAngle()
    {
        float angle;

        if (EnemyWeapon)
        {
            Vector3 pos = ETM.Return_Current_Target().position;
            if(WeaponExpertise == HumanoidWeaponExpertise.Commando) //Predict Movement
            {
                Vector3 vel = ETM.Return_Current_Target().GetComponentInParent<Rigidbody>().velocity;
                float time_est = (pos - transform.position).magnitude / projectileVelocity;
                pos += vel * time_est;
            }
            pos -= transform.position;

            angle = Mathf.Atan2(pos.x, pos.z);
        }
        else
        {
            Vector3 mousePos3d = new Vector3();
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hitray;
            int layerMask = (LayerMask.GetMask("Terrain", "TerrainNoCam"));
            if (Physics.Raycast(ray, out hitray, Mathf.Infinity, layerMask))
            {
                mousePos3d = hitray.point;
            }

            float special_offset = -Mathf.Tan(Mathf.PI / 2 - (Camera.main.transform.rotation.eulerAngles.x * Mathf.Deg2Rad)) * 2; //Made to hit 2 meter tall enemies. //negitive so it is the opposite angle
            angle = Mathf.Atan2((mousePos3d.x + special_offset * Mathf.Sin(Camera.main.transform.rotation.eulerAngles.y * Mathf.Deg2Rad)) - transform.position.x, (mousePos3d.z + special_offset * Mathf.Cos(Camera.main.transform.rotation.eulerAngles.y * Mathf.Deg2Rad)) - transform.position.z);
        }

        return angle;
    }

    protected void UseAmmoFunc(int amount = 1)
    {
        Assert.IsFalse(EnemyWeapon);
        currentAmmoCount -= amount;
        ammoDisplay.UseAmmo();
    }

    public bool CheckAmmo(int amount = 1)
    {
        return (currentAmmoCount >= amount);
    }

    protected override void AdvStatsHelper(List<(string, string)> tempList)
    {
        base.AdvStatsHelper(tempList);
        tempList.Add(("Ammo Capacity:", ammoCapacity.ToString()));
        tempList.Add(("Reload Energy Cost:", reloadEnergyCost.ToString()));
        tempList.Add(("Damage:", damage.ToString()));
        tempList.Add(("Firing Energy Cost:", energyCost.ToString()));
    }
}
