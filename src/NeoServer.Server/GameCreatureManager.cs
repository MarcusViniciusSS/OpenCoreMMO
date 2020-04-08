﻿using NeoServer.Game.Contracts;
using NeoServer.Game.Contracts.Creatures;
using NeoServer.Server.Contracts.Network;
using NeoServer.Server.Model.Players;
using NeoServer.Server.Model.Players.Contracts;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace NeoServer.Server
{
    public class GameCreatureManager
    {

        private ICreatureGameInstance creatureInstances;
        private readonly Dictionary<uint, ICreature> Monsters;
        private readonly Dictionary<uint, ICreature> Npcs;

        private readonly Func<PlayerModel, IPlayer> playerFactory;
        private readonly ConcurrentDictionary<uint, IConnection> playersConnection;
        private IMap map;
        public GameCreatureManager(ICreatureGameInstance creatureInstances, IMap map, Func<PlayerModel, IPlayer> playerFactory)
        {
            this.creatureInstances = creatureInstances;
            this.map = map;
            this.playerFactory = playerFactory;
            playersConnection = new ConcurrentDictionary<uint, IConnection>();
        }

        public bool AddCreature(ICreature creature)
        {
            creatureInstances.Add(creature);
            map.AddCreature(creature);

            return true;
        }

        public ICreature GetCreature(uint id) => creatureInstances[id];

        public bool RemoveCreature(ICreature creature)
        {
            if (creature.IsRemoved)
            {
                return false;
            }

            var thing = creature as IThing;

            map.RemoveThing(ref thing, creature.Tile, 1);


            creatureInstances.TryRemove(creature.CreatureId);
            creature.Removed();

            //todo remove summons
            return true;
        }

        public IPlayer AddPlayer(PlayerModel playerRecord, IConnection connection)
        {
            var player = playerFactory(playerRecord);


            connection.SetConnectionOwner(player);

            playersConnection.TryAdd(player.CreatureId, connection);

            AddCreature(player);

            return player;
        }

        public bool RemovePlayer(IPlayer player)
        {
            IConnection connection = null;
            if (!playersConnection.TryRemove(player.CreatureId, out connection))
            {
                return false;
            }

            connection.Close();

            RemoveCreature(player);

            return true;
        }

        public IConnection GetPlayerConnection(uint playerId) => playersConnection[playerId];
    }
}
