module CarGame

open Microsoft.Xna.Framework
open Microsoft.Xna.Framework.Graphics
open Microsoft.Xna.Framework.Input

type Cargame () as x =
    inherit Game()
 
    do x.Content.RootDirectory <- "assets"
    let graphics = new GraphicsDeviceManager(x)
    let mutable spriteBatch = Unchecked.defaultof<SpriteBatch>
 
    do
        x.Window.Title <- "Test Game"
        graphics.PreferredBackBufferHeight <- 640
        graphics.PreferredBackBufferWidth <- 640

    override x.Initialize() =
        do spriteBatch <- new SpriteBatch(x.GraphicsDevice)  
        do base.Initialize()
        ()
 
    override x.LoadContent() =
        ()
 
    override x.Update (gameTime) =
        ()
 
    override x.Draw (gameTime) =
        ()