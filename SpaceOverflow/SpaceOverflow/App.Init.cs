﻿using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
//using Nuclex.Fonts;
using StackExchange;
using System.Threading;
using SpaceOverflow.UI;
using SpaceOverflow.Effects;
using Microsoft.Xna.Framework.Audio;

namespace SpaceOverflow
{
    public partial class App
    {
        public App() {
            this.DeviceManager = new GraphicsDeviceManager(this);

            //Anti-aliasing
            this.DeviceManager.PreferMultiSampling = true;
            this.DeviceManager.PreparingDeviceSettings += new EventHandler<PreparingDeviceSettingsEventArgs>((sender, e) => {
                var pp = e.GraphicsDeviceInformation.PresentationParameters;
                int quality = 0;
                var adapter = e.GraphicsDeviceInformation.Adapter;
                var format = adapter.CurrentDisplayMode.Format;

                // Check for 2xAA
                if (adapter.CheckDeviceMultiSampleType(DeviceType.Hardware,
                   format, false, MultiSampleType.TwoSamples, out quality)) {
                    // even if a greater quality is returned, we only want quality 0
                    pp.MultiSampleQuality = 0;
                    pp.MultiSampleType = MultiSampleType.FourSamples;
                }
                return;
            });
            
            //Window resizing
            this.Window.AllowUserResizing = true;
            this.Window.ClientSizeChanged += new EventHandler((sender, e) => {
                var pp = this.GraphicsDevice.PresentationParameters;
                pp.BackBufferWidth = this.Window.ClientBounds.Width;
                pp.BackBufferHeight = this.Window.ClientBounds.Height;
                this.DeviceManager.ApplyChanges();

                this.UpdateProjection();
                
            });

            //Show mouse cursor
            this.IsMouseVisible = true;

            this.Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize() {
            this.World = Matrix.Identity;

            this.ResetView();

            this.ResetProjection();
            this.UpdateProjection();

            this.Questions = new List<QuestionInSpace>();

            StackAPI.Key = "yLHF9Tb_NEGJY_el8Gu1Bw";

            EventInput.Initialize(this.Window);

            var current = new Vector3();
            var next = new Vector3(1, 1, -1);

            var normal = new Vector3(1, 1, -1);

            var direction = (next - current) * normal;
            direction.Normalize();

            System.Diagnostics.Debug.Print("TEST: Direction: {0} {1} {2}", direction.X, direction.Y, direction.Z);

            var theta = Math.Acos(direction.Z / Math.Sqrt(Math.Pow(direction.X, 2) + Math.Pow(direction.Y, 2) + Math.Pow(direction.Z, 2)));
            var phi = Math.Atan2(direction.Y, direction.X);

            System.Diagnostics.Debug.Print("Theta: {0} Phi: {1}", MathHelper.ToDegrees((float)theta), MathHelper.ToDegrees((float)phi));

            base.Initialize();
        }

        protected void ResetView() {
            this.View = Matrix.CreateLookAt(new Vector3(0, 0, 0), new Vector3(0, 0, -1), new Vector3(0, 1, 0));
        }

        protected void ResetProjection() {
            this.FarPlane = 3400;
            this.FarFade = 1300;
            this.NearFade = 400;
            this.NearPlane = 100;
        }

        protected void UpdateProjection() {
            this.Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, (float)this.Window.ClientBounds.Width / (float)this.Window.ClientBounds.Height, 
                this.NearPlane, 10000);
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent() {
            this.SpriteBatch = new SpriteBatch(GraphicsDevice);
            //this.TextBatch = new TextBatch(GraphicsDevice);

            //this.VectorQuestionFont = this.Content.Load<VectorFont>("VectorFont");
            this.QuestionFont = this.Content.Load<SpriteFont>("QuestionFont");
            this.UIFont = this.Content.Load<SpriteFont>("UIFont");
            this.SmallUIFont = this.Content.Load<SpriteFont>("SmallUIFont");

            //Textures
            this.SpaceBackground = this.Content.Load<Texture2D>("Space");
            this.God = this.Content.Load<Texture2D>("God");
            this.ButtonBackground = this.Content.Load<Texture2D>("ButtonBackground");
            this.ButtonEdge = this.Content.Load<Texture2D>("ButtonEdge");
            this.ButtonSplit = this.Content.Load<Texture2D>("ButtonSplit");
            this.ButtonIndicator = this.Content.Load<Texture2D>("ButtonIndicator");
            this.TextBoxBackground = this.Content.Load<Texture2D>("TextBoxBackground");
            this.TextBoxEdge = this.Content.Load<Texture2D>("TextBoxEdge");
            this.TextBoxRoundedEdge = this.Content.Load<Texture2D>("TextBoxRoundedEdge");
            this.TextBoxIndicator = this.Content.Load<Texture2D>("TextBoxIndicator");
            this.DropDownBackgroundS = this.Content.Load<Texture2D>("DropDownBackgroundS");
            this.DropDownEdgeS = this.Content.Load<Texture2D>("DropDownEdgeS");
            this.DropDownBackgroundM = this.Content.Load<Texture2D>("DropDownBackgroundM");
            this.DropDownEdgeM = this.Content.Load<Texture2D>("DropDownEdgeM");
            //this.DropDownBackgroundL = this.Content.Load<Texture2D>("DropDownBackgroundL");
            //this.DropDownEdgeL = this.Content.Load<Texture2D>("DropDownEdgeL");
            this.DropDownSplit = this.Content.Load<Texture2D>("DropDownSplit");
            this.ToolBarBackground = new Texture2D(this.GraphicsDevice, 1, 1, 1, TextureUsage.Tiled, SurfaceFormat.Color);
            this.ToolBarBackground.FillSolid(new Color(83, 80, 133, 122));
            this.Caret = new Texture2D(this.GraphicsDevice, 1, 1, 1, TextureUsage.Tiled, SurfaceFormat.Color);
            this.Caret.FillSolid(new Color(84, 84, 84));
            this.Wheel = this.Content.Load<Texture2D>("Wheel");
            //this.CornerMask = this.Content.Load<Texture2D>("CornerMask");

            //Models
            //this.TieFighter = this.Content.Load<Model>("TieFighter");

            //Sounds
            this.Plop = this.Content.Load<SoundEffect>("Plop");

            this.InitializeGUI();
            this.ReloadAndPopulate();
        }
    }
}