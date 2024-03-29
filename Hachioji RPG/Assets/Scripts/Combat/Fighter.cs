﻿using RPG.Core;
using RPG.Movement;
using RPG.Saving;
using RPG.Attributes;
using UnityEngine;
using RPG.Stats;
using System;
using System.Collections.Generic;

namespace RPG.Combat
{
    public class Fighter : MonoBehaviour, IAction, ISaveable, IModify
    {
        [SerializeField] float timeBetweenAttacks = 1f;
        [SerializeField] Transform rightHandTransform = null;
        [SerializeField] Transform leftHandTransform = null;
        [SerializeField] Weapon defaultWeapon = null;

        Health target;
        float timeSinceLastAttack = 0;
        Weapon currentWeapon = null;

        private void Start()
        {
            if (!currentWeapon) EquipWeapon(defaultWeapon);
        }

        private void Update()
        {
            timeSinceLastAttack += Time.deltaTime;
            if (!target) return;
            if (target.IsDead()) return;
            if (!GetIsInRange())
            {
                GetComponent<Mover>().MoveTo(target.transform.position, 1f);
            }
            else
            {
                GetComponent<Mover>().Cancel();
                AttackBehaviour();
            }
        }

        private void AttackBehaviour()
        {
            transform.LookAt(target.transform);
            if (timeSinceLastAttack > timeBetweenAttacks)
            {
                // this will trigger the Hit() event
                TriggerAttack();
                timeSinceLastAttack = 0;
            }
        }

        private void TriggerAttack()
        {
            GetComponent<Animator>().ResetTrigger("stopAttack");
            GetComponent<Animator>().SetTrigger("attack");
        }

        public bool CanAttack(GameObject combatTarget)
        {
            if (!combatTarget) return false;
            Health targetToTest = combatTarget.GetComponent<Health>();
            return targetToTest != null && !targetToTest.IsDead();
        }

        /**
         * Animation Event
         * **/
        void Hit()
        {
            if (!target) return;
            float damage = GetComponent<BaseStats>().GetStat(Stat.Damage);
            if (currentWeapon.IsRanged())
            {
                currentWeapon.ShootProjectile(rightHandTransform, leftHandTransform, target, gameObject);
            }
            else
            {
                target.TakeDamage(gameObject, damage);
            }
        }

        void Shoot()
        {
            Hit();
        }

        /**
         * 
         * **/

        private bool GetIsInRange()
        {
            return Vector3.Distance(transform.position, target.transform.position) < currentWeapon.GetRange();
        }

        public void Attack(GameObject combatTarget)
        {
            GetComponent<CommandManager>().StartAction(this); 
            target = combatTarget.GetComponent<Health>();
        }

        public void Cancel()
        {
            StopAttack();
            target = null;
            GetComponent<Mover>().Cancel();
        }

        private void StopAttack()
        {
            GetComponent<Animator>().ResetTrigger("attack");
            GetComponent<Animator>().SetTrigger("stopAttack");
        }

        public void EquipWeapon(Weapon weapon)
        {
            currentWeapon = weapon;
            Animator animator = GetComponent<Animator>();
            weapon.Create(rightHandTransform, leftHandTransform, animator);
            
        }

        public Health GetTarget()
        {
            return target;
        }

        public object CaptureState()
        {
            return currentWeapon.name;
        }

        public void RestoreState(object state)
        {
            string weapon = state.ToString();
            EquipWeapon(Resources.Load<Weapon>(weapon));
        }

        public IEnumerable<float> GetModifier(Stat stat)
        {
            if (stat == Stat.Damage)
            {
                yield return currentWeapon.GetDamage();
            }
        }

        public IEnumerable<float> GetPercentModifier(Stat stat)
        {
            if (stat == Stat.Damage)
            {
                yield return currentWeapon.GetPercentModifier();
            }
        }
    }
}