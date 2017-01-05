﻿//   Copyright 2017 Luca De Petrillo
//
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
//
//       http://www.apache.org/licenses/LICENSE-2.0
//
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.
using RimWorld;
using Verse;

namespace RimWorldRealFoW {
	class MoteBubbleExt : MoteBubble {
		public override void Draw() {
			if (linkShown(link1)) {
				base.Draw();
			}
		}

		private bool linkShown(MoteAttachLink link) {
			if (link.Linked && link.Target != null && link.Target.Thing != null) {
				return link.Target.Thing.isVisible();
			}
			return true;
		}
	}
}
