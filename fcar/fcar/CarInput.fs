module CarInput

open Microsoft.Xna.Framework
open Microsoft.Xna.Framework.Input
open CarActor

let maxS = 5.0f // Max speed
let incS = 0.1f // Speed inc

type keyBinding = {up:Keys; down:Keys; left:Keys; right:Keys;}
    
let HandleInput (kbs:KeyboardState) actor =
    let newSpeed (inc:float32) max  dir = if (System.Math.Abs (dir + inc)) > maxS then max else dir + inc
    let newRot (r:Vector2) deg = 
        Vector2(
            (r.X * (float32)(System.Math.Cos deg)) - (r.X * (float32)(System.Math.Sin deg)),
            (r.Y * (float32)(System.Math.Sin deg)) + (r.Y * (float32)(System.Math.Cos deg)))

    let rec HandleKeys keys (kb, speed, rot) =
        match keys with
        | []      -> speed, rot
        | x :: xs ->
            match x with
                | x when x = kb.up      -> HandleKeys xs (kb, (newSpeed  incS  maxS speed), rot)
                | x when x = kb.down    -> HandleKeys xs (kb, (newSpeed -incS -maxS speed), rot)
                | x when x = kb.left    -> HandleKeys xs (kb, speed, (newRot rot 0.1))
                | x when x = kb.right   -> HandleKeys xs (kb, speed, (newRot rot 0.1))
                | _ -> HandleKeys xs (kb, speed, rot)

    match actor.Type with
    | Player(pn,speed,rot) ->
        let kb = match pn with
                    | 1 -> {up = Keys.Up;   down = Keys.Down;   left = Keys.Left;   right = Keys.Right;}
                    | 2 -> {up = Keys.W;    down = Keys.S;      left = Keys.A;      right = Keys.D;}
                    | _ -> failwith "No keybinding for this player"
        let newSpeed, newRotation = HandleKeys (kbs.GetPressedKeys() |> Array.toList) (kb, speed, rot)
        //printfn "Player: %i vel: %f %f %f" pn newSpeed newRotation.X newRotation.Y
        { 
            actor with 
                Type = Player(pn, newSpeed, newRotation) 
        }
    | _ -> actor



