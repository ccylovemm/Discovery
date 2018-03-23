using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System.Linq;
using BehaviorDesigner.Runtime;

public class SceneManager : Singleton<SceneManager>
{
    public MapRoom mapRoom;
    public SceneType sceneType;
    public bool levelOver = false;
    private Vector2 playerPos = Vector2.zero;
    private Dictionary<int, MapResourceItem> mapGird = new Dictionary<int, MapResourceItem>();

    public MapVo currMapVo
    {
        get
        {
            return MapCFG.items[DataManager.userData.Group.ToString()];
        }
    }

    public LevelVo currLevelVo
    {
        get
        {
            return LevelCFG.items[DataManager.userData.Group + "" + DataManager.userData.GroupLevel];
        }
    }

    public void Enter()
    {
        if (DataManager.userData.Group == 1 && DataManager.userData.GroupLevel == 1)
        {
            EnterHome();
        }
        else
        {
            if (!LevelCFG.items.ContainsKey(SceneManager.Instance.currLevelVo.MapId + "" + (SceneManager.Instance.currLevelVo.Level + 1)))
            {
                EnterBoss();
            }
            else
            {
                EnterScene();
            }
        }
    }

    public void EnterHome()
    {
        Clear();
        sceneType = SceneType.Home;
        GameData.myData.currHp = GameData.myData.cfgVo.MaxHp;
        StartCoroutine(EnterHome_());
    }

    IEnumerator EnterHome_()
    {
        AsyncOperation operation = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync("Map");
        yield return operation;

        ResourceManager.Instance.LoadScene("resourceassets/map/City" + 1 + ".unity3d", city =>
        {
            ResourceManager.Instance.LoadAsset("resourceassets/configAssets.assetbundle", ab =>
            {
                MapRoom mapRoom = new MapRoom();
                mapRoom.offsetX = mapRoom.offsetY = 20;
                mapRoom.currMapLevelVo = new MapLevelVo();
                MapManager.ClearPath();
                mapGird.Clear();
                MapSceneAsset mapSceneAsset = (MapSceneAsset)ab.LoadAsset<MapSceneAsset>("City" + 1 + ".asset");
                int index = mapSceneAsset.layers.IndexOf(MapEditorSortLayer.Floor1);
                MapLayerItem layerItem = mapSceneAsset.layerItems[index];
                int c = layerItem.items.Count;
                for (int j = 0; j < c; j++)
                {
                    int x = layerItem.posList[j] / 1000;
                    int y = layerItem.posList[j] % 1000;
                    int key = (x + mapRoom.offsetX) * 1000 + (y + mapRoom.offsetY);
                    mapGird.Add(key, layerItem.items[j]);
                }
                mapRoom.CreateScene(mapSceneAsset);
                Loading.Instance.FadeDisable(() => { WindowManager.Instance.OpenWindow(WindowKey.MainUI);});
            });
        });
    }

    public void EnterScene()
    {
        Clear();
        sceneType = SceneType.Level;
        StartCoroutine(EnterScene_());
    }

