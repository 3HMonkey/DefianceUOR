/*************************************************************************
 * ModernUO                                                              *
 * Copyright 2019-2021 - ModernUO Development Team                       *
 * Email: hi@modernuo.com                                                *
 * File: ExportSpawnersCommand.cs                                        *
 *                                                                       *
 * This program is free software: you can redistribute it and/or modify  *
 * it under the terms of the GNU General Public License as published by  *
 * the Free Software Foundation, either version 3 of the License, or     *
 * (at your option) any later version.                                   *
 *                                                                       *
 * You should have received a copy of the GNU General Public License     *
 * along with this program.  If not, see <http://www.gnu.org/licenses/>. *
 *************************************************************************/

using System.Collections.Generic;
using System.IO;
using Server.Commands.Generic;
using Server.Json;
using Server.Network;

namespace Server.Engines.Spawners
{
    public class ExportSpawnersCommand : BaseCommand
    {
        public static void Initialize()
        {
            TargetCommands.Register(new ExportSpawnersCommand());
        }

        public ExportSpawnersCommand()
        {
            AccessLevel = AccessLevel.GameMaster;
            Supports = CommandSupport.AllItems & ~CommandSupport.Contained;
            Commands = new[] { "ExportSpawners" };
            ObjectTypes = ObjectTypes.Items;
            Usage = "ExportSpawners";
            Description = "Exports the given the spawners to the a file";
            ListOptimized = true;
        }

        public override void ExecuteList(CommandEventArgs e, List<object> list)
        {
            var path = e.Arguments.Length == 0 ? null : e.Arguments[0].Trim();

            if (string.IsNullOrEmpty(path))
            {
                path = Path.Combine(Core.BaseDirectory, $"Data/Spawns/{Utility.GetTimeStamp()}.json");
            }
            else
            {
                var directory = Path.GetDirectoryName(Path.GetFullPath(path!))!;
                if (!Path.IsPathRooted(path))
                {
                    path = Path.Combine(Core.BaseDirectory, path);
                    AssemblyHandler.EnsureDirectory(directory);
                }
                else if (!Directory.Exists(directory))
                {
                    LogFailure("Directory doesn't exist.");
                    return;
                }
            }

            NetState.FlushAll();

            var options = JsonConfig.GetOptions(new TextDefinitionConverterFactory());

            var spawnRecords = new List<DynamicJson>(list.Count);
            for (var i = 0; i < list.Count; i++)
            {
                if (list[i] is BaseSpawner spawner)
                {
                    var dynamicJson = DynamicJson.Create(spawner.GetType());
                    spawner.ToJson(dynamicJson, options);
                    spawnRecords.Add(dynamicJson);
                }
            }

            if (spawnRecords.Count == 0)
            {
                LogFailure("No matching spawners found.");
                return;
            }

            e.Mobile.SendMessage("Exporting spawners...");

            JsonConfig.Serialize(path, spawnRecords, options);

            e.Mobile.SendMessage($"Spawners exported to {path}");
        }
    }
}
