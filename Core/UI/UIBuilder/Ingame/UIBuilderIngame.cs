using Veldrid;
using ElementEngine;
using ElementEngine.ElementUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Rectangle = ElementEngine.Rectangle;

namespace FinalFrontier
{
    public static class UIBuilderIngame
    {
        public static UILabelStyle ChatMessageLabelStyle;
        public static UIContainer ChatMessageContainer;

        public static UIScreen Build(GameStatePlay play)
        {
            var screen = new UIScreen();

            BuildChat(screen);

            return screen;

        } // Build

        public static void BuildChat(UIScreen screen)
        {
            ChatMessageLabelStyle = new UILabelStyle(UITheme.BaseLabelStyle)
            {
                UIPosition = new UIPosition() { Position = new Vector2I(0) },
                Margins = new UISpacing(0, 5),
            };

            var chatContainerStyle = new UIContainerStyle(new UISpriteStatic(Globals.UIAtlas.GetUITexture("text_container.png")), fullDraggableRect: true);
            var chatContainer = new UIContainer("ChatContainer", chatContainerStyle);
            chatContainer.SetPosition(25, 300);

            var chatIconStyle = new UIImageStyle(new UISpriteStatic(Globals.UIAtlas.GetUITexture("chat_bubble_symbol.png")))
            {
                UIPosition = new UIPosition() { AnchorBottom = true, Position = new Vector2I(0) },
            };

            var chatIcon = new UIImage("ChatIcon", chatIconStyle);
            chatContainer.AddChild(chatIcon);

            var chatTextboxStyle = new UITextboxStyle(UITheme.BaseTextboxLabelStyle, new UISpriteStatic(Globals.UIAtlas.GetUITexture("type_box.png")))
            {
                UIPosition = new UIPosition() { AnchorBottom = true, AnchorRight = true },
                Padding = new UISpacing(10, 0),
            };

            var chatTextbox = new UITextbox("ChatTextbox", chatTextboxStyle, "");
            chatContainer.AddChild(chatTextbox);

            var chatContentContainerStyle = new UIContainerStyle(new UISpriteColor(RgbaByte.Clear), scrollbarV: UITheme.ContainerScrollbarVStyle)
            {
                UISize = new UISize() { Size = new Vector2I(447, 140) },
                UIPosition = new UIPosition() { Position = new Vector2I(2, 2) },
                Padding = new UISpacing(5),
                OverflowType = OverflowType.Scroll,
            };

            ChatMessageContainer = new UIContainer("ChatContentContainer", chatContentContainerStyle);
            ChatMessageContainer.IgnoreMouseClicks = true;
            chatContainer.AddChild(ChatMessageContainer);

            screen.AddChild(chatContainer);

        } // BuildChat

        public static void AddChatMessage(string message)
        {
            var scrollToBottom = ChatMessageContainer.ScrollbarV.CurrentValue == ChatMessageContainer.ScrollbarV.MaxValue;

            var messageLabel = new UILabel("", ChatMessageLabelStyle, message);
            ChatMessageContainer.AddChild(messageLabel);
            ChatMessageContainer.UpdateLayout();

            if (scrollToBottom)
                ChatMessageContainer.ScrollToBottom();
        }

    } // UIBuilderIngame
}
