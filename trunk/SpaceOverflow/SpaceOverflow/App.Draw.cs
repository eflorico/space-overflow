﻿using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Linq;
using System.Collections.Generic;

namespace SpaceOverflow
{
    public partial class App
    {
        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime) {
            if (this.State != AppState.BrowserOpened) {
                this.GraphicsDevice.Clear(Color.Black);

                this.SpriteBatch.Begin();
                {
                    this.SpriteBatch.Draw(this.SpaceBackground, new Rectangle(0, 0, this.GraphicsDevice.PresentationParameters.BackBufferWidth, this.GraphicsDevice.PresentationParameters.BackBufferHeight), Color.White);
                } this.SpriteBatch.End();


                //TODO: Make decision!
                if (!this.vectorRendering) {
                    this.TextBatch.ViewProjection = this.View * this.Projection;
                    this.SpriteBatch.Begin();
                    {
                        //[Far plane] .. Fade In .. [Far fade] .. Fully visible .. [Near fade] .. Fade out .. [Near pl
                        var mapOpacity = new Func<float, float>(z => {
                            float farBegin = this.FarPlane, farEnd = this.FarFade, nearEnd = this.NearFade, nearBegin = this.NearPlane;
                            var distance = -this.View.Translation.Z - z;

                            if (distance >= farBegin || distance <= nearBegin) return 0; //Out of visible range
                            else if (distance >= farEnd) return (-(distance - farEnd) + (farBegin - farEnd)) / (farBegin - farEnd); //Fade in at far plane
                            else if (distance >= nearEnd) return 1; //Fully inside visible range
                            else return (distance - nearBegin) / (nearEnd - nearBegin); //Fade in at near plane
                        });

                        lock (this.Questions) {
                            foreach (var question in this.Questions) {
                                var opacity = mapOpacity(question.Position.Z);

                                if (opacity > 0) {
                                    var projectedTopLeft = this.GraphicsDevice.Viewport.Project(question.TopLeft, this.Projection, this.View, this.World);
                                    var projectedBottomRight = this.GraphicsDevice.Viewport.Project(question.BottomRight, this.Projection, this.View, this.World);
                                    var scale = (projectedBottomRight.X - projectedTopLeft.X) / question.TextSize.X;
                                    var topLeft = new Vector2(projectedTopLeft.X, projectedTopLeft.Y);
                                    var color = new Color(Color.White, opacity);

                                    this.SpriteBatch.DrawString(this.QuestionFont, question.Question.Title, topLeft, color, 0f, new Vector2(), scale, SpriteEffects.None, 0f);
                                }
                            }
                        }

                    } this.SpriteBatch.End();
                }
                else {
                    this.TextBatch.ViewProjection = this.View * this.Projection;
                    this.TextBatch.Begin();
                    {
                        //[Far plane] .. Fade In .. [Far fade] .. Fully visible .. [Near fade] .. Fade out .. [Near pl
                        var mapOpacity = new Func<float, float>(z => {
                            float farBegin = this.FarPlane, farEnd = this.FarFade, nearEnd = this.NearFade, nearBegin = this.NearPlane;
                            var distance = -this.View.Translation.Z - z;

                            if (distance >= farBegin || distance <= nearBegin) return 0; //Out of visible range
                            else if (distance >= farEnd) return (-(distance - farEnd) + (farBegin - farEnd)) / (farBegin - farEnd); //Fade in at far plane
                            else if (distance >= nearEnd) return 1; //Fully inside visible range
                            else return (distance - nearBegin) / (nearEnd - nearBegin); //Fade in at near plane
                        });

                        lock (this.Questions)
                            foreach (var question in this.Questions) {
                                var opacity = mapOpacity(question.Position.Z);

                                if (opacity > 0) this.TextBatch.DrawText(question.Text,
                                          this.World * Matrix.CreateTranslation(question.Position),
                                          new Color(1, 1, 1, opacity));
                            }

                    } this.TextBatch.End();
                }
                //Draw god
                var vertices = new VertexPositionTexture[] { 
                    new VertexPositionTexture(new Vector3(-100,+100,-20000), new Vector2(0, 0)),
                    new VertexPositionTexture(new Vector3(+100,+100,-20000), new Vector2(1, 0)),
                    new VertexPositionTexture(new Vector3(-100,-100,-20000), new Vector2(0, 1)),
                    new VertexPositionTexture(new Vector3(+100,-100,-20000), new Vector2(1, 1))
                };

                var indices = new int[] {
                    1, 3, 0,
                    0, 2, 3
                };

                var vertexDeclaration = new VertexDeclaration(this.GraphicsDevice, VertexPositionTexture.VertexElements);

                var effect = new BasicEffect(this.GraphicsDevice, null);
                effect.World = this.World;
                effect.View = this.View;
                effect.Projection = this.Projection;
                effect.TextureEnabled = true;
                effect.Texture = this.God;

                effect.Begin();
                foreach (var pass in effect.CurrentTechnique.Passes) {
                    pass.Begin();
                    this.GraphicsDevice.VertexDeclaration = vertexDeclaration;
                    this.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, vertices, 0, 2);
                    pass.End();
                }
                effect.End();

                this.SpriteBatch.Begin(SpriteBlendMode.AlphaBlend, SpriteSortMode.Immediate, SaveStateMode.None);
                {
                    this.ToolBar.DrawTo(this.SpriteBatch);
                } this.SpriteBatch.End();

                //TIE Fighter
                {
                    GraphicsDevice.RenderState.DepthBufferEnable = true;

                    foreach (var fighter in this.TieFighters) {
                        var transforms = new Matrix[this.TieFighter.Bones.Count];
                        this.TieFighter.CopyAbsoluteBoneTransformsTo(transforms);

                        foreach (var mesh in this.TieFighter.Meshes) {
                            foreach (BasicEffect effect1 in mesh.Effects) {

                                effect1.EnableDefaultLighting();
                                effect1.World = this.World * transforms[mesh.ParentBone.Index] * Matrix.CreateScale(200f)  * Matrix.CreateTranslation(fighter.Position);
                                effect1.View = this.View;
                                effect1.Projection = this.Projection;
                            }

                            mesh.Draw();
                        }
                    }
                    
                }

#if false
            //Draw axes
            var effect = new BasicEffect(this.GraphicsDevice, null);
            effect.VertexColorEnabled = true;
            effect.World = this.World;
            effect.View = this.View;
            effect.Projection = this.Projection;

            var vertices = new VertexPositionColor[] { 
                new VertexPositionColor(new Vector3(-100,0,0), Color.LimeGreen),
                new VertexPositionColor(new Vector3(+100,0,0), Color.DarkGreen),
                new VertexPositionColor(new Vector3(0,-100,0), Color.Pink),
                new VertexPositionColor(new Vector3(0,+100,0), Color.Purple),
                new VertexPositionColor(new Vector3(0,0,-10000), Color.CornflowerBlue),
                new VertexPositionColor(new Vector3(0,0,+100), Color.DarkBlue)
            };

            var indices = new int[]{
                0, 1,
                2, 3,
                4, 5
            };

            effect.Begin();
            foreach (var pass in effect.CurrentTechnique.Passes)
            {
                pass.Begin();
                this.GraphicsDevice.DrawUserIndexedPrimitives(PrimitiveType.LineList, vertices, 0, 6, indices, 0, 3);
                pass.End();
            }
            effect.End();
#endif
            }
            base.Draw(gameTime);
        }
    }
}