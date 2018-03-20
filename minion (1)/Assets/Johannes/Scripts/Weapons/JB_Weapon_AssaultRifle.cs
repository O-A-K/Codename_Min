﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JB_Weapon_AssaultRifle : JB_Weapon
{
    // Assault Rifle
    [Header("Assault Rifle")]
    public float spreadDegreeAR;
    public float timeToMaxSpread = 1;
    private float spreadMultiplier;

    // Shotgun
    [Header("Shotgun")]
    public float spreadDegreeSG;
    public int numberOfPellets;

    public LineRenderer smokeTrailPrefab;
    public ParticleSystem onHitParticleHeadshot;

    void Update()
    {
        if (isFiring)
        {
            fireDuration += Time.deltaTime;
        }
    }

    public override void FireWeapon()
    {
        isFiring = true;

        if (!isReloading && Time.time >= fireTime && currentAmmo > 0)
        {
            Instantiate(firePrimarySFX, transform.position, Quaternion.identity);
            currentAmmo -= ammoUsagePrimary;
            CheckAmmo();
            UpdateAmmoUI();
            fireTime = Time.time + 1 / fireRatePrimary;

            Vector3 fireDirection = pc.cam.transform.TransformDirection(Vector3.forward);

            // Spread
            spreadMultiplier = Mathf.Lerp(0, 1, fireDuration / timeToMaxSpread);
            fireDirection = Quaternion.AngleAxis(Random.Range(-spreadDegreeAR, spreadDegreeAR) * spreadMultiplier, pc.cam.transform.TransformDirection(Vector3.up)) * fireDirection;
            fireDirection = Quaternion.AngleAxis(Random.Range(-spreadDegreeAR, spreadDegreeAR) * spreadMultiplier, pc.cam.transform.TransformDirection(Vector3.left)) * fireDirection;
            //

            RaycastHit hit;
            Vector3 terrainHitPoint = Vector3.zero;
            Vector3 terrainNormal = Vector3.zero;
            bool hitTerrain = false;

            if (Physics.Raycast(pc.cam.transform.position + pc.cam.transform.TransformDirection(Vector3.forward) / 2,
                fireDirection, out hit, 200, JB_GameManager.gm.terrainLayer))
            {
                hitTerrain = true;
                terrainHitPoint = hit.point;
                terrainNormal = hit.normal;
            }

            var trail = Instantiate(smokeTrailPrefab, pc.transform.position + Vector3.up, Quaternion.identity);
            trail.SetPosition(0, trail.transform.position);
            trail.SetPosition(1, hit.point);

            if (Physics.SphereCast(pc.cam.transform.position, .1f, pc.cam.transform.TransformDirection(Vector3.forward), out hit, JB_GameManager.gm.enemyLayer))
            {
                if (hitTerrain) 
                {
                    if ((pc.transform.position - terrainHitPoint).sqrMagnitude < (pc.transform.position - hit.point).sqrMagnitude)    // if terrain was hit earlier than player
                    {
                        var part = Instantiate(onHitParticle, terrainHitPoint, Quaternion.identity);
                        part.transform.forward = terrainNormal;
                        return;
                    }
                    else    // if player was hit before terrain
                    {
                        if (hit.collider.gameObject.layer == 10)    // headshot
                        {
                            var part = Instantiate(onHitParticleHeadshot, hit.point, Quaternion.identity);
                            part.transform.forward = hit.normal;
                        }
                        else
                        {
                            var part = Instantiate(onHitParticle, hit.point, Quaternion.identity);
                            part.transform.forward = hit.normal;
                        }
                        trail.SetPosition(1, hit.point);
                        pc.ShootPC(hit.collider.gameObject, weaponDamagePrimary, hit.collider.gameObject.layer);
                        return;
                    }
                }
                else
                {
                    if (hit.collider.gameObject.layer == 10)    // headshot
                    {
                        var part = Instantiate(onHitParticleHeadshot, hit.point, Quaternion.identity);
                        part.transform.forward = hit.normal;
                    }
                    else
                    {
                        var part = Instantiate(onHitParticle, hit.point, Quaternion.identity);
                        part.transform.forward = hit.normal;
                    }
                    pc.ShootPC(hit.collider.gameObject, weaponDamagePrimary, hit.collider.gameObject.layer);
                    return;
                }
            }
            else if (hitTerrain)
            {
                var part = Instantiate(onHitParticle, terrainHitPoint, Quaternion.identity);
                part.transform.forward = terrainNormal;
                return;
            }
            
            trail.SetPosition(0, trail.transform.position);
            trail.SetPosition(1, pc.cam.transform.TransformDirection(Vector3.forward) * 20);


        }
    }

    public override void FireWeaponSecondary()
    {
        isFiring = true;
        if (!isReloading && Time.time >= fireTime && currentAmmo > 0)
        {
            fireTime = Time.time + 1 / fireRateSecondary;
            Vector3 fireDirection;
            RaycastHit hit;

            for (int i = 0; i < numberOfPellets; i++)
            {
                fireDirection = pc.cam.transform.TransformDirection(Vector3.forward);
                fireDirection = Quaternion.AngleAxis(Random.Range(-spreadDegreeSG, spreadDegreeSG), pc.cam.transform.TransformDirection(Vector3.up)) * fireDirection;
                fireDirection = Quaternion.AngleAxis(Random.Range(-spreadDegreeSG, spreadDegreeSG), pc.cam.transform.TransformDirection(Vector3.left)) * fireDirection;

                if (Physics.Raycast(pc.cam.transform.position, fireDirection, out hit, 100, JB_GameManager.gm.weaponLayer))
                {
                    LineRenderer trail = Instantiate(smokeTrailPrefab, pc.transform.position + Vector3.up, Quaternion.identity);
                    trail.SetPosition(0, trail.transform.position);
                    trail.SetPosition(1, hit.point);
                    if (hit.collider.gameObject.layer == 8) // if it's terrain
                    {
                        ParticleSystem part = Instantiate(onHitParticle, hit.point, Quaternion.identity);
                        part.transform.forward = hit.normal;
                    }
                    else    // if it's another PC
                    {
                        print("hit player");

                        if (hit.collider.gameObject.layer == 10)    // headshot
                        {
                            ParticleSystem part = Instantiate(onHitParticleHeadshot, hit.point, Quaternion.identity);
                            part.transform.forward = hit.normal;
                        }
                        else
                        {
                            ParticleSystem part = Instantiate(onHitParticlePlayer, hit.point, Quaternion.identity);
                            part.transform.forward = hit.normal;
                        }

                        pc.ShootPC(hit.collider.gameObject, weaponDamageSecondary, hit.collider.gameObject.layer);

                    }
                }
                else
                {
                    LineRenderer trail = Instantiate(smokeTrailPrefab, pc.transform.position + Vector3.up, Quaternion.identity);
                    trail.SetPosition(0, trail.transform.position);
                    trail.SetPosition(1, pc.cam.transform.TransformDirection(Vector3.forward) * 20);
                }
            }
            
            Instantiate(fireSecondarySFX, transform.position, Quaternion.identity);
            currentAmmo -= ammoUsageSecondary;
            CheckAmmo();
            UpdateAmmoUI();
        }
    }

    public override void StoppedFiring()
    {
        isFiring = false;
        fireDuration = 0;
    }
}
