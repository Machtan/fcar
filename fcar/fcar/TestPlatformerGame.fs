//https://bruinbrown.wordpress.com/2013/10/06/making-a-platformer-in-f-with-monogame/
//http://ideone.com/uWE14c
module TestPlatformerGame
 
open Microsoft.Xna.Framework
open Microsoft.Xna.Framework.Graphics
open Microsoft.Xna.Framework.Input
open TestPlatformerActor
open TestPlatformerPhysics
open TestPlatformerInput
 
type Cargame () as x =
    inherit Game()
 
    do x.Content.RootDirectory <- "assets"
    let graphics = new GraphicsDeviceManager(x)
    let mutable spriteBatch = Unchecked.defaultof<SpriteBatch>
 
    do
        x.Window.Title <- "Test Game"
        graphics.PreferredBackBufferHeight <- 200
        graphics.PreferredBackBufferWidth <- 800

    let CreateActor' = CreateActor x.Content
 
    let mutable WorldObjects = lazy (
        [
            ("player.png", Player(Nothing), Vector2(10.f,28.f), Vector2(32.f,32.f), false);
            ("obstacle.png", Obstacle, Vector2(0.f,60.f), Vector2(32.f,32.f), true);
            ("obstacle.png", Obstacle, Vector2(32.f,60.f), Vector2(32.f,32.f), true);
            ("obstacle.png", Obstacle, Vector2(64.f,60.f), Vector2(32.f,32.f), true);
            ("obstacle.png", Obstacle, Vector2(96.f,60.f), Vector2(32.f,32.f), true);
            ("obstacle.png", Obstacle, Vector2(128.f,60.f), Vector2(32.f,32.f), true);
            ("obstacle.png", Obstacle, Vector2(160.f,60.f), Vector2(32.f,32.f), true);
            ("obstacle.png", Obstacle, Vector2(192.f,60.f), Vector2(32.f,32.f), true);
            ("obstacle.png", Obstacle, Vector2(224.f,60.f), Vector2(32.f,32.f), true);
            ("obstacle.png", Obstacle, Vector2(256.f,60.f), Vector2(32.f,32.f), true);
            ("obstacle.png", Obstacle, Vector2(288.f,60.f), Vector2(32.f,32.f), true);
            ("obstacle.png", Obstacle, Vector2(320.f,60.f), Vector2(32.f,32.f), true);
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
        do WorldObjects.Force () |> ignore
        ()
 
    override x.Update (gameTime) =
        let AddGravity' = AddGravity gameTime
        let HandleInput' = HandleInput (Keyboard.GetState ())
        let current = WorldObjects.Value
        do WorldObjects <- lazy (
            current
            |> List.map HandleInput'
            |> List.map AddGravity'
            |> List.map AddFriction
            |> HandleCollisions
            |> List.map ResolveVelocities
        )
        do WorldObjects.Force () |> ignore
        ()
 
    override x.Draw (gameTime) =
        do x.GraphicsDevice.Clear Color.CornflowerBlue
        let DrawActor' = DrawActor spriteBatch
        do spriteBatch.Begin ()
        WorldObjects.Value |> List.iter DrawActor'
        do spriteBatch.End ()
        ()
