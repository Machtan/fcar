//https://bruinbrown.wordpress.com/2013/10/06/making-a-platformer-in-f-with-monogame/
module TestPlatformerInput

open Microsoft.Xna.Framework
open Microsoft.Xna.Framework.Input
open TestPlatformerActor

let maxvx = 1.0f // Max x velocity
let xinc = 0.1f // x velocity increment
let gravity = 4.0f // How fast you fall

let HandleInput (kbs:KeyboardState) actor =
    let rec HandleKeys keys (vel:Vector2, state) =
        match keys with
        | [] -> vel
        | x :: xs -> 
            match x with
            | Keys.Left | Keys.Right -> 
                let new_vx = 
                    let r = (x = Keys.Right)
                    let dx = if r then xinc else -xinc
                    let max = if r then maxvx else -maxvx
                    let nx = vel.X + dx
                    let too_high = if r then nx > max else nx < max
                    if too_high
                    then max
                    else nx
                HandleKeys xs (Vector2(new_vx, vel.Y), state)
            | Keys.Space -> 
                match state with
                | Nothing -> 
                    HandleKeys xs (Vector2(vel.X, vel.Y - gravity), Jumping)
                | Jumping -> HandleKeys xs (vel, state)
            | _ -> HandleKeys xs (vel, state)
    match actor.ActorType with
    | Player(s) -> 
        let init_vel = 
            match actor.BodyType with
            | Dynamic(v)   -> v
            | _            -> Vector2()
        let velocity = HandleKeys (kbs.GetPressedKeys() |> Array.toList) (init_vel, s)
        { 
            actor with 
                BodyType = Dynamic(velocity); 
                ActorType = Player(Jumping) 
        }
    | _ -> actor
