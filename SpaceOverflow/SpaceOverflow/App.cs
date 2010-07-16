using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Nuclex.Fonts;
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
        TextBatch TextBatch;
        Matrix World, View, Projection;
        float FarPlane, FarFade, NearFade, NearPlane;

        //General graphics
        GraphicsDeviceManager DeviceManager;

        //Font resources
        VectorFont VectorQuestionFont;
        SpriteFont QuestionFont, UIFont, SmallUIFont;

        //Texture resources
        Texture2D ToolBarBackground, ButtonBackground, ButtonEdge, ButtonSplit, ButtonIndicator;
        Texture2D TextBoxBackground, TextBoxEdge, TextBoxRoundedEdge, TextBoxIndicator;
        Texture2D Caret, Wheel, DropDownBackground, DropDownEdge, DropDownSplit;
        Texture2D SpaceBackground, God;

        Model TieFighter;
        List<TieFighter> TieFighters = new List<TieFighter>();
        SoundEffect Plop;
        Queue<QuestionChange> PendingChanges = new Queue<QuestionChange>();
        DateTime LastPoll, LastChange;
        TimeSpan ChangeInterval;
        

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            this.SpriteBatch.Dispose();

            this.TextBatch.Dispose();

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
        Imploding,
        Waiting,
        Exploding,
        BrowserOpened
    }

    enum RequestType
    {
        Questions,
        Search
    }
}
