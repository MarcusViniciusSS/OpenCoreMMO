﻿using NeoServer.Loaders.Interfaces;
using NeoServer.Server.Contracts;
using System.Collections.Generic;

namespace NeoServer.Scripts
{
    public class AttachEventLoaders : IRunBeforeLoaders
    {
        private readonly IEnumerable<IStartupLoader> loaders;

        public AttachEventLoaders(IEnumerable<IStartupLoader> loaders)
        {
            this.loaders = loaders;
        }

        public void Run()
        {
            //if (loaders.SingleOrDefault(x => x is NpcLoader) is NpcLoader npcLoader)
            //{
            //}
        }
    }
}
