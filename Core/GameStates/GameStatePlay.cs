using ElementEngine;
using ElementEngine.ECS;
using ElementEngine.ElementUI;
using FinalFrontier.Components;
using FinalFrontier.Networking;
using System.Collections.Generic;
using System.Diagnostics;
using System.Numerics;
using Veldrid;

using Rectangle = ElementEngine.Rectangle;

namespace FinalFrontier
{
    public class GameStatePlay : GameState, INetworkGameState
    {
        public GameClient GameClient;

        public SpriteBatch2D SpriteBatch;
        public Camera2D Camera;
        public Vector2I CameraSector;
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

        protected List<Vector2I> _visibleSectors = new List<Vector2I>();
        protected SpriteFont _defaultFont;

        public GameStatePlay(GameClient client)
        {
            GameClient = client;
        }

        public override void Initialize()
        {
            SpriteBatch = new SpriteBatch2D();
            _defaultFont = UITheme.FontRoboto.GetFont(UIFontStyle.Normal, UIFontWeight.Black);
        }

        public override void Load()
        {
            _cameraLocked = true;
            _dragging = false;
            _dragMousePosition = Vector2.Zero;
            _zoomIndex = 1;

            Camera = new Camera2D(new Rectangle(0, 0, ElementGlobals.TargetResolutionWidth, ElementGlobals.TargetResolutionHeight))
            {
                Zoom = _zoomLevels[_zoomIndex],
            };

            CameraSector = Vector2I.Zero;

            ClientPacketSender.JoinGame();

            UIScreen = UIBuilderIngame.Build(this);
            UIScreen?.ShowEnable();

            //UIBuilderIngame.AddChatMessage("Pandepic: Test 1 abc abc abc abc abc abc abc abc abc abc abc abc abc abc abc abc abc test end");
            //UIBuilderIngame.AddChatMessage("Pandepic: Test 2");
            //UIBuilderIngame.AddChatMessage("Pandepic: Test 3");
            //UIBuilderIngame.AddChatMessage("Pandepic: Test 4");
            //UIBuilderIngame.AddChatMessage("Pandepic: Test 5");
            //UIBuilderIngame.AddChatMessage("Pandepic: Test 6");
            //UIBuilderIngame.AddChatMessage("Pandepic: Test 7");
            //UIBuilderIngame.AddChatMessage("Pandepic: Test 8");
            //UIBuilderIngame.AddChatMessage("Pandepic: Test 9");
            //UIBuilderIngame.AddChatMessage("Pandepic: Test 10");
        }

        public override void Unload()
        {
            UIScreen?.HideDisable();
        }

        public override void Update(GameTimer gameTimer)
        {
            _visibleSectors.Clear();
            _visibleSectors = ClientUtility.GetVisibleSectors(Camera, CameraSector, GameClient.GalaxyGenerator);

            UIScreen.Update(gameTimer);
            HandleCamera();

            if (_zoomIndex <= Globals.MAX_ZOOM_PLANET)
                OrbitSystem.Run(GameClient.GalaxyGenerator, Camera, CameraSector, GameClient.WorldTime, _visibleSectors);

            GameClient.Registry.SystemsFinished();
        }

        public override void Draw(GameTimer gameTimer)
        {
            // World space
            SpriteBatch.Begin(SamplerType.Linear, Camera.GetViewMatrix());
            EntityDrawList.Clear();
            DrawableSystem.BuildDrawList(_zoomIndex, _visibleSectors, EntityDrawList, GameClient.DrawableGroup, GameClient.GalaxyGenerator);
            DrawableSystem.RunDrawables(EntityDrawList, SpriteBatch, Camera, CameraSector, _zoomIndex);
            SpriteBatch.End();

            // Screen space
            SpriteBatch.Begin(SamplerType.Linear);
            DrawableSystem.RunWorldIcons(SpriteBatch, Camera, CameraSector);
            DrawableSystem.RunWorldSpaceLabels(SpriteBatch, Camera, CameraSector, _defaultFont);
            UIScreen.Draw(SpriteBatch);
            //DrawDebug();
            DrawInfo();
            SpriteBatch.End();
        }

