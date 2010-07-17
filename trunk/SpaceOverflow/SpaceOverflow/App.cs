using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
//using Nuclex.Fonts;
using System.Drawing;
using SpaceOverflow.UI;
using System.Threading;
using SpaceOverflow.Effects;
using StackExchange;
using Microsoft.Xna.Framework.Audio;

namespace SpaceOverflow
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public partial class App : Game
    {
        //State
        AppState State = AppState.Ready;

        //Input
        KeyboardState LastKeyboardState;
        MouseState LastMouseState;
        QuestionInSpace ClickedQuestion;
        Vector3 PanForce;

        //2D Drawing
        SpriteBatch SpriteBatch;

        //3D Drawing
        //TextBatch TextBatch;
        Matrix World, View, Projection;
        float FarPlane, FarFade, NearFade, NearPlane;

        //General graphics
        GraphicsDeviceManager DeviceManager;

        //Font resources
        //VectorFont VectorQuestionFont;
        SpriteFont QuestionFont, UIFont, SmallUIFont;

        //Texture resources
        Texture2D ToolBarBackground, ButtonBackground, ButtonEdge, ButtonSplit, ButtonIndicator;
        Texture2D TextBoxBackground, TextBoxEdge, TextBoxRoundedEdge, TextBoxIndicator;
        Texture2D Caret, Wheel, DropDownBackgroundS, DropDownEdgeS, DropDownBackgroundM, DropDownEdgeM, DropDownSplit;
        Texture2D DropDownBackgroundL, DropDownEdgeL;
        Texture2D SpaceBackground, God;

        //3D Model resources
        //Model TieFighter;

        //Audio resources
        SoundEffect Plop;


        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            this.AbortLoading();
            this.SpriteBatch.Dispose();
            this.SpaceBackground.Dispose();
            this.ButtonBackground.Dispose();
            this.ButtonEdge.Dispose();
            this.ButtonSplit.Dispose();
            this.ButtonIndicator.Dispose();
        }
    }

    enum AppState
    {
        Ready,
        BrowserOpened
    }
}
