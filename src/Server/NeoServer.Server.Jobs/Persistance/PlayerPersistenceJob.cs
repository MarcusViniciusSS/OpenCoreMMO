﻿using NeoServer.Data.Interfaces;
using NeoServer.Server.Contracts;
using NeoServer.Server.Standalone;
using Serilog.Core;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NeoServer.Server.Jobs.Persistance
{
    public class PlayerPersistenceJob
    {
        private readonly IGameServer gameServer;
        private readonly IAccountRepository accountRepository;
        private readonly Logger logger;
        private readonly ServerConfiguration serverConfiguration;
        private readonly Stopwatch stopwatch = new Stopwatch();

        private int saveInterval = 60000;
        public PlayerPersistenceJob(IGameServer gameServer, IAccountRepository accountRepository, Logger logger, ServerConfiguration serverConfiguration)
        {
            this.gameServer = gameServer;
            this.accountRepository = accountRepository;
            this.logger = logger;
            this.serverConfiguration = serverConfiguration;

        }
        public void Start(CancellationToken token)
        {
            saveInterval = (int)(serverConfiguration?.Save?.Players ?? (uint)saveInterval);
            Task.Run(async () =>
            {
                while (true)
                {
                    if (token.IsCancellationRequested) return;
                    try
                    {
                        await SavePlayers();
                    }
                    catch (Exception ex)
                    {
                        logger.Error("Could not save players");
                        logger.Debug(ex.Message);
                        logger.Debug(ex.StackTrace);
                    }
                    await Task.Delay(saveInterval, token);
                }
            });
        }

        public async Task SavePlayers()
        {
            var players = gameServer.CreatureManager.GetAllLoggedPlayers().ToList();

            if (players.Any())
            {
                stopwatch.Restart();

                await accountRepository.UpdatePlayers(players);

                logger.Information("{numPlayers} players saved in {elapsed} ms", players.Count, stopwatch.ElapsedMilliseconds);
            }
        }
    }
}