        public void DrawInfo()
        {
            if (!ClientGlobals.PlayerShip.IsAlive)
                return;

            ref var playerShip = ref ClientGlobals.PlayerShip.GetComponent<PlayerShip>();
            var expToNext = "";

            if (playerShip.Rank != RankType.Admiral)
                expToNext = $"/{PlayerShip.RankExpRequirements[(RankType)((int)playerShip.Rank + 1)]}";

            SpriteBatch.DrawText(_defaultFont, $"Money: {playerShip.Money}\nExp: {playerShip.Exp}{expToNext}\nRank: {playerShip.Rank}", new Vector2(25), RgbaByte.White, 20, 1);
        }

        [Conditional("DEBUG")]
        public void DrawDebug()
        {
            var debugText = $"World Time: {GameClient.WorldTime:0.00}\nCamera Sector: {CameraSector}\nCamera Position: {Camera.Position}";

            if (ClientGlobals.PlayerShip.IsAlive)
            {
                ref var playerShip = ref ClientGlobals.PlayerShip.GetComponent<PlayerShip>();
                var expToNext = "";

                if (playerShip.Rank != RankType.Admiral)
                    expToNext = $"/{PlayerShip.RankExpRequirements[(RankType)((int)playerShip.Rank + 1)]}";

                debugText += $"\nMoney: {playerShip.Money}\nExp: {playerShip.Exp}{expToNext}\nRank: {playerShip.Rank}";
            }

            SpriteBatch.DrawText(_defaultFont, debugText, new Vector2(25), RgbaByte.White, 20, 1);
        }

        public void HandleCamera()
        {
            if (_cameraLocked
                && ClientGlobals.PlayerShip.IsAlive
                && ClientGlobals.PlayerShip.HasComponent<Transform>())
            {
                var playerTransform = ClientGlobals.PlayerShip.GetComponent<Transform>();
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
            var mouseWorldPosition = Camera.ScreenToWorld(mousePosition);

            var textbox = UIScreen.FindChildByName<UITextbox>("ChatTextbox", true);

            switch (controlName)
            {
                case "BuyShip":
                    {
                        if (textbox != null && textbox.IsFocused)
                            return;

                        var buyShipContainer = UIScreen.FindChildByName<UIContainer>("BuyShipContainer", true);

                        if (buyShipContainer != null)
                        {
                            UIBuilderIngame.UpdateBuyShip();

                            if (!buyShipContainer.IsVisible)
                                buyShipContainer.ShowEnable();
                            else
                                buyShipContainer.HideDisable();
                        }
                    }
                    break;

                case "SendChatMessage":
                    if (state == GameControlState.Released)
                    {
                        var chatTextbox = UIScreen.FindChildByName<UITextbox>("ChatTextbox", true);

                        if (chatTextbox.IsFocused && !string.IsNullOrWhiteSpace(chatTextbox.Text))
                        {
                            ClientPacketSender.ChatMessage(chatTextbox.Text);
                            chatTextbox.Text = "";
                        }
                    }
                    break;

                case "OpenInventory":
                    {
                        if (textbox != null && textbox.IsFocused)
                            return;

                        var inventoryContainer = UIScreen.FindChildByName<UIContainer>("InventoryContainer", true);

                        if (inventoryContainer != null)
                        {
                            if (!inventoryContainer.IsVisible)
                                inventoryContainer.ShowEnable();
                            else
                                inventoryContainer.HideDisable();
                        }
                    }
                    break;

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

                case "Command":
                    if (state == GameControlState.Released)
                    {
                        ClientPacketSender.PlayerMoveToPosition(mouseWorldPosition, CameraSector);
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
            GameClient.SetGameState(GameStateType.Menu);
        }

    } // GameStatePlay
}
