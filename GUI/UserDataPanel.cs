﻿using System;
using System.IO;
using System.Linq;
using MAI2.Util;
using Manager;
using MelonLoader;
using SinmaiAssist.Utils;
using UnityEngine;

namespace SinmaiAssist.GUI;

public class UserDataPanel
{
    private static UserData _player1 = null;
    private static UserData _player2 = null;
    
    private enum CollectionType
    {
        Icon = UserData.Collection.Icon,
        Plate = UserData.Collection.Plate,
        Title = UserData.Collection.Title,
        Partner = UserData.Collection.Partner,
        Frame = UserData.Collection.Frame
    }
    
    private static string[] _userInputId = new string[6] { "", "", "", "", "", ""};
    
    public static void OnGUI()
    {
        GUILayout.Label($"User Info:");
        try
        {
            _player1 = Singleton<UserDataManager>.Instance.GetUserData(0);
            _player2 = Singleton<UserDataManager>.Instance.GetUserData(1);
        }
        catch (Exception e)
        {
            // ignore
        }
        GUILayout.Label($"1P: {_player1.Detail.UserName} ({_player1.Detail.UserID})");
        GUILayout.Label($"2P: {_player2.Detail.UserName} ({_player2.Detail.UserID})");
        
        GUILayout.Label("Add Collections", MainGUI.Style.Title);
        foreach (CollectionType type in Enum.GetValues(typeof(CollectionType)))
        {
            GUILayout.BeginHorizontal();
            int typeId = (int)type;
            GUILayout.Label(type.ToString(), new GUIStyle(UnityEngine.GUI.skin.label){fixedWidth = 50});
            _userInputId[typeId] = GUILayout.TextField(_userInputId[typeId]);
            if (GUILayout.Button("Add", new GUIStyle(UnityEngine.GUI.skin.button){ fixedWidth = 50}))
            {
                AddCollections(0, type, _userInputId[typeId]);
                AddCollections(1, type, _userInputId[typeId]);
            }
            GUILayout.EndHorizontal();
        }
        
        GUILayout.Label("Unlock Music", MainGUI.Style.Title);
        GUILayout.BeginHorizontal();
        GUILayout.Label("Music", new GUIStyle(UnityEngine.GUI.skin.label){fixedWidth = 50});
        _userInputId[0] = GUILayout.TextField(_userInputId[0]);
        if (GUILayout.Button("Add", new GUIStyle(UnityEngine.GUI.skin.button){ fixedWidth = 50}))
        {
            UnlockMusic(0, _userInputId[0]);
            UnlockMusic(1, _userInputId[0]);
        }
        GUILayout.EndHorizontal();
        
        GUILayout.Label("User Data Backup", MainGUI.Style.Title);
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("1P")) User.ExportBackupData(0);
        if (GUILayout.Button("2P")) User.ExportBackupData(1);
        GUILayout.EndHorizontal();
        
    }

    private static void AddCollections(long index, CollectionType type, string input)
    {
        UserData userData = Singleton<UserDataManager>.Instance.GetUserData(index);
        if (userData.IsGuest())
        {
            GameMessageManager.SendGameMessage("Guest Account\nUnable to add collections", (int)index);
            return;
        }
        try
        {
            if (int.TryParse(input, out int id))
            {
                if (userData.AddCollections((UserData.Collection)type, id))
                {
                    GameMessageManager.SendGameMessage($"Add Collections \n{type} {id}", (int)index);
                }
                else
                {
                    GameMessageManager.SendGameMessage($"Failed to add Collections \n{type} {id}", (int)index);
                }
            }
            else
            {
                GameMessageManager.SendGameMessage($"Invalid ID\n {input}", (int)index);
            }
        }
        catch (Exception e)
        {
            GameMessageManager.SendGameMessage($"Unknown error", (int)index);
            MelonLogger.Error(e);
        }
    }

    private static void UnlockMusic(long index, string input)
    {
        UserData userData = Singleton<UserDataManager>.Instance.GetUserData(index);
        if (userData.IsGuest())
        {
            GameMessageManager.SendGameMessage("Guest Account\nUnable to unlock music", (int)index);
            return;
        }
        try
        {
            if (int.TryParse(input, out int id))
            {
                if (!userData.IsUnlockMusic(UserData.MusicUnlock.Base, id))
                {
                    if (userData.AddUnlockMusic(UserData.MusicUnlock.Base, id))
                    {
                        GameMessageManager.SendGameMessage($"Unlock Music \n{id}", (int)index);
                    }
                    else
                    {
                        GameMessageManager.SendGameMessage($"Failed to unlock music \n{id}", (int)index);
                    }
                }
                else if(!userData.IsUnlockMusic(UserData.MusicUnlock.Master, id))
                {
                    userData.AddUnlockMusic(UserData.MusicUnlock.Master, id);
                    userData.AddUnlockMusic(UserData.MusicUnlock.ReMaster, id);
                    GameMessageManager.SendGameMessage($"Unlock Master \n{id}", (int)index);
                }
                else
                {
                    GameMessageManager.SendGameMessage($"Music not found or already unlocked\n{id}", (int)index);
                }
            }
            else
            {
                GameMessageManager.SendGameMessage($"Invalid ID\n {input}", (int)index);
            }
        }
        catch (Exception e)
        {
            GameMessageManager.SendGameMessage($"Unknown error", (int)index);
            MelonLogger.Error(e);
        }
    }
}