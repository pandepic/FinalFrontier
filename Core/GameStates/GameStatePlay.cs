﻿using ElementEngine;
using ElementEngine.ECS;
using ElementEngine.ElementUI;
using FinalFrontier.Components;
using FinalFrontier.Networking;
using FinalFrontier.Systems;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Veldrid;

using Rectangle = ElementEngine.Rectangle;

namespace FinalFrontier
{
    public class LocalPlayer
    {
        public string Username => ClientGlobals.Username;
        public string AuthToken => ClientGlobals.AuthToken;

        public Entity Ship;
        public uint Money;
    }

    public class GameStatePlay : GameState, INetworkGameState
    {
        public GameClient Client;

        public SpriteBatch2D SpriteBatch;
        public Camera2D Camera;
        public Vector2I CameraSector;
        public LocalPlayer LocalPlayer;
        public SparseSet<Entity> EntityDrawList = new SparseSet<Entity>(1000);

        public UIScreen UIScreen;

        protected bool _dragging = false;
        protected Vector2 _dragMousePosition = Vector2.Zero;
        protected bool _cameraLocked = false;

        protected int _zoomIndex = 1;
        protected float[] _zoomLevels = new float[]
        {
            2f,
            1f,
            0.5f,
            0.25f,
            0.125f,
            0.0625f,
            0.03125f,
            0.015625f,
            0.0078125f,
            0.00390625f,
            0.001953125f,
            0.0009765625f,
            0.00048828125f,
            0.00024414062f,
            0.00012207031f,
            0.00006103515f,
            0.00003051757f,
            0.00001525878f,
            0.00000762939f,
        };

        public GameStatePlay(GameClient client)
        {
            Client = client;
        }

        public override void Initialize()
        {
            SpriteBatch = new SpriteBatch2D();
            UIScreen = new UIScreen();
        }

        public override void Load()
        {
            _dragging = false;
            _dragMousePosition = Vector2.Zero;
            _zoomIndex = 1;

            Camera = new Camera2D(new Rectangle(0, 0, ElementGlobals.TargetResolutionWidth, ElementGlobals.TargetResolutionHeight))
            {
                Zoom = _zoomLevels[_zoomIndex],
            };

            CameraSector = Vector2I.Zero;

            UIScreen?.ShowEnable();

            ClientPacketSender.JoinGame();
        }

        public override void Unload()
        {
            UIScreen?.HideDisable();
        }

        public override void Update(GameTimer gameTimer)
        {
            UIScreen.Update(gameTimer);
            HandleCamera();

            OrbitSystem.Run(Client.GalaxyGenerator, Camera, CameraSector, Client.WorldTime);
            Client.Registry.SystemsFinished();
        }

        public override void Draw(GameTimer gameTimer)
        {
            var visibleSectors = ClientUtility.GetVisibleSectors(Camera, CameraSector);

            // World space
            SpriteBatch.Begin(SamplerType.Linear, Camera.GetViewMatrix());
            EntityDrawList.Clear();
            DrawableSystem.BuildDrawList(_zoomIndex, visibleSectors, EntityDrawList, Client.GalaxyGenerator);
            DrawableSystem.RunDrawables(EntityDrawList, SpriteBatch, Camera, CameraSector);
            SpriteBatch.End();

            // Screen space
            SpriteBatch.Begin(SamplerType.Linear);
            UIScreen.Draw(SpriteBatch);
            DrawDebug();
            SpriteBatch.End();
        }

        [Conditional("DEBUG")]
        public void DrawDebug()
        {
            var debugFont = UITheme.FontRoboto.GetFont(UIFontStyle.Normal, UIFontWeight.Black);

            SpriteBatch.DrawText(debugFont, $"World Time: {Client.WorldTime:0.00}", new Vector2(25), RgbaByte.White, 20, 1);
        }

        public void HandleCamera()
        {
            if (_cameraLocked && LocalPlayer != null)
            {
                var playerTransform = LocalPlayer.Ship.GetComponent<Transform>();
                CameraSector = playerTransform.TransformedSectorPosition;
                Camera.Center(playerTransform.TransformedPosition);
                return;
            }

            if (Camera.View.Center.X < 0)
            {
                CameraSector.X -= 1;
                Camera.X += Globals.GalaxySectorScale;
            }
            else if (Camera.View.Center.X >= Globals.GalaxySectorScale)
            {
                CameraSector.X += 1;
                Camera.X -= Globals.GalaxySectorScale;
            }

            if (Camera.View.Center.Y < 0)
            {
                CameraSector.Y -= 1;
                Camera.Y += Globals.GalaxySectorScale;
            }
            else if (Camera.View.Center.Y >= Globals.GalaxySectorScale)
            {
                CameraSector.Y += 1;
                Camera.Y -= Globals.GalaxySectorScale;
            }
        } // HandleCamera

        public override void HandleGameControl(string controlName, GameControlState state, GameTimer gameTimer)
        {
            var mousePosition = InputManager.MousePosition;

            switch (controlName)
            {
                case "DragCamera":
                    {
                        if (state == GameControlState.Pressed)
                        {
                            if (!_dragging)
                            {
                                _dragging = true;
                                _cameraLocked = false;
                                _dragMousePosition = mousePosition;
                            }
                        }
                        else if (state == GameControlState.Released)
                        {
                            _dragging = false;
                        }
                    }
                    break;

                case "LockCamera":
                    {
                        _dragging = false;
                        _cameraLocked = true;
                    }
                    break;

                case "ZoomIn":
                    if (state == GameControlState.Released || state == GameControlState.WheelUp)
                    {
                        CameraZoomIn();
                    }
                    break;

                case "ZoomOut":
                    if (state == GameControlState.Released || state == GameControlState.WheelDown)
                    {
                        CameraZoomOut();
                    }
                    break;
            }
        } // HandleGameControl

        public override void HandleMouseMotion(Vector2 mousePosition, Vector2 prevMousePosition, GameTimer gameTimer)
        {
            if (_dragging)
            {
                var difference = mousePosition - _dragMousePosition;
                difference /= Camera.Zoom;
                Camera.Position -= difference;

                _dragMousePosition = mousePosition;
            }
        }

        public void CameraZoomIn()
        {
            var mouseScreenPos = InputManager.MousePosition;

            _zoomIndex -= 1;
            if (_zoomIndex < 0)
                _zoomIndex = 0;

            var oldZoom = Camera.Zoom;
            var newZoom = _zoomLevels[_zoomIndex];
            var zoomToCursor = SettingsManager.GetSetting<bool>("Gameplay", "ZoomToCursor");

            if (zoomToCursor && oldZoom < newZoom)
            {
                var width = ElementGlobals.TargetResolutionWidth;
                var height = ElementGlobals.TargetResolutionHeight;

                var diffWidth = (width / oldZoom) - (width / newZoom);
                var diffHeight = (height / oldZoom) - (height / newZoom);

                var ratioSideX = (mouseScreenPos.X - (width / 2)) / width;
                var ratioSideY = (mouseScreenPos.Y - (height / 2)) / height;

                Camera.Position += new Vector2(diffWidth * ratioSideX, diffHeight * ratioSideY);
            }

            Camera.Zoom = newZoom;

        } // CameraZoomIn

        public void CameraZoomOut()
        {
            _zoomIndex += 1;
            if (_zoomIndex >= _zoomLevels.Length)
                _zoomIndex = _zoomLevels.Length - 1;

            Camera.Zoom = _zoomLevels[_zoomIndex];

        } // CameraZoomOut

        public void OnServerDisconnected()
        {
            Client.SetGameState(GameStateType.Menu);
        }

    } // GameStatePlay
}
