module CarGame

open Microsoft.Xna.Framework
open Microsoft.Xna.Framework.Graphics
open Microsoft.Xna.Framework.Input
open CarActor
open CarInput

type Cargame () as x =
    inherit Game()
 
    do x.Content.RootDirectory <- "assets"
    let graphics = new GraphicsDeviceManager(x)
    let mutable spriteBatch = Unchecked.defaultof<SpriteBatch>
 
    do
        x.Window.Title <- "fcar Super GAME!!!"
        graphics.PreferredBackBufferHeight <- 640
        graphics.PreferredBackBufferWidth <- 640

    let CreateActor' = CreateActor x.Content

    let mutable WorldObjects = lazy (
        [
            (1,"player.png", Player(1,Vector2(0.f,0.f), Vector2(0.f,0.f)), Vector2(32.f,32.f), Circle(16.f));
        ] |> List.map CreateActor'
    )

    let DrawActor (sb:SpriteBatch) actor =
        if actor.Texture.IsSome 
            then do sb.Draw(actor.Texture.Value, actor.Pos, Color.White)
        ()

    override x.Initialize() =
        do spriteBatch <- new SpriteBatch(x.GraphicsDevice)  
        do base.Initialize()
        ()
 
    override x.LoadContent() =
        ()
 
    override x.Update (gameTime) =
        let HandleInput' = HandleInput (Keyboard.GetState ())
        ()
 
    override x.Draw (gameTime) =
        do x.GraphicsDevice.Clear Color.CornflowerBlue
        let DrawActor' = DrawActor spriteBatch
        do spriteBatch.Begin ()
        WorldObjects.Value |> List.iter DrawActor'
        do spriteBatch.End ()
        ()