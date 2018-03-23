using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

[CanEditMultipleObjects]
[CustomEditor(typeof(MapResourceItem))]
public class MapResourceItemEditor : Editor
{
    private bool show1 = true;
    private bool show2 = false;
    private MapResourceItem resourceItem;

    void OnEnable()
    {
        resourceItem = (MapResourceItem)target;
    }

    public void OnSceneGUI()
    {
        MapResourceItem s = (MapResourceItem)target;

        if (GUI.changed)
        {
            EditorUtility.SetDirty(s);
        }
    }

    private Vector2 scrollPos = Vector2.zero;

    public override void OnInspectorGUI()
    {
        EditorGUILayout.BeginVertical();

        GUILayout.Label("物件类型", GUILayout.Width(60));

        GUILayout.BeginVertical();
        resourceItem.itemType = (MapEditorItemType)EditorGUILayout.EnumPopup(resourceItem.itemType, GUILayout.Width(100));

        if (resourceItem.itemType == MapEditorItemType.StoneWall || resourceItem.itemType == MapEditorItemType.Wall)
        {
            resourceItem.replaceItem = (MapResourceItem)EditorGUILayout.ObjectField("墙的边缘替换" , resourceItem.replaceItem, typeof(MapResourceItem), true, GUILayout.Width(400), GUILayout.Height(20));
            resourceItem.replaceEdgeUp = (MapResourceItem)EditorGUILayout.ObjectField("墙的边角上" , resourceItem.replaceEdgeUp, typeof(MapResourceItem), true, GUILayout.Width(400), GUILayout.Height(20));
            resourceItem.replaceEdgeLeft = (MapResourceItem)EditorGUILayout.ObjectField("墙的边角左", resourceItem.replaceEdgeLeft, typeof(MapResourceItem), true, GUILayout.Width(400), GUILayout.Height(20));
            resourceItem.replaceEdgeRight = (MapResourceItem)EditorGUILayout.ObjectField("墙的边角右", resourceItem.replaceEdgeRight, typeof(MapResourceItem), true, GUILayout.Width(400), GUILayout.Height(20));
            resourceItem.replaceEdgeSingleUp = (MapResourceItem)EditorGUILayout.ObjectField("墙的边角单个向上", resourceItem.replaceEdgeSingleUp, typeof(MapResourceItem), true, GUILayout.Width(400), GUILayout.Height(20));
            resourceItem.replaceEdgeLeftUp = (MapResourceItem)EditorGUILayout.ObjectField("墙的边角左上", resourceItem.replaceEdgeLeftUp, typeof(MapResourceItem), true, GUILayout.Width(400), GUILayout.Height(20));
            resourceItem.replaceEdgeRightUp = (MapResourceItem)EditorGUILayout.ObjectField("墙的边角右上", resourceItem.replaceEdgeRightUp, typeof(MapResourceItem), true, GUILayout.Width(400), GUILayout.Height(20));
            resourceItem.replaceEdgeVer = (MapResourceItem)EditorGUILayout.ObjectField("墙的边角竖", resourceItem.replaceEdgeVer, typeof(MapResourceItem), true, GUILayout.Width(400), GUILayout.Height(20));
        }

        GUILayout.EndVertical();

        GUILayout.BeginHorizontal();
        resourceItem.worldResource = (MapWorldResource)EditorGUILayout.EnumPopup(resourceItem.worldResource, GUILayout.Width(100));
        resourceItem.isPrefab = GUILayout.Toggle(resourceItem.isPrefab, "是否已经定制好了");
        resourceItem.isPath = GUILayout.Toggle(resourceItem.isPath, "是否可寻路");

        GUILayout.BeginHorizontal();
        GUILayout.Label("显示层级", GUILayout.Width(60));
        resourceItem.layer = (MapEditorSortLayer)EditorGUILayout.EnumPopup(resourceItem.layer, GUILayout.Width(100));
     
        GUILayout.EndHorizontal();

        if (resourceItem.isPrefab)
        {

        }
        else
        {
            GUILayout.EndHorizontal();
            resourceItem.isNine = GUILayout.Toggle(resourceItem.isNine, "九宫格");
            GUILayout.BeginHorizontal();
            show1 = GUILayout.Toggle(show1, "显示上半部" , GUILayout.Width(100));
            show2 = !show1;
            show2 = GUILayout.Toggle(show2, "显示下半部" , GUILayout.Width(100));
            show1 = !show2;
            GUILayout.EndHorizontal();
            scrollPos = GUILayout.BeginScrollView(scrollPos, GUILayout.Height(600));
            if (resourceItem.isNine)
            {
                if (show1)
                {
                    DrawImage("外角左上", resourceItem.out_1);
                    DrawImage("外角右上", resourceItem.out_2);
                    DrawImage("外角左下", resourceItem.out_3);
                    DrawImage("外角右下", resourceItem.out_4);

                    DrawImage("内角左上", resourceItem.in_1);
                    DrawImage("内角右上", resourceItem.in_2);
                    DrawImage("内角左下", resourceItem.in_3);
                    DrawImage("内角右下", resourceItem.in_4);

                    DrawImage("上", resourceItem.up);
                    DrawImage("下", resourceItem.down);
                    DrawImage("左", resourceItem.left);
                    DrawImage("右", resourceItem.right);
                    DrawImage("中", resourceItem.center);

                    DrawImage("单个", resourceItem.single);

                    DrawImage("单个向上", resourceItem.singleUp);
                    DrawImage("单个向下", resourceItem.singleDown);
                    DrawImage("单个向左", resourceItem.singleLeft);
                    DrawImage("单个向右", resourceItem.singleRight);

                    DrawImage("左上拐角", resourceItem.corner1_1);
                    DrawImage("右上拐角", resourceItem.corner1_2);
                    DrawImage("左下拐角", resourceItem.corner1_3);
                    DrawImage("右下拐角", resourceItem.corner1_4);
                }

                if (show2)
                {
                    DrawImage("左上右下对角", resourceItem.angle1_1);
                    DrawImage("左下右上对角", resourceItem.angle1_2);

                    DrawImage("横向双通", resourceItem.pass1_1);
                    DrawImage("竖向双通", resourceItem.pass1_2);

                    DrawImage("上三通", resourceItem.pass2_1);
                    DrawImage("下三通", resourceItem.pass2_2);
                    DrawImage("左三通", resourceItem.pass2_3);
                    DrawImage("右三通", resourceItem.pass2_4);

                    DrawImage("上凸", resourceItem.pass3_1);
                    DrawImage("下凸", resourceItem.pass3_2);
                    DrawImage("左凸", resourceItem.pass3_3);
                    DrawImage("右凸", resourceItem.pass3_4);

                    DrawImage("上凸靠左", resourceItem.pass4_1);
                    DrawImage("上凸靠右", resourceItem.pass4_2);
                    DrawImage("下凸靠左", resourceItem.pass4_3);
                    DrawImage("下凸靠右", resourceItem.pass4_4);
                    DrawImage("左凸靠上", resourceItem.pass4_5);
                    DrawImage("左凸靠下", resourceItem.pass4_6);
                    DrawImage("右凸靠上", resourceItem.pass4_7);
                    DrawImage("右凸靠下", resourceItem.pass4_8);

                    DrawImage("左上通两路", resourceItem.pass5_1);
                    DrawImage("右上通两路", resourceItem.pass5_2);
                    DrawImage("左下通两路", resourceItem.pass5_3);
                    DrawImage("右下通两路", resourceItem.pass5_4);

                    DrawImage("四通", resourceItem.passAll);
                }
            }
            else
            {
                DrawImage("随机列表", resourceItem.normalList);
            }
            GUILayout.EndScrollView();
        }

        if (GUI.changed)
        {
            EditorUtility.SetDirty(resourceItem);
        }
        EditorGUILayout.EndVertical();
    }

