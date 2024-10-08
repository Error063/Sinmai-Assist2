﻿using System;
using System.Reflection;
using HarmonyLib;
using Net.Packet;
using Net.Packet.Mai2;
using Net.VO;
using Net.VO.Mai2;

namespace SinmaiAssist.Cheat;

public class ResetLoginBonusRecord
{
    [HarmonyPrefix]
    [HarmonyPatch(typeof(PacketGetUserData), "SafeNullMember")]
    public static bool SafeNullMember(PacketGetUserData __instance, UserDetailResponseVO src)
    {
        if (src.userData.lastLoginDate != null)
        {
            src.userData.lastLoginDate = "2000-01-01 00:00:00";
            src.userData.lastPairLoginDate = "2000-01-01 00:00:00";
        }
        return true;
    }
}