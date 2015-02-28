module CarPhysics

open Microsoft.Xna.Framework
open CarActor

type Collision = {
    actor: Actor;
    mov: Vector2;
    dist: float;
    target: Actor;
}

let handle_collision col =
    printfn "Handling collision"

let get_collision actor check =
    match actor.Type with
        | Player(_, vel, rot) | Active(vel, rot) ->
            let bodies = (actor.Geom, check.Geom)
            match bodies with
                | (Circle(ar), Circle(cr)) ->
                    let dr = ar + cr
                    let (dx, dy) = check.Pos - (actor.Pos + vel)
                    let dist = System.Math.Sqrt (dx * dx + dy * dy)
                    let diff = dist - dr
                    if diff < 0
                    then Some ({
                        actor = actor;
                        mov = vel + (vel.Normalize() * diff);
                        dist = System.Math.Abs diff;
                        target = check;
                    })
                    else None

                | _ -> failwith "Unhandled body types... sorry"
        | _ -> failwith "Collision check invoked for static actor"

let Move objects =
    let stc, dyn = objects |> List.partition (fun a -> a.Type = Obstacle)

    let rec checkcols actor targets col =
        match targets with
        | [] -> col
        | tar::rem ->
            let c = match get_collision actor tar with
            | Some(newc) ->
                match col with
                | Some(oldc) ->
                    if newc.dist < oldc.dist
                    then Some(newc)
                    else Some(oldc)
                | None -> Some(newc)
            | None -> col
            checkcols actor rem c

    let rec move_objs remaining fixed =
        match remaining with
        | [] -> fixed
        | actor::rem ->
            let a = match checkcols actor rem None with
            | Some(col) ->
                handle_collision col
                { actor with Pos = actor.Pos + col.mov; }
            | None ->
                match actor.Type with
                | Player(_, vel, rot) | Active(vel, rot) ->
                    { actor with Pos = actor.Pos + vel}
                | _ -> failwith "Attempting to move obstacle :i"
            move_objs rem a::fixed

    move_objs dyn stc