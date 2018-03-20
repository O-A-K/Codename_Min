using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JB_Ammo_NoobTube : JB_Ammo
{
    public float explosionRadius;
    public float pushForce = 20;
    private List<ControlPC> playersHit = new List<ControlPC>();
    private float lifetimer = 0;
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        lifetimer += Time.deltaTime;
        if (lifetimer > 3)
        {
            rb.drag = 0;
        }
        else
        {
            //print(rb.velocity);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        RaycastHit[] hits = Physics.SphereCastAll(transform.position, explosionRadius, collision.contacts[0].normal, 1, JB_GameManager.gm.weaponLayer);
        
        if (hits.Length > 0)
        {
            for (int i = 0; i < hits.Length; i++)
            {
                HitboxLink hbl = hits[0].collider.GetComponent<HitboxLink>();
                if (hbl)
                {
                    // has this PC been accounted for already
                    if (!HasPlayerBeenHit(hbl.pc))
                    {
                        // if no terrain between explosion and player hitbox link
                        if (!Physics.Linecast(transform.position, hbl.transform.position, JB_GameManager.gm.terrainLayer))
                        {
                            // add pc to players hit list, explode on pc
                            playersHit.Add(hbl.pc);
                        }
                    }
                }
            }

            foreach (ControlPC _pc in playersHit)
            {
                _pc.CmdTakeDamage(damage);
                Vector3 dir = (_pc.transform.position + Vector3.up) - transform.position;
                dir.Normalize();
                _pc.movementModifiers.Add(new MovementMod(dir * pushForce, Time.time, Time.time + .5f, true, false, true));
            }
        }
        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        Instantiate(onHitParticle, transform.position, Quaternion.identity);
    }

    private bool HasPlayerBeenHit(ControlPC _pc)
    {
        foreach (ControlPC item in playersHit)
        {
            if (item == _pc)
            {
                return true;
            }
        }
        return false;
    }
}
