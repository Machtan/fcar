//https://bruinbrown.wordpress.com/2013/10/06/making-a-platformer-in-f-with-monogame/
module TestPlatformerActor

open Microsoft.Xna.Framework
open Microsoft.Xna.Framework.Graphics
open Microsoft.Xna.Framework.Content
 
type BodyType =
    | Static
    | Dynamic of Vector2
 
type PlayerState =
    | Nothing
    | Jumping
 
type ActorType =
    | Player of PlayerState
    | Obstacle
 
type WorldActor =
    {
        ActorType   : ActorType;
        Pos         : Vector2;
        Size        : Vector2;
        Texture     : Texture2D option;
        BodyType    : BodyType
    }
    member this.Bounds
        with get () = Rectangle((int this.Pos.X),(int this.Pos.Y),(int this.Size.X),(int this.Size.Y))
 
    member this.NextBounds
        with get () = 
            let pos = 
                match this.BodyType with
                | Dynamic(s) -> this.Pos + s
                | _          -> this.Pos
            Rectangle((int  pos.X), (int  pos.Y), (int this.Size.X), (int this.Size.Y))

let CreateActor (content:ContentManager) (textureName, actorType, position, size, isStatic) =
    let tex = 
        if not (System.String.IsNullOrEmpty textureName) 
        then Some(content.Load textureName)
        else None
    let bt = 
        if isStatic 
        then Static
        else Dynamic(Vector2(0.f,0.f))
    {   
        ActorType = actorType; Pos = position; 
        Size = size; Texture = tex; BodyType = bt; 
    }

