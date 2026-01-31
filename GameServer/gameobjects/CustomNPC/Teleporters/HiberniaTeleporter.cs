/*
 * DAWN OF LIGHT - The first free open source DAoC server emulator
 * 
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU General Public License
 * as published by the Free Software Foundation; either version 2
 * of the License, or (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA  02111-1307, USA.
 *
 */

using System;
using DOL.Database;
using DOL.GS.PacketHandler;

namespace DOL.GS
{
	/// <summary>
	/// Hibernia teleporter.
	/// </summary>
	/// <author>Aredhel</author>
	public class HiberniaTeleporter : GameTeleporter
	{
		/// <summary>
		/// Player right-clicked the teleporter.
		/// </summary>
		/// <param name="player"></param>
		/// <returns></returns>
		public override bool Interact(GamePlayer player)
		{
			if (!base.Interact(player))
				return false;

			string message = "Greetings, " + player.Name +
			              " I am able to channel energy to transport you to distant lands. I can send you to the following locations:\n\n" +
			              "[Druim Ligen] in Connacht or \n[Druim Cain] in Bri Leith\n" +
			              "[Shannon Estuary] watchtower\n" +
			              "[Domnann] Grove in the [Shrouded Isles]\n" +
			              "[Tir na Nog] our glorious capital\n" +
			              "[Entrance] to the areas of [housing]\n\n" +
			              "Or one of the many [towns] throughout Hibernia";
		
			// In PvP servers, allow cross-realm travel
			if (GameServer.ServerRules is DOL.GS.ServerRules.PvPServerRules)
			{
				message += "\n\nI can also send you to other realms:\n[Camelot] (Albion)\n[Jordheim] (Midgard)";
			}
		
			SayTo(player, message);
			return true;
		}

		/// <summary>
		/// Player has picked a subselection.
		/// </summary>
		/// <param name="player"></param>
		/// <param name="subSelection"></param>
		protected override void OnSubSelectionPicked(GamePlayer player, DbTeleport subSelection)
		{
			switch (subSelection.TeleportID.ToLower())
			{
				case "shrouded isles":
					{
						String reply = String.Format("The isles of Hy Brasil are an excellent choice. {0} {1}",
						                             "Would you prefer the grove of [Domnann] or perhaps one of the outlying towns",
						                             "like [Droighaid], [Aalid Feie], or [Necht]?");
						SayTo(player, reply);
						return;
					}
				case "housing":
				{
					SayTo(player,
						"I can send you to your [personal] or [guild] house. If you do not have a personal house, I can teleport you to the housing [entrance] or your housing [hearth] bindstone.");
					return;
				}
				
				case "towns":
				{
					SayTo(player,
						"I can send you to:\n" +
						"[Mag Mell]\n" +
						"[Tir na mBeo]\n" +
						"[Ardagh]\n" +
						"[Howth]\n" +
						"[Connla]\n" +
						"[Innis Carthaig]");
					return;
				}
			}
			base.OnSubSelectionPicked(player, subSelection);
		}

		/// <summary>
		/// Override to allow cross-realm teleports in PvP servers
		/// </summary>
		protected override bool GetTeleportLocation(GamePlayer player, string text)
		{
			// In PvP servers, allow teleport to other realm capitals
			if (GameServer.ServerRules is DOL.GS.ServerRules.PvPServerRules)
			{
				if (text.ToLower() == "camelot")
				{
					DbTeleport teleport = WorldMgr.GetTeleportLocation(eRealm.Albion, ":Camelot");
					if (teleport != null)
					{
						OnDestinationPicked(player, teleport);
						return true;
					}
				}
				else if (text.ToLower() == "jordheim")
				{
					DbTeleport teleport = WorldMgr.GetTeleportLocation(eRealm.Midgard, ":Jordheim");
					if (teleport != null)
					{
						OnDestinationPicked(player, teleport);
						return true;
					}
				}
			}
		
			return base.GetTeleportLocation(player, text);
		}

		/// <summary>
		/// Player has picked a destination.
		/// </summary>
		/// <param name="player"></param>
		/// <param name="destination"></param>
		protected override void OnDestinationPicked(GamePlayer player, DbTeleport destination)
		{
			
			Region region = WorldMgr.GetRegion((ushort) destination.RegionID);

			if (region == null || region.IsDisabled)
			{
				player.Out.SendMessage("This destination is not available.", eChatType.CT_System,
					eChatLoc.CL_SystemWindow);
				return;
			}
			
			Say("I'm now teleporting you to " + destination.TeleportID + ".");
			OnTeleportSpell(player, destination);
		}
	}
}