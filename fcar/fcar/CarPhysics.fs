module CarPhysics

open Microsoft.Xna.Framework
open CarActor

type Collision = {
    actor: Actor;
    mov: Vector2;
    dist: float32;
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
                    if vel = Vector2.Zero
                    then None
                    else
                        let dr = ar + cr
                        let d = check.Pos - (actor.Pos + vel)
                        let dist = d.Length()
                        let diff = dist - dr
                        if diff < 0.0f
                        then
                            Some ({ actor = actor;
                            mov = vel + ((Vector2.Normalize vel) * diff);
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
            let c =
                match get_collision actor tar with
                | Some(newc) ->
                    match col with
                    | Some(oldc) ->
                        if newc.dist < oldc.dist
                        then Some(newc)
                        else Some(oldc)
                    | None -> Some(newc)
                | None -> col
            checkcols actor rem c

    let rec move_objs (remaining: Actor list) (finished: Actor list) =
        match remaining with
        | [] -> finished
        | actor::rem ->
            let a =
                match checkcols actor finished None with
                | Some(col) ->
                    handle_collision col
                    (*match actor.Type with
                    | Player(num, _, _) ->
                        printfn "Moving player %i by (%f, %f)" num col.mov.X col.mov.Y
                    | _ -> ()*)
                    { actor with Pos = actor.Pos + col.mov; }
                | None ->
                    match actor.Type with
                    | Player(_, vel, rot) | Active(vel, rot) ->
                        { actor with Pos = actor.Pos + vel; }
                    | _ -> failwith "Attempting to move obstacle :i"
            (move_objs rem (a::finished))

    move_objs dyn stc

let AddFriction actor =
    match actor.Type with
    | Player(id, v, rot) ->
        let newV = Vector2(v.X*0.995f, v.Y)
        { actor with Type = Player(id, newV, rot) }
    | _ -> actor