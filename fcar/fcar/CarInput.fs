module CarInput

open Microsoft.Xna.Framework
open Microsoft.Xna.Framework.Input
open CarActor

let maxS = 5.0f // Max speed
let incS = 0.1f // Speed inc
let rotS = 0.1f // Rotation speed

type keyBinding = {up:Keys; down:Keys; left:Keys; right:Keys;}
//Finding a new speed from: a incrisment(int), a max speed(maxS) and the old speed(oldS)
let newSpeed (inc:float32) max  oldS = if (System.Math.Abs (oldS + inc)) > maxS 
                                       then max 
                                       else oldS + inc
//Finding a new rotation from: the old(r) and a diraktion(dir[1,-1])
let newRot (r:Vector2) dir = Vector2.Normalize(r * Vector2(
                                                        -r.Y * dir * rotS, 
                                                         r.X * dir * rotS))

let HandleInput (kbs:KeyboardState) actor =
    let rec HandleKeys keys (kb, speed, rot) =
        match keys with
        | []      -> speed, rot
        | x :: xs ->
            match x with
                | x when x = kb.up      -> HandleKeys xs (kb, (newSpeed  incS  maxS speed), rot)
                | x when x = kb.down    -> HandleKeys xs (kb, (newSpeed -incS -maxS speed), rot)
                | x when x = kb.left    -> HandleKeys xs (kb, speed, (newRot rot 1.f))
                | x when x = kb.right   -> HandleKeys xs (kb, speed, (newRot rot -1.f))
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