    IEnumerator EnterScene_()
    {
        AsyncOperation operation = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync("Map");

        while (!operation.isDone)
        {
            yield return null;
        }

        ResourceManager.Instance.LoadAsset("resourceassets/configAssets.assetbundle", ab =>
        {
            List<MapStyleVo> styles = MapStyleCFG.items.Where(o => o.Type == currLevelVo.MapStyle).ToList<MapStyleVo>();

            MapAsset mapPrefabAsset = (MapAsset)ab.LoadAsset<MapAsset>(styles[Random.Range(0 , styles.Count)].ResName + ".asset");

            List<AStarNode> point = new List<AStarNode>();
            Dictionary<Vector2, AStarNode> allRooms = new Dictionary<Vector2, AStarNode>();
            for (int i = 0; i < mapPrefabAsset.mapdata.Count; i++)
            {
                if (mapPrefabAsset.mapdata[i] == 0) continue;

                int x = i % mapPrefabAsset.sizeX;
                int y = i / mapPrefabAsset.sizeY;
                AStarNode aStarNode = new AStarNode(new Vector2(x, y));
                aStarNode.i = i;
                aStarNode.index = mapPrefabAsset.mapdata[i];
                allRooms.Add(aStarNode.mapGird, aStarNode);

                if (mapPrefabAsset.mapdata[i] == 1 || mapPrefabAsset.mapdata[i] == 2 || mapPrefabAsset.mapdata[i] == 3 || mapPrefabAsset.mapdata[i] == 4)
                {
                    point.Add(aStarNode);
                }
            }
            int length = 0;

            List<AStarNode> allPos = new List<AStarNode>();
            AStarNode brithPos = point[Random.Range(0, point.Count)];
            point.Remove(brithPos);
            for(int i = 0; i < point.Count; i ++)
            {
                List<AStarNode> path = FindRoomPath(allRooms , brithPos.mapGird , point[i].mapGird);
                if(path != null)
                {
                    if (path.Count > length)
                    {
                        allPos.Clear();
                        allPos.Add(point[i]);
                        length = path.Count;
                    }
                    else if(path.Count == length)
                    {
                        allPos.Add(point[i]);
                    }
                }
            }

            AStarNode transPos = allPos[Random.Range(0, allPos.Count)];

            List<MapRoom> allRoomList = new List<MapRoom>();
            int count = mapPrefabAsset.mapdata.Count;
            for (int i = 0; i < count; i ++)
            {
                if (mapPrefabAsset.mapdata[i] == 0) continue;
                MapRoom mapRoom = new MapRoom();
                mapRoom.offsetX = (i % mapPrefabAsset.sizeX) * (int)currMapVo.SizeX;
                mapRoom.offsetY = (i / mapPrefabAsset.sizeY) * (int)currMapVo.SizeY;
                if (brithPos.i == i)
                {
                    List<MapLevelVo> births = MapLevelCFG.items.Where(o => o.World == currMapVo.Id && (MapRoomType)o.Type == MapRoomType.Birth && o.AreaType == mapPrefabAsset.mapdata[i]).ToList<MapLevelVo>();
                    mapRoom.currMapLevelVo = births[Random.Range(0, births.Count)];
                }
                else if (transPos.i == i)
                {
                    List<MapLevelVo> trans = MapLevelCFG.items.Where(o => o.World == currMapVo.Id && (MapRoomType)o.Type == MapRoomType.Trans && o.AreaType == mapPrefabAsset.mapdata[i]).ToList<MapLevelVo>();
                    mapRoom.currMapLevelVo = trans[Random.Range(0, trans.Count)];
                }
                else
                {
                    List<MapLevelVo> levels = MapLevelCFG.items.Where(o => o.World == currMapVo.Id && (MapRoomType)o.Type == MapRoomType.Level && o.AreaType == mapPrefabAsset.mapdata[i]).ToList<MapLevelVo>();
                    mapRoom.currMapLevelVo = levels[Random.Range(0, levels.Count)];
                }
                allRoomList.Add(mapRoom);
            }

            MapManager.ClearPath();

            mapGird.Clear();

            count = allRoomList.Count;
            for (int i = 0;i < count; i++)
            {
                MapRoom mapRoom = allRoomList[i];
                MapSceneAsset mapSceneAsset = (MapSceneAsset)ab.LoadAsset<MapSceneAsset>(mapRoom.currMapLevelVo.ResName + ".asset");
                int index = mapSceneAsset.layers.IndexOf(MapEditorSortLayer.Floor1);
                MapLayerItem mapLayerItem = mapSceneAsset.layerItems[index];
                int c = mapLayerItem.items.Count;
                for (int n = 0; n < c; n++)
                {
                    int x = mapLayerItem.posList[n] / 1000;
                    int y = mapLayerItem.posList[n] % 1000;
                    int key = (x + mapRoom.offsetX) * 1000 + (y + mapRoom.offsetY);
                    mapGird.Add(key, mapLayerItem.items[n]);
                }
            }

            for (int i = 0; i < count; i++)
            {
                allRoomList[i].CreateScene((MapSceneAsset)ab.LoadAsset<MapSceneAsset>(allRoomList[i].currMapLevelVo.ResName + ".asset") , mapGird);
            }
            RandomLevelAltar(currLevelVo , new List<MapRoom>(allRoomList));
            RandomLevelDrop(currLevelVo.SceneDrop, new List<MapRoom>(allRoomList));
            RandomLevelMonster(currLevelVo.MonsterGroups, new List<MapRoom>(allRoomList));
            Loading.Instance.FadeDisable(() => { WindowManager.Instance.OpenWindow(WindowKey.MainUI); EventCenter.DispatchEvent(EventEnum.EnterLevel); });
        });

    }

