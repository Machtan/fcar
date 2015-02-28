module CarInput

open Microsoft.Xna.Framework
open Microsoft.Xna.Framework.Input
open CarActor

let maxvx = 5.0f // Max x velocity
let inc = 1.0f // x velocity increment
let gravity = 4.0f // How fast you fall

let HandleInput (kbs:KeyboardState) actor =
    let rec HandleKeys keys (vel:Vector2, state) =
        match keys with
        | [] -> vel
        | x :: xs ->
            let newXY (incs:float32) max = if (System.Math.Abs vel.X) + (System.Math.Abs incs) > maxvx then max else vel.X + incs
            match x with
            | Keys.Up    -> HandleKeys xs (Vector2((newXY -inc -maxvx), vel.Y),  state)
            | Keys.Down  -> HandleKeys xs (Vector2((newXY inc maxvx), vel.Y),    state)
            | Keys.Left  -> HandleKeys xs (Vector2(vel.Y, (newXY -inc -maxvx)),  state)
            | Keys.Right -> HandleKeys xs (Vector2(vel.Y, (newXY inc maxvx)),  state)
            | _ -> HandleKeys xs (vel, state)
            | Keys.Space -> 
                match state with
                | Nothing -> 
                    HandleKeys xs (Vector2(vel.X, vel.Y - gravity), Jumping)
                | Jumping -> HandleKeys xs (vel, state)
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
