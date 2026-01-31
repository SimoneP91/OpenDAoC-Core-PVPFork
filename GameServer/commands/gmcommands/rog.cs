using System;
using DOL.GS.PacketHandler;

namespace DOL.GS.Commands
{
    [Cmd("&rog",
         ePrivLevel.GM,
         "Generate Random Object Generation (ROG) items",
         "/rog generate - Generate a ROG item for your level",
         "/rog generate <level> - Generate a ROG item of specified level",
         "/rog generate <level> <utility> - Generate a ROG item with minimum utility",
         "/rog jewel <level> - Generate a ROG jewel of specified level",
         "/rog jewel <level> <utility> - Generate a ROG jewel with minimum utility")]
    public class ROGCommandHandler : AbstractCommandHandler, ICommandHandler
    {
        public void OnCommand(GameClient client, string[] args)
        {
            if (args.Length < 2)
            {
                DisplaySyntax(client);
                return;
            }

            GamePlayer player = client.Player;
            
            switch (args[1].ToLower())
            {
                case "generate":
                {
                    if (args.Length == 2)
                    {
                        // Generate ROG at player level + 3
                        AtlasROGManager.GenerateROG(player);
                        DisplayMessage(client, "ROG item generated at level " + (player.Level + 3));
                    }
                    else if (args.Length == 3)
                    {
                        // Generate ROG at specified level
                        if (byte.TryParse(args[2], out byte level))
                        {
                            AtlasROGManager.GenerateROG(player, level);
                            DisplayMessage(client, "ROG item generated at level " + level);
                        }
                        else
                        {
                            DisplayMessage(client, "Invalid level. Usage: /rog generate <level>");
                        }
                    }
                    else if (args.Length == 4)
                    {
                        // Generate ROG with minimum utility
                        if (byte.TryParse(args[2], out byte level) && byte.TryParse(args[3], out byte utility))
                        {
                            AtlasROGManager.GenerateMinimumUtilityROG(player, utility);
                            DisplayMessage(client, "ROG item generated at level " + level + " with minimum utility " + utility);
                        }
                        else
                        {
                            DisplayMessage(client, "Invalid parameters. Usage: /rog generate <level> <utility>");
                        }
                    }
                    break;
                }
                
                case "jewel":
                {
                    if (args.Length < 3)
                    {
                        DisplayMessage(client, "Usage: /rog jewel <level> [utility]");
                        return;
                    }
                    
                    if (byte.TryParse(args[2], out byte level))
                    {
                        int utility = 0;
                        if (args.Length == 4 && int.TryParse(args[3], out utility))
                        {
                            AtlasROGManager.GenerateJewel(player, level, utility);
                            DisplayMessage(client, "ROG jewel generated at level " + level + " with minimum utility " + utility);
                        }
                        else
                        {
                            AtlasROGManager.GenerateJewel(player, level);
                            DisplayMessage(client, "ROG jewel generated at level " + level);
                        }
                    }
                    else
                    {
                        DisplayMessage(client, "Invalid level. Usage: /rog jewel <level> [utility]");
                    }
                    break;
                }
                
                default:
                {
                    DisplaySyntax(client);
                    break;
                }
            }
        }
    }
}