    public void EnterBoss()
    {
        Clear();
        sceneType = SceneType.Boss;
        StartCoroutine(EnterBoss_());
    }

    IEnumerator EnterBoss_()
    {
        AsyncOperation operation = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync("Map");
        yield return operation;

        ResourceManager.Instance.LoadAsset("resourceassets/configAssets.assetbundle", ab =>
        {
            List<MapLevelVo> bossLevels = MapLevelCFG.items.Where(o => o.World == currMapVo.Id && (MapRoomType)o.Type == MapRoomType.BossLevel).ToList<MapLevelVo>();
            mapRoom = new MapRoom();
            mapRoom.offsetX = mapRoom.offsetY = 20;
            mapRoom.currMapLevelVo = bossLevels[Random.Range(0, bossLevels.Count)];
            MapManager.ClearPath();

            mapGird.Clear();
            MapSceneAsset mapSceneAsset = (MapSceneAsset)ab.LoadAsset<MapSceneAsset>(mapRoom.currMapLevelVo.ResName + ".asset");
            int index = mapSceneAsset.layers.IndexOf(MapEditorSortLayer.Floor1);
            MapLayerItem mapLayerItem = mapSceneAsset.layerItems[index];
            for (int i = 0; i < mapLayerItem.items.Count; i++)
            {
                int x = mapLayerItem.posList[i] / 1000;
                int y = mapLayerItem.posList[i] % 1000;
                int key = (x + mapRoom.offsetX) * 1000 + (y + mapRoom.offsetY);
                mapGird.Add(key, mapLayerItem.items[i]);
            }
            mapRoom.CreateScene(mapSceneAsset);
            RandomLevelDrop(currLevelVo.SceneDrop, new List<MapRoom>() { mapRoom });
            Loading.Instance.FadeDisable(() => { WindowManager.Instance.OpenWindow(WindowKey.MainUI); EventCenter.DispatchEvent(EventEnum.EnterLevel); });
        });
    }

    public void RandHolePoint()
    {
        Vector2 pos = MapManager.FindGridByRange(GameData.myself.transform.position, 2, 1);
        ResourceManager.Instance.LoadAsset("resourceassets/map.assetbundle", hole =>
        {
            GameObject.Instantiate((GameObject)hole.LoadAsset("Hole.prefab"), MapManager.GetPos((int)pos.x, (int)pos.y), Quaternion.identity);
        });
    }

    public void CreateMonster(Vector2 grid , string groupStr)
    {
        if (groupStr == "") return;
        string[] groups = groupStr.Split(',');
        for (int i = 0; i < groups.Length; i++)
        {
            CreateMonster(new ActorData(uint.Parse(groups[i])), MapManager.GetPos((int)grid.x, (int)grid.y));
        }
    }

    public void CreateActor(ActorData actorData, Vector2 pos)
    {
        ResourceManager.Instance.LoadAsset("resourceassets/avatar.assetbundle", model =>
        {
            GameObject actor = GameObject.Instantiate((GameObject)model.LoadAsset(System.IO.Path.GetFileNameWithoutExtension(actorData.cfgVo.ResName) + ".prefab"));
            actor.transform.position = new Vector3(pos.x, pos.y, 0);
            actor.transform.localScale = Vector3.one * actorData.cfgVo.Scale;
            ActorObject actorObject = actor.GetComponent<ActorObject>();
            actorObject.actorData = actorData;
          
            if (GameData.myData.uniqueId == actorData.uniqueId)
            {
                actor.layer = LayerUtil.LayerToPlayer();

                if (GameData.myself != null)
                {
                    GameObject.Destroy(GameData.myself.gameObject);
                }

                actor.AddComponent<PlayerInput>();

                GameData.myself = actorObject;
                EventCenter.DispatchEvent(EventEnum.UpdateMainUIAttackBtn);
                CameraManager.Instance.Init(actorObject.transform);
                if (DataManager.userData.EmployId != 0)
                {
                    pos = MapManager.FindPosByRange(actorObject.transform.position, 2 * MapManager.textSize, MapManager.textSize);
                    ActorData data = new ActorData(DataManager.userData.EmployId);
                    data.currHp = DataManager.userData.EmployHp;
                    SceneManager.Instance.CreateFriend(data , pos, actorObject , true);
                }
            }
            GameData.friends.Add(actorObject);
            GameData.allUnits.Add(actorObject);
            ResourceManager.Instance.LoadAsset("resourceassets/gui.assetbundle", headbar =>
            {
                actorObject.headBar = GameObject.Instantiate((GameObject)headbar.LoadAsset("PlayerHeadBar.prefab"), HeadRoot.root).GetComponent<HeadBar>();
                actorObject.headBar.target = actor.transform.Find("HeadPos");
                actorObject.UpdateHp();
                if (GameData.myData.uniqueId == actorData.uniqueId)
                {
                    actorObject.headSkill = GameObject.Instantiate((GameObject)headbar.LoadAsset("HeadSkill.prefab"), HeadRoot.root).GetComponent<HeadSkill>();
                    actorObject.headSkill.target = actor.transform.Find("SkillPos");
                }
            });
        });
    }

