module CarInput

open Microsoft.Xna.Framework
open Microsoft.Xna.Framework.Input
open CarActor

let maxvx = 5.0f // Max x velocity
let inc = 0.1f // x velocity increment

type keyBinding = {up:Keys; down:Keys; left:Keys; right:Keys;}
    
let HandleInput (kbs:KeyboardState) actor =
    let newXY (incs:float32) max  dir = if (System.Math.Abs (dir + incs)) > maxvx then max else dir + incs

    let rec HandleKeys keys (kb, vel:Vector2) =
        match keys with
        | [] -> vel
        | x :: xs ->
            match x with
                | x when x = kb.up      -> HandleKeys xs (kb, Vector2(       (newXY  inc  maxvx vel.X), vel.Y))
                | x when x = kb.down    -> HandleKeys xs (kb, Vector2(       (newXY -inc -maxvx vel.X), vel.Y))
                | x when x = kb.left    -> HandleKeys xs (kb, Vector2(vel.X, (newXY  inc  maxvx vel.Y)       ))
                | x when x = kb.right   -> HandleKeys xs (kb, Vector2(vel.X, (newXY -inc -maxvx vel.Y)       ))
                | _ -> HandleKeys xs (kb, vel)

    match actor.Type with
    | Player(pn,init_vel,rot) ->
        let kb = match pn with
                    | 1 -> {up = Keys.Up;   down = Keys.Down;   left = Keys.Left;   right = Keys.Right;}
                    | 2 -> {up = Keys.W;    down = Keys.S;      left = Keys.A;      right = Keys.D;}
                    | _ -> failwith "No keybinding for this player"
        let velocity = HandleKeys (kbs.GetPressedKeys() |> Array.toList) (kb, init_vel)
        //printfn "Player: %i vel: %f %f" pn velocity.X velocity.Y
        { 
            actor with 
                Type = Player(pn,velocity,rot) 
        }
    | _ -> actor