    private void DrawImage(string desc, List<MapSprite> list)
    {
        GUILayout.Label(desc);

        GUILayout.BeginHorizontal();
        for (int i = 0; i < list.Count; i++)
        {
            GUILayout.BeginHorizontal();
            GUILayout.BeginVertical();
            list[i].sprite = (Sprite)EditorGUILayout.ObjectField(list[i].sprite, typeof(Sprite), true, GUILayout.Width(60), GUILayout.Height(60));
            if (GUILayout.Button("删除", GUILayout.Width(60), GUILayout.Height(20)))
            {
                list.RemoveAt(i);
            }
            list[i].sRate = EditorGUILayout.IntField(list[i].sRate, GUILayout.Width(50));
            GUILayout.EndVertical();
            GUILayout.BeginVertical();
            GUILayout.Label("偏移 X");
            list[i].offsetX = EditorGUILayout.FloatField(list[i].offsetX, GUILayout.Width(50));
            GUILayout.Label("偏移 Y");
            list[i].offsetY = EditorGUILayout.FloatField(list[i].offsetY, GUILayout.Width(50));
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();
        }
        if (GUILayout.Button("+", GUILayout.Width(40), GUILayout.Height(40)))
        {
            list.Add(new MapSprite());
        }
        GUILayout.EndHorizontal();
    }
}