    public void CreateMonster(ActorData actorData, Vector2 pos)
    {
        ResourceManager.Instance.LoadAsset("resourceassets/avatar.assetbundle", model =>
        {
            GameObject actor = GameObject.Instantiate((GameObject)model.LoadAsset(System.IO.Path.GetFileNameWithoutExtension(actorData.cfgVo.ResName) + ".prefab"));
            actor.transform.position = new Vector3(pos.x, pos.y, 0);
            actor.transform.localScale = Vector3.one * actorData.cfgVo.Scale;
            ActorObject actorObject = actor.GetComponent<ActorObject>();
            actorObject.actorData = actorData;
            actorObject.isAI = true;
            if ((ActorType)actorData.cfgVo.Type == ActorType.Boss)
            {
                GameData.boss.Add(actorObject);
            }

            if(!string.IsNullOrEmpty(actorData.cfgVo.AIRes))
            {
                StartCoroutine(DelayAddAI(actorObject, actorData.cfgVo.AIRes));
            }

            GameData.enemys.Add(actorObject);
            GameData.allUnits.Add(actorObject);
            ResourceManager.Instance.LoadAsset("resourceassets/gui.assetbundle", headbar =>
            {
                Transform trans = actor.transform.Find("HeadPos");
                if (trans != null)
                {
                    actorObject.headBar = GameObject.Instantiate((GameObject)headbar.LoadAsset("ActorHeadBar.prefab"), HeadRoot.root).GetComponent<HeadBar>();
                    actorObject.headBar.target = actor.transform.Find("HeadPos");
                    actorObject.UpdateHp();
                    actorObject.headBar.hpBar.gameObject.SetActive((ActorType)actorData.cfgVo.Type != ActorType.Boss);
                }
            });
        });
    }

    public void CreateFriend(ActorData actorData, Vector2 pos, ActorObject master, bool isEmploy = false)
    {
        ResourceManager.Instance.LoadAsset("resourceassets/avatar.assetbundle", model =>
        {
            GameObject actor = GameObject.Instantiate((GameObject)model.LoadAsset(System.IO.Path.GetFileNameWithoutExtension(actorData.cfgVo.ResName) + ".prefab"));
            actor.transform.position = new Vector3(pos.x, pos.y, 0);
            actor.transform.localScale = Vector3.one * actorData.cfgVo.Scale;
            ActorObject actorObject = actor.GetComponent<ActorObject>();
            actorObject.actorData = actorData;
            actorObject.isAI = true;
            actorObject.master = master;
            if ((ActorType)master.actorData.cfgVo.Type == ActorType.Player)
            {
                GameData.friends.Add(actorObject);
            }
            else
            {
                GameData.enemys.Add(actorObject);
            }
            GameData.allUnits.Add(actorObject);
            if (isEmploy)
            {
                if (GameData.employ != null)
                {
                    GameObject.Destroy(GameData.employ.gameObject);
                }
                GameData.employ = actorObject;
            }
            else
            {
                if (master.pets.Count > 2)
                {
                    master.pets[0].IsDead = true;
                    GameObject.Destroy(master.pets[0].gameObject);
                    master.pets.RemoveAt(0);
                }
                master.pets.Add(actorObject);
            }
            StartCoroutine(DelayAddAI(actorObject, actorData.cfgVo.AIRes));
            ResourceManager.Instance.LoadAsset("resourceassets/gui.assetbundle", headbar =>
            {
                actorObject.headBar = GameObject.Instantiate((GameObject)headbar.LoadAsset("ActorHeadBar.prefab"), HeadRoot.root).GetComponent<HeadBar>();
                actorObject.headBar.target = actor.transform.Find("HeadPos");
                actorObject.UpdateHp();
            });
        });
    }

