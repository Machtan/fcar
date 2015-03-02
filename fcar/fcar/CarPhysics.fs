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
            match col.target.Type with
            | _ ->
                Player(n, -vel, dir) // Bounce
            (*| other -> // Stop
                let nv = vel * HIT_FRICTION
                Player(n, (if nv < VEL_TRESHOLD then 0.f else nv), dir)*)
        | other -> other
    ({ col.actor with Type = a; }, col.mov, col.target)

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
                        mov = Vector2(0.f,0.f);//vel + ((Vector2.Normalize vel) * diff);
                        dist = System.Math.Abs diff;
                        target = check;
                        })
                    else None

                    | _ -> failwith "Unhandled body types... sorry"
        | _ -> failwith "Collision check invoked for static actor"

let Move objects =
    let dec, solid = objects |> List.partition (fun a -> a.Type = Decoration)
    let stc, dyn = solid |> List.partition (fun a -> a.Type = Obstacle)

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
            let moved_actor =
                // Create the movement target
                let target =
                    match actor.Type with
                    | Player(_, vel, dir) ->
                        { actor with Pos = actor.Pos + dir * vel; }
                    | _ -> failwith "Attempting to move obstacle or Active :i"

                // Check if the target collides with anything
                match checkcols target finished (checkcols target rem None) with
                | Some(col) ->
                    // Get the handled collision parameters
                    let (new_actor, new_movement, new_target) = handle_collision col
                    { new_actor with Pos = actor.Pos + new_movement; }
                    //actor
                | None -> target
            (move_objs rem (moved_actor::finished))

    dec @ (move_objs dyn stc)

let AddFriction actor =
    match actor.Type with
    | Player(id, v, dir) ->
        { actor with Type = Player(id, v * FRICTION, dir) }
    | _ -> actor