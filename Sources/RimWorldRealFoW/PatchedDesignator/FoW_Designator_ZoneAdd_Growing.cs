﻿using RimWorld;
using RimWorldRealFoW.Utils;
using Verse;

namespace RimWorldRealFoW.PatchedDesignators {
	class FoW_Designator_ZoneAdd_Growing : Designator_ZoneAdd_Growing {

		public override AcceptanceReport CanDesignateCell(IntVec3 c) {
			MapComponentSeenFog mapCmq = base.Map.getMapComponentSeenFog();
			if (mapCmq != null && c.InBounds(base.Map) && !mapCmq.knownCells[base.Map.cellIndices.CellToIndex(c)]) {
				return false;
			}

			return base.CanDesignateCell(c);
		}
	}
}