﻿using Booma.Instance.Common;
using Booma.Instance.Data;
using GladBehaviour.Common;
using GladNet.Engine.Server;
using SceneJect.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Booma.Instance.Server
{
	[Injectee]
	public class ServerPlayerEntityFactory : GladMonoBehaviour, IPlayerEntityFactory
	{
		[SerializeField]
		private IEntityPrefabProvider prefabProvider;

		[SerializeField]
		private readonly ISpawnPointStrategy playerSpawnStrategy;

		private void Start()
		{
			if (prefabProvider == null)
				throw new InvalidOperationException($"Set {nameof(prefabProvider)} in the inspector with the player entity prefab.");
		}

		public GameObject CreatePlayerEntity(int id, Vector3 position, Quaternion rotation)
		{
			GameObject playerGo = GameObject.Instantiate(prefabProvider.GetPrefab(EntityType.Player), position, rotation) as GameObject;

			return PostProcessEntityGameObject(playerGo, id, EntityType.Player);
		}

		public GameObject PostProcessEntityGameObject(GameObject playerGameObject, int id, EntityType type)
		{
			EntityIdentifier identifierComponent = playerGameObject.GetComponent<EntityIdentifier>();

			//Initialize the component that contains the info about the entity.
			identifierComponent.Initialize(id, type);

			return playerGameObject;
		}

		public GameObject CreatePlayerEntity(int id)
		{
			Transform spawnTransform = playerSpawnStrategy.GetSpawnpoint();

			if (spawnTransform == null)
				throw new InvalidOperationException($"Unable to produce {nameof(Transform)} from {nameof(ISpawnPointStrategy)}.");

			return CreatePlayerEntity(id, spawnTransform.position, spawnTransform.rotation);
		}
	}
}