﻿using ElementEngine;
using ElementEngine.ECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinalFrontier
{
    public class GameStateLoading : GameState
    {
        public GameClient Client;

        public int IdleFrames = 0;
        
        public GameStateLoading(GameClient client)
        {
            Client = client;
        }
        
        public override void Initialize()
        {
        }

        public override void Load()
        {
            IdleFrames = 0;

            Client.Registry = new Registry();
            Client.GalaxyGenerator = new GalaxyGenerator(Client.Registry, Client.WorldSeed, false);
        }

        public override void Unload()
        {
        }

        public override void Update(GameTimer gameTimer)
        {
            IdleFrames += 1;

            if (IdleFrames == 5)
            {
                Client.GalaxyGenerator.GenerateGalaxy();
                Client.SetGameState(GameStateType.Play);
            }
        }

        public override void Draw(GameTimer gameTimer)
        {
        }

    } // GameStateLoading
}