﻿using System;
using HarmonyLib;
using RimWorldRealFoW.ThingComps.ThingSubComps;
using RimWorldRealFoW.Utils;
using Verse;

namespace RimWorldRealFoW.Detours
{
	// Token: 0x0200001F RID: 31
	public static class _Designator_Prefix
	{
		// Token: 0x0600007F RID: 127 RVA: 0x00008F34 File Offset: 0x00007134
		public static bool CanDesignateCell_Prefix(ref IntVec3 c, ref Designator __instance, ref AcceptanceReport __result)
		{
			Map value = Traverse.Create(__instance).Property("Map", null).GetValue<Map>();
			MapComponentSeenFog mapComponentSeenFog = value.getMapComponentSeenFog();
			bool flag = mapComponentSeenFog != null && c.InBounds(value) && !mapComponentSeenFog.knownCells[value.cellIndices.CellToIndex(c)];
			bool result;
			if (flag)
			{
				__result = false;
				result = false;
			}
			else
			{
				result = true;
			}
			return result;
		}

		// Token: 0x06000080 RID: 128 RVA: 0x00008FAC File Offset: 0x000071AC
		public static bool CanDesignateThing_Prefix(ref Thing t, ref AcceptanceReport __result)
		{
			CompHiddenable compHiddenable = t.TryGetCompHiddenable();
			bool flag = compHiddenable != null && compHiddenable.hidden;
			bool result;
			if (flag)
			{
				__result = false;
				result = false;
			}
			else
			{
				result = true;
			}
			return result;
		}
	}
}
