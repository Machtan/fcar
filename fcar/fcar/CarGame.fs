module CarGame

open Microsoft.Xna.Framework
open Microsoft.Xna.Framework.Graphics
open Microsoft.Xna.Framework.Input
open CarActor
open CarInput
open CarPhysics

let VectorToAngle (vector: Vector2) =
    float32 (System.Math.Atan2 (float vector.X, float -vector.Y))

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
            (1,"car.png", Player(1, 0.f, Vector2(0.f,0.f)), Vector2(32.f,32.f), Circle(16.f));
            (2, "obstacle.png", Obstacle, Vector2(128.f, 128.f), Circle(16.f));
            (3, "obstacle.png", Obstacle, Vector2(128.f, 160.f), Circle(16.f));
            (4, "obstacle.png", Obstacle, Vector2(256.f, 128.f), Circle(16.f));
            (5, "obstacle.png", Obstacle, Vector2(256.f, 256.f), Circle(16.f));
            (6,"car.png", Player(2, 0.f, Vector2(0.f,0.f)), Vector2(32.f,128.f), Circle(16.f));
        ] |> List.map CreateActor'
    )

    let DrawActor (sb:SpriteBatch) actor =
        if actor.Texture.IsSome then
            let origin =
                match actor.Geom with
                | Circle(r) -> Vector2(r, r)
                | _ -> actor.Pos
            let angle =
                match actor.Type with
                | Player(_, vel, dir) -> VectorToAngle dir
                | _ -> 0.0f
            do sb.Draw(actor.Texture.Value, actor.Pos, System.Nullable(),
                Color.White, angle, origin, 1.0f, SpriteEffects.None, 0.0f)
        ()

    override x.Initialize() =
        do spriteBatch <- new SpriteBatch(x.GraphicsDevice)
        do base.Initialize()
        ()

    override x.LoadContent() =
        ()

    override x.Update (gameTime) =
        let HandleInput' = HandleInput (Keyboard.GetState ())
        let current = WorldObjects.Value
        do WorldObjects <- lazy (
            current
            |> List.map HandleInput'
            |> List.map AddFriction
            |> Move)
        do WorldObjects.Force () |> ignore
        ()

    override x.Draw (gameTime) =
        do x.GraphicsDevice.Clear Color.CornflowerBlue
        let DrawActor' = DrawActor spriteBatch
        do spriteBatch.Begin ()
        WorldObjects.Value |> List.iter DrawActor'
        do spriteBatch.End ()
        ()