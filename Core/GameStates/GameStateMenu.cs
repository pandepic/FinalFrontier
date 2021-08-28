﻿using ElementEngine;
using ElementEngine.ElementUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinalFrontier
{
    public class GameStateMenu : GameState
    {
        public UIScreen UIScreen;
        public SpriteBatch2D SpriteBatch;

        public override void Initialize()
        {
            SpriteBatch = new SpriteBatch2D();
            UIScreen = UIBuilderMenu.Build();
        }

        public override void Load()
        {
            UIScreen?.ShowEnable();
        }

        public override void Unload()
        {
            UIScreen?.HideDisable();
        }

        public override void Update(GameTimer gameTimer)
        {
            UIScreen?.Update(gameTimer);
        }

        public override void Draw(GameTimer gameTimer)
        {
            SpriteBatch.Begin(SamplerType.Linear);
            UIScreen?.Draw(SpriteBatch);
            SpriteBatch.End();
        }

    } // GameStateMenu
}