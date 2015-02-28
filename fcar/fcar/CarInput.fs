module CarInput

open Microsoft.Xna.Framework
open Microsoft.Xna.Framework.Input
open CarActor

let maxvx = 5.0f // Max x velocity
let inc = 0.1f // x velocity increment
let gravity = 4.0f // How fast you fall

type keyBinding = {up:Keys; down:Keys; left:Keys; right:Keys;}
    
let HandleInput (kbs:KeyboardState) actor =
    let rec HandleKeys keys (pn, vel:Vector2) =
        let newXY (incs:float32) max  dir = if (System.Math.Abs (dir + incs)) > maxvx then max else dir + incs
        let kb = match pn with
                    | 1 -> {up = Keys.Up; down = Keys.Down; left = Keys.Left; right = Keys.Right;}
                    | 2 -> {up = Keys.W; down = Keys.S; left = Keys.A; right = Keys.D;}
                    | _ -> failwith "No keybinding for this player"
        match keys with
        | [] -> vel
        | x :: xs ->
            match (x:Keys) with
                | x when x = kb.up      -> HandleKeys xs (pn, Vector2(          (newXY inc maxvx vel.X), vel.Y  ))
                | x when x = kb.down    -> HandleKeys xs (pn, Vector2(          (newXY -inc -maxvx vel.X), vel.Y))
                | x when x = kb.left    -> HandleKeys xs (pn, Vector2(vel.X,    (newXY inc maxvx vel.Y)         ))
                | x when x = kb.right   -> HandleKeys xs (pn, Vector2(vel.X,    (newXY -inc -maxvx vel.Y)       ))
                | _ -> HandleKeys xs (pn, vel)
    match actor.Type with
    | Player(pn,init_vel,rot) ->
        let velocity = HandleKeys (kbs.GetPressedKeys() |> Array.toList) (pn, init_vel)
        printfn "Player: %i vel: %f %f" pn velocity.X velocity.Y
        { 
            actor with 
                Type = Player(pn,velocity,rot) 
        }
    | _ -> actor



