﻿using System;
using System.Collections.Generic;
using System.Text;

namespace NeoServer.Server.Model.Chat
{
    public enum ChatChannel : ushort
    {
        RuleViolations = 0x03,
        Game = 0x04,
        Trade = 0x05,
        RealLife = 0x06,
        Help = 0x08,
        Private = 0xFFFF,
        None = 0xAAAA
    }
}
