module CarPhysics

open Microsoft.Xna.Framework
open CarActor

let FRICTION = 0.95f
let HIT_FRICTION = 0.5f
let VEL_TRESHOLD = 0.001f

type Collision = {
    actor: Actor;
    mov: Vector2;
    dist: float32;
    target: Actor;
}

let handle_collision col =
    //printfn "Handling collision"
    let a =
        match col.actor.Type with
        | Player(n, vel, dir) ->
            let nv = vel * HIT_FRICTION
            Player(n, (if nv < VEL_TRESHOLD then 0.f else nv), dir)
        | other -> other
    (a, col.mov, col.target)

let get_collision actor check =
    match actor.Type with
        | Player(_, v, dir)->
            if v = 0.f || dir = Vector2.Zero
            then None
            else
                let bodies = (actor.Geom, check.Geom)
                let vel = dir * v
                //printfn "dir: (%f, %f) v: %f Vel: (%f, %f)" dir.X dir.Y v vel.X vel.Y
                match bodies with
                | (Circle(ar), Circle(cr)) ->
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
                    let (na, nm, nt) = handle_collision col
                    (*match actor.Type with
                    | Player(num, _, _) ->
                        printfn "Moving player %i by (%f, %f)" num col.mov.X col.mov.Y
                    | _ -> ()*)
                    { actor with Pos = actor.Pos + col.mov; }
                | None ->
                    match actor.Type with
                    | Player(_, vel, dir) ->
                        { actor with Pos = actor.Pos + dir * vel; }
                    | _ -> failwith "Attempting to move obstacle or Active :i"
            (move_objs rem (a::finished))

    move_objs dyn stc

let AddFriction actor =
    match actor.Type with
    | Player(id, v, dir) ->
        { actor with Type = Player(id, v * FRICTION, dir) }
    | _ -> actor