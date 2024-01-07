using System;
using System.Collections;
using System.Collections.Generic;
using GameNetcodeStuff;
using UnityEngine;

// Token: 0x02000002 RID: 2
public class GameObjectManager
{
	// Token: 0x17000001 RID: 1
	// (get) Token: 0x06000001 RID: 1 RVA: 0x00002050 File Offset: 0x00000250
	public static GameObjectManager Instance
	{
		get
		{
			bool flag = GameObjectManager.instance == null;
			if (flag)
			{
				GameObjectManager.instance = new GameObjectManager();
			}
			return GameObjectManager.instance;
		}
	}

	// Token: 0x06000002 RID: 2 RVA: 0x0000207F File Offset: 0x0000027F
	public IEnumerator CollectObjects()
	{
		for (;;)
		{
			this.InitializeReferences();
			this.ClearLists();
			this.CollectObjectsOfType<GrabbableObject>(this.items, null);
			this.CollectObjectsOfType<Landmine>(this.landmines, null);
			this.CollectObjectsOfType<Turret>(this.turrets, null);
			this.CollectObjectsOfType<DoorLock>(this.doorLocks, null);
			this.CollectObjectsOfType<EntranceTeleport>(this.entranceTeleports, null);
			this.CollectObjectsOfType<PlayerControllerB>(this.players, (PlayerControllerB p) => !p.name.StartsWith("Player #"));
			this.CollectObjectsOfType<EnemyAI>(this.enemies, null);
			this.CollectObjectsOfType<SteamValveHazard>(this.steamValves, null);
			this.CollectObjectsOfType<PlaceableShipObject>(this.shipObjects, null);
			this.bigDoors = this.FindObjectsOfType<TerminalAccessibleObject>((TerminalAccessibleObject obj) => obj.isBigDoor);
			yield return new WaitForSeconds(3f);
		}
	}

	// Token: 0x06000003 RID: 3 RVA: 0x00002090 File Offset: 0x00000290
	public void InitializeReferences()
	{
		GameNetworkManager gameNetworkManager = GameNetworkManager.Instance;
		this.localPlayer = ((gameNetworkManager != null) ? gameNetworkManager.localPlayerController : null);
		this.shipLights = UnityEngine.Object.FindObjectOfType<ShipLights>();
		this.shipTerminal = UnityEngine.Object.FindObjectOfType<Terminal>();
		this.shipRoom = UnityEngine.Object.FindAnyObjectByType<StartMatchLever>();
		this.itemsDesk = UnityEngine.Object.FindObjectOfType<DepositItemsDesk>();
		this.shipDoor = UnityEngine.Object.FindObjectOfType<HangarShipDoor>();
		this.tvScript = UnityEngine.Object.FindObjectOfType<TVScript>();
		this.localVisor = GameObject.Find("Systems/Rendering/PlayerHUDHelmetModel/");
	}

	// Token: 0x06000004 RID: 4 RVA: 0x00002110 File Offset: 0x00000310
	public void ClearLists()
	{
		this.items.Clear();
		this.landmines.Clear();
		this.turrets.Clear();
		this.doorLocks.Clear();
		this.entranceTeleports.Clear();
		this.players.Clear();
		this.enemies.Clear();
		this.steamValves.Clear();
		this.shipObjects.Clear();
		this.bigDoors.Clear();
	}

	// Token: 0x06000005 RID: 5 RVA: 0x00002198 File Offset: 0x00000398
	public void CollectObjectsOfType<T>(List<T> list, Predicate<T> predicate = null) where T : MonoBehaviour
	{
		foreach (T t in UnityEngine.Object.FindObjectsOfType<T>())
		{
			bool flag = predicate == null || predicate(t);
			if (flag)
			{
				list.Add(t);
			}
		}
	}

	// Token: 0x06000006 RID: 6 RVA: 0x000021E0 File Offset: 0x000003E0
	public List<T> FindObjectsOfType<T>(Predicate<T> predicate = null) where T : MonoBehaviour
	{
		T[] collection = UnityEngine.Object.FindObjectsOfType<T>();
		bool flag = predicate != null;
		List<T> result;
		if (flag)
		{
			result = new List<T>(collection).FindAll(predicate);
		}
		else
		{
			result = new List<T>(collection);
		}
		return result;
	}

	// Token: 0x04000001 RID: 1
	public const float CollectionInterval = 3f;

	// Token: 0x04000002 RID: 2
	public List<GrabbableObject> items = new List<GrabbableObject>();

	// Token: 0x04000003 RID: 3
	public List<Landmine> landmines = new List<Landmine>();

	// Token: 0x04000004 RID: 4
	public List<Turret> turrets = new List<Turret>();

	// Token: 0x04000005 RID: 5
	public List<DoorLock> doorLocks = new List<DoorLock>();

	// Token: 0x04000006 RID: 6
	public List<EntranceTeleport> entranceTeleports = new List<EntranceTeleport>();

	// Token: 0x04000007 RID: 7
	public List<PlayerControllerB> players = new List<PlayerControllerB>();

	// Token: 0x04000008 RID: 8
	public List<EnemyAI> enemies = new List<EnemyAI>();

	// Token: 0x04000009 RID: 9
	public List<SteamValveHazard> steamValves = new List<SteamValveHazard>();

	// Token: 0x0400000A RID: 10
	public List<PlaceableShipObject> shipObjects = new List<PlaceableShipObject>();

	// Token: 0x0400000B RID: 11
	public List<TerminalAccessibleObject> bigDoors = new List<TerminalAccessibleObject>();

	// Token: 0x0400000C RID: 12
	public PlayerControllerB localPlayer;

	// Token: 0x0400000D RID: 13
	public HangarShipDoor shipDoor;

	// Token: 0x0400000E RID: 14
	public StartMatchLever shipRoom;

	// Token: 0x0400000F RID: 15
	public ShipLights shipLights;

	// Token: 0x04000010 RID: 16
	public Terminal shipTerminal;

	// Token: 0x04000011 RID: 17
	public DepositItemsDesk itemsDesk;

	// Token: 0x04000012 RID: 18
	public TVScript tvScript;

	// Token: 0x04000013 RID: 19
	public GameObject localVisor;

	// Token: 0x04000014 RID: 20
	public static GameObjectManager instance;
}