    IEnumerator DelayAddAI(ActorObject actor , string AIRes)
    {
        yield return new WaitForSeconds(1.0f);
        if (!string.IsNullOrEmpty(AIRes))
        {
            ResourceManager.Instance.LoadAsset("resourceassets/configAssets.assetbundle", config =>
            {
                actor.behaviorTree = actor.gameObject.AddComponent<BehaviorTree>();
                actor.behaviorTree.ExternalBehavior = config.LoadAsset<ExternalBehavior>(AIRes + ".asset");
                actor.behaviorTree.StartWhenEnabled = true;
                actor.behaviorTree.PauseWhenDisabled = true;
                actor.behaviorTree.RestartWhenComplete = true;
                actor.behaviorTree.ResetValuesOnRestart = true;
            });
        }
    }

    public bool TerrainIsWater(Vector2 pos)
    {
        Vector2 gridPos = MapManager.GetGrid(pos.x, pos.y);
        int posValue = (int)gridPos.x * 1000 + (int)gridPos.y;
        if (!mapGird.ContainsKey(posValue)) return false;
        MapEditorItemType itemType = mapGird[posValue].itemType;
        if (itemType == MapEditorItemType.Water || itemType == MapEditorItemType.DeepWater)
        {
            return true;
        }
        return false;
    }

    public bool TerrainIsFall(Vector2 pos)
    {
        Vector2 gridPos = MapManager.GetGrid(pos.x, pos.y);
        int posValue = (int)gridPos.x * 1000 + (int)gridPos.y;
        if (!mapGird.ContainsKey(posValue)) return false;
        MapEditorItemType itemType = mapGird[posValue].itemType;
        if (itemType == MapEditorItemType.Hole || itemType == MapEditorItemType.DeepWater && !MapWaterFrezon.IsFrezon(gridPos))
        {
            return true;
        }
        return false;
    }

    public bool TerrainIsHole(Vector2 pos)
    {
        Vector2 gridPos = MapManager.GetGrid(pos.x, pos.y);
        int posValue = (int)gridPos.x * 1000 + (int)gridPos.y;
        if (!mapGird.ContainsKey(posValue)) return false;
        MapEditorItemType itemType = mapGird[posValue].itemType;
        if (itemType == MapEditorItemType.Hole)
        {
            return true;
        }
        return false;
    }

    public bool TerrainIn(Vector2 pos , MapEditorItemType type)
    {
         Vector2 gridPos = MapManager.GetGrid(pos.x, pos.y);
        int posValue = (int)gridPos.x * 1000 + (int)gridPos.y;
        if (!mapGird.ContainsKey(posValue)) return false;
        MapEditorItemType itemType = mapGird[posValue].itemType;
        if (itemType == type)
        {
            return true;
        }
        return false;
    }

    private void RandomLevelDrop(string str, List<MapRoom> allRoomList)
    {
        if (string.IsNullOrEmpty(str)) return;
        string[] drops = str.Split('|');
        List<uint> boxs = new List<uint>();
        for (int i = 0; i < drops.Length; i++)
        {
            string[] box = drops[i].Split(',');
            if (Random.Range(0, 10000) <= int.Parse(box[2]))
            {
                int num = int.Parse(box[1]);
                for (int n = 0; n < num; n++)
                {
                    boxs.Add(uint.Parse(box[0]));
                }
            }
        }
        while (allRoomList.Count > 0 && boxs.Count > 0)
        {
            int index = Random.Range(0 , allRoomList.Count);
            if (allRoomList[index].mapInfo.randonChestPos.Count == 0)
            {
                allRoomList.RemoveAt(index);
                continue;
            }
            uint id = boxs[Random.Range(0 , boxs.Count)];
            RandomDropBox(id , allRoomList[index].mapInfo.randonChestPos[Random.Range(0 , allRoomList[index].mapInfo.randonChestPos.Count)] + new Vector2(allRoomList[index].offsetX , allRoomList[index].offsetY));
            allRoomList.RemoveAt(index);
            boxs.Remove(id);
        }
    }

