using System;
using DOTS.Tags;
using Unity.Entities;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace DefaultNamespace
{
    public class LoadScene: MonoBehaviour
    {
        public static bool IsEnd = false;
        private void OnCollisionEnter(Collision other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                var entitymanager = World.DefaultGameObjectInjectionWorld.EntityManager;
                entitymanager.DestroyEntity(entitymanager.GetAllEntities());
                SceneManager.LoadScene("Prison", LoadSceneMode.Single);
            }
        }

        private void Update()
        {
            if (IsEnd)
            {
                IsEnd = false;
                var entitymanager = World.DefaultGameObjectInjectionWorld.EntityManager;
                entitymanager.DestroyEntity(entitymanager.GetAllEntities());
                SceneManager.LoadScene("End", LoadSceneMode.Single);
            }
        }
    }
}