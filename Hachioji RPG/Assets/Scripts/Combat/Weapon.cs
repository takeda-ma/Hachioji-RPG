using RPG.Core;
using UnityEngine;

namespace RPG.Combat
{
    [CreateAssetMenu(fileName = "Weapon", menuName = "Hachioji RPG/Create New Weapon", order = 0)]
    public class Weapon : ScriptableObject
    {
        [SerializeField] AnimatorOverrideController swordAnimatorOR = null;
        [SerializeField] GameObject weaponPrefab = null;
        [SerializeField] float weaponRange = 2f;
        [SerializeField] float weaponDamage = 5f;
        [SerializeField] bool isRightHand = true;
        [SerializeField] Projectile projectile = null;

        public float GetDamage() { return weaponDamage; }

        public float GetRange() { return weaponRange; }

        public bool IsRanged() { return projectile != null; }

        public void Create(Transform rightHand, Transform leftHand, Animator animator)
        {
            if (weaponPrefab)
            {
                Transform handTransform = GetHandTransform(rightHand, leftHand);
                Instantiate(weaponPrefab, handTransform);
            }
            if (swordAnimatorOR)
            {
                animator.runtimeAnimatorController = swordAnimatorOR;
            }
        }

        private Transform GetHandTransform(Transform rightHand, Transform leftHand)
        {
            Transform handTransform;
            if (isRightHand) handTransform = rightHand;
            else handTransform = leftHand;
            return handTransform;
        }

        public void ShootProjectile(Transform rightHand, Transform leftHand, Health target)
        {
            Projectile projectileInstance = Instantiate(projectile, GetHandTransform(rightHand, leftHand).position, Quaternion.identity);
            projectileInstance.setTarget(target, weaponDamage);
        }
    }
}