    private void RandomLevelMonster(string str, List<MapRoom> allRoomList)
    {
        if (string.IsNullOrEmpty(str)) return;
        string[] groups = str.Split(',');
        List<uint> monsterGroups = new List<uint>();
        for(int i = 0; i < groups.Length; i ++)
        {
            monsterGroups.Add(uint.Parse(groups[i]));
        }

        string level = SceneManager.Instance.currLevelVo.MonsterLevel;

        while (allRoomList.Count > 0 && monsterGroups.Count > 0)
        {
            int monsterMaxCount = 2;
            int index = Random.Range(0, allRoomList.Count);

            while (true)
            {
                if (allRoomList[index].mapInfo.randonMonsterPos.Count == 0)
                {
                    allRoomList.RemoveAt(index);
                    break;
                }
                uint group = monsterGroups[Random.Range(0, monsterGroups.Count)];
                if (MonsterGroupCFG.items.ContainsKey(group.ToString()))
                {
                    MonsterGroupVo groupVo = MonsterGroupCFG.items[group.ToString()];
                    object o = groupVo.GetType().GetField(level).GetValue(groupVo);
                    int posIndex = Random.Range(0, allRoomList[index].mapInfo.randonMonsterPos.Count);
                    Vector2 grid = allRoomList[index].mapInfo.randonMonsterPos[posIndex] + new Vector2(allRoomList[index].offsetX, allRoomList[index].offsetY);
                    CreateMonster(grid, System.Convert.ToString(o));
                    allRoomList[index].mapInfo.randonMonsterPos.RemoveAt(posIndex);
                }
                monsterGroups.Remove(group);
                monsterMaxCount--;
                if (monsterMaxCount == 0)
                {
                    allRoomList.RemoveAt(index);
                    break;
                }
            }
        }
    }

    private void RandomLevelAltar(LevelVo levelVo , List<MapRoom> allRoomList)
    {
        uint altarNum = levelVo.AltarNum;
        string[] altarsTypes = levelVo.AltarType.Split(',');
        string[] altarRates = levelVo.AltarRate.Split(',');
        List<uint> altars = new List<uint>();
        for (int i = 0; i < altarNum; i++)
        {
            for (int n = 0; n < altarRates.Length ; n ++)
            {
                int value = Random.Range(0 , 10000);
                if (value < int.Parse(altarRates[n]))
                {
                    altars.Add(uint.Parse(altarsTypes[n]));
                    break;
                }
            }
        }
        while (allRoomList.Count > 0 && altars.Count > 0)
        {
            int index = Random.Range(0, allRoomList.Count);
            if (allRoomList[index].mapInfo.randonAltarPos.Count == 0)
            {
                allRoomList.RemoveAt(index);
                continue;
            }
            uint id = altars[Random.Range(0, altars.Count)];
            CreateAltar(id , allRoomList[index].mapInfo.randonAltarPos[Random.Range(0, allRoomList[index].mapInfo.randonAltarPos.Count)] + new Vector2(allRoomList[index].offsetX, allRoomList[index].offsetY));
            allRoomList.RemoveAt(index);
            altars.Remove(id);
        }
    }

    public void CreateAltar(uint id, Vector2 grid)
    {
        MapManager.SetPathData(grid, false);
        MapManager.SetPathHoleData(grid, false);

        AltarVo altarVo = AltarCFG.items[id.ToString()];
        ResourceManager.Instance.LoadAsset("resourceassets/map.assetbundle", asset =>
        {
            GameObject.Instantiate((GameObject)asset.LoadAsset(altarVo.ResName + ".prefab"), MapManager.GetPos((int)grid.x, (int)grid.y), Quaternion.identity);
        });
    }

    private void RandomDropBox(uint id , Vector2 grid)
    {
        DropVo drop = DropCFG.items[id.ToString()];
        ResourceManager.Instance.LoadAsset("resourceassets/item.assetbundle", asset =>
        {
            DropItem item = GameObject.Instantiate((GameObject)asset.LoadAsset(drop.ResName + ".prefab"), MapManager.GetPos((int)grid.x, (int)grid.y), Quaternion.identity).AddComponent<DropItem>();
            item.dropVo = drop;
        });
    }

