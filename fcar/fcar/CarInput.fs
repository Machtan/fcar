module CarInput

open Microsoft.Xna.Framework
open Microsoft.Xna.Framework.Input
open CarActor

let maxS = 8.f // Max speed
let incS = 0.1f // Speed inc
let rotS = 0.1f // Rotation speed

type keyBinding = {up:Keys; down:Keys; left:Keys; right:Keys;}

//Finding a new speed from: a incrisment(int), a max speed(maxS) and the old speed(oldS)
let newSpeed (inc:float32) max  oldS = if (System.Math.Abs (oldS + inc)) > maxS 
                                       then max 
                                       else oldS + inc
//Finding a new rotation from: the old(r) and a diraktion(dir[1,-1])
let newRot (r:Vector2) dir = Vector2.Normalize(r + Vector2(
                                                        -r.Y * dir * rotS, 
                                                         r.X * dir * rotS))
//shit
let dirMod (speed:float) =
    if System.Math.Abs speed > float maxS / 2.0                                                                                         //abs(x) > x(B) / 2
    then 1.0 - System.Math.Pow(((float (-maxS)) / 2.0 + System.Math.Abs speed), 2.0) * 1.0 / float maxS / float maxS    //g(x) = 1 - ((-x(B)) / 2 + abs(x))² 1 / x(B) / x(B)
    else 
        let newDir = 1.0 + System.Math.Log(System.Math.Abs speed / (float  maxS / 2.0)) / (float  maxS / 3.0)                                   //f(x) = 1 + ln(abs(x) / (x(B) / 2)) / (x(B) / 2)
        if newDir < 0.0
        then 0.0
        else newDir


let HandleInput (kbs:KeyboardState) actor =
    let rec HandleKeys keys (kb, speed, rot) =
        match keys with
        | []      -> speed, rot
        | x :: xs ->
            match x with
                | x when x = kb.up      -> HandleKeys xs (kb, (newSpeed  incS  maxS speed), rot)
                | x when x = kb.down    -> HandleKeys xs (kb, (newSpeed -incS -maxS speed), rot)
                | x when x = kb.left    -> HandleKeys xs (kb, speed, (newRot rot (-1.f * float32 (dirMod (float speed)))))
                | x when x = kb.right   -> HandleKeys xs (kb, speed, (newRot rot ( 1.f * float32 (dirMod (float speed)))))
                | _ -> HandleKeys xs (kb, speed, rot)

    match actor.Type with
    | Player(pn,speed,rot) ->
        let kb = match pn with
                    | 1 -> {up = Keys.Up;   down = Keys.Down;   left = Keys.Left;   right = Keys.Right;}
                    | 2 -> {up = Keys.W;    down = Keys.S;      left = Keys.A;      right = Keys.D;}
                    | _ -> failwith "No keybinding for this player"
        let newSpeed, newRotation = HandleKeys (kbs.GetPressedKeys() |> Array.toList) (kb, speed, rot)
        if pn = 1 then
            //printfn "Player: %i vel: %f %f %f" pn newSpeed newRotation.X newRotation.Y
            printfn "0.1: %f 0.5: %f 1.0: %f 4.0: %f 8.0: %f" (dirMod -0.1) (dirMod -0.5) (dirMod -1.0) (dirMod -4.0) (dirMod -8.0)
        { 
            actor with 
                Type = Player(pn, newSpeed, newRotation) 
        }
    | _ -> actor