    public void RandomDrop(uint id, Vector2 originPos)
    {
        if (id == 0) return;
        DropVo dropVo = DropCFG.items[id.ToString()];
        Vector3 targetPos = MapManager.FindDropPos(originPos);
        if (dropVo.Type1 == 1)
        {
            string[] items = dropVo.Reward.Split(',');
            string[] nums = dropVo.Num.Split(',');
            string[] rates = dropVo.Rate.Split(',');

            if (dropVo.Type2 == 1)
            {
                int index = -1;
                int rand = Random.Range(0, 10000);
                for (int i = 0; i < items.Length; i++)
                {
                    if (rand < int.Parse(rates[i]))
                    {
                        index = i;
                        break;
                    }
                }
                if (index != -1)
                {
                   RandomDropItem(uint.Parse(items[index]), uint.Parse(nums[index]), originPos);
                }
            }
            else if (dropVo.Type2 == 2)
            {
                for (int i = 0; i < items.Length; i++)
                {
                    int rand = Random.Range(0, 10000);
                    if (rand < int.Parse(rates[i]))
                    {
                        RandomDropItem(uint.Parse(items[i]), uint.Parse(nums[i]), originPos);
                    }
                }
            }
        }
        else
        {
            ResourceManager.Instance.LoadAsset("resourceassets/item.assetbundle", asset =>
            {
                DropItem item = GameObject.Instantiate((GameObject)asset.LoadAsset(dropVo.ResName + ".prefab"), originPos , Quaternion.identity).AddComponent<DropItem>();
                item.dropVo = dropVo;
                Vector3 pos = new Vector3((originPos.x + targetPos.x) / 2.0f, Mathf.Max(originPos.y, targetPos.y) + 0.2f, 0);
                DOTween.To(() => 0.0f, t => item.transform.position = Bezier.CalculateCubicBezierPoint(t, originPos, pos, targetPos), 1.0f, 0.3f);
            });
        }
    }

    public void RandomDropItem(uint itemId , uint num , Vector3 originPos)
    {
        Vector3 targetPos = MapManager.FindDropPos(originPos);

        ItemVo itemVo = ItemCFG.items[itemId.ToString()];
        ResourceManager.Instance.LoadAsset("resourceassets/item.assetbundle", asset =>
        {
            DropItem item = null;
            if (itemVo.Id == 1)//是金币
            {
                item = GameObject.Instantiate((GameObject)asset.LoadAsset("Item_Gold" + Random.Range(1 , 4) + ".prefab"), originPos, Quaternion.identity).AddComponent<DropItem>();
                item.itemVo = itemVo;
                item.dropNum = num;
            }
            else
            {
                item = GameObject.Instantiate((GameObject)asset.LoadAsset(itemVo.ItemResources + ".prefab"), originPos, Quaternion.identity).AddComponent<DropItem>();
                item.itemVo = itemVo;
                item.dropNum = num;
            }

            Vector3 pos = new Vector3((originPos.x + targetPos.x) / 2.0f , Mathf.Max(originPos.y , targetPos.y) + 0.3f , 0);
            DOTween.To(() => 0.0f, t => item.transform.position = Bezier.CalculateCubicBezierPoint(t, originPos, pos , targetPos), 1.0f, 0.3f);
        });
    }

    public void CheckLevel()
    {
        if (levelOver || sceneType != SceneType.Boss) return;
        if (GameData.boss.Count == 0)
        {
            levelOver = true;
            RandHolePoint();
        }
    }

    static public List<AStarNode> FindRoomPath(Dictionary<Vector2 , AStarNode> roomPath , Vector2 startPos , Vector2 endPoss)
    {
        List<AStarNode> openPath = new List<AStarNode>();
        List<AStarNode> ErrorPath = new List<AStarNode>();
        AStarNode startNode = roomPath[startPos];
        AStarNode endNode = roomPath[endPoss];
        List<AStarNode> openNode = new List<AStarNode>();
        List<AStarNode> closeNode = new List<AStarNode>();
        openNode.Add(startNode);
        startNode.parent = null;
        while (openNode.Count > 0)
        {
            AStarNode currentNode = openNode[0];

            for (int i = 0; i < openNode.Count; i++)
            {
                if (openNode[i].fValue <= currentNode.fValue && openNode[i].hValue < currentNode.hValue)
                {
                    currentNode = openNode[i];
                }
            }
            openNode.Remove(currentNode);
            closeNode.Add(currentNode);
            if (currentNode == endNode)
            {
                List<AStarNode> path = new List<AStarNode>();
                while (currentNode.parent != null)
                {
                    path.Add(currentNode);
                    currentNode = currentNode.parent;
                }
                path.Reverse();
                return path;
            }
            openPath.Add(currentNode);

            List<AStarNode> OpenNodeList = GetOpenAroundNode(currentNode, closeNode, roomPath);
            foreach (var node in OpenNodeList)
            {
                int newCode = currentNode.gValue + MapManager.GetDistanceToPos(currentNode, node);
                if (!openNode.Contains(node) || node.gValue > newCode)
                {
                    node.gValue = newCode;
                    node.hValue = MapManager.GetDistanceToPos(node, endNode);
                    node.parent = currentNode;
                    if (!openNode.Contains(node))
                    {
                        openNode.Add(node);
                    }
                }
            }
        }
        return null;
    }

    static private List<AStarNode> GetOpenAroundNode(AStarNode currentNode, List<AStarNode> closeNode, Dictionary<Vector2, AStarNode> pathData)
    {
        List<AStarNode> OpenNodeList = new List<AStarNode>();

        Vector2 pos1 = new Vector2(currentNode.mapGird.x - 1, currentNode.mapGird.y);
        Vector2 pos2 = new Vector2(currentNode.mapGird.x + 1, currentNode.mapGird.y);
        Vector2 pos3 = new Vector2(currentNode.mapGird.x, currentNode.mapGird.y - 1);
        Vector2 pos4 = new Vector2(currentNode.mapGird.x, currentNode.mapGird.y + 1);

        if (pathData.ContainsKey(pos1) && !closeNode.Contains(pathData[pos1]) &&(
            currentNode.index == 3 ||
            currentNode.index == 6 ||
            currentNode.index == 8 ||
            currentNode.index == 9 ||
            currentNode.index == 10 ||
            currentNode.index == 11 ||
            currentNode.index == 14 ||
            currentNode.index == 15
            ))
        {
            OpenNodeList.Add(pathData[pos1]);
        }

        if (pathData.ContainsKey(pos2) && !closeNode.Contains(pathData[pos2]) && (
            currentNode.index == 4 ||
            currentNode.index == 5 ||
            currentNode.index == 7 ||
            currentNode.index == 9 ||
            currentNode.index == 10 ||
            currentNode.index == 12 ||
            currentNode.index == 14 ||
            currentNode.index == 15
            ))
        {
            OpenNodeList.Add(pathData[pos2]);
        }

        if (pathData.ContainsKey(pos3) && !closeNode.Contains(pathData[pos3]) && (
            currentNode.index == 2 ||
            currentNode.index == 5 ||
            currentNode.index == 6 ||
            currentNode.index == 10 ||
            currentNode.index == 11 ||
            currentNode.index == 12 ||
            currentNode.index == 13 ||
            currentNode.index == 15
            ))
        {
            OpenNodeList.Add(pathData[pos3]);
        }

        if (pathData.ContainsKey(pos4) && !closeNode.Contains(pathData[pos4]) && (
            currentNode.index == 1 ||
            currentNode.index == 7 ||
            currentNode.index == 8 ||
            currentNode.index == 9 ||
            currentNode.index == 11 ||
            currentNode.index == 12 ||
            currentNode.index == 13 ||
            currentNode.index == 15
            ))
        {
            OpenNodeList.Add(pathData[pos4]);
        }

        return OpenNodeList;
    }

    private void Clear()
    {
        levelOver = false;
        GameData.allUnits.Clear();
        GameData.friends.Clear();
        GameData.enemys.Clear();
        GameData.boss.Clear();
        SysManager.Instance.sceneObjs.Clear();
        Loading.Instance.Reset();
        WindowManager.Instance.CloseAllWindow();
        BombMagic.allBombList.Clear();
    }
}
