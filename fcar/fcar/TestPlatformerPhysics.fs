﻿//https://bruinbrown.wordpress.com/2013/10/06/making-a-platformer-in-f-with-monogame/
module TestPlatformerPhysics
 
open Microsoft.Xna.Framework
open TestPlatformerActor

let IsActorStatic actor =
    match actor.BodyType with
    | Static -> true
    | _ -> false

let PartitionWorldObjects worldObjects =
    worldObjects
    |> List.partition IsActorStatic

let HandleCollisions worldObjects =
    let stc, dyn = PartitionWorldObjects worldObjects
    
    let FindNewVelocity rect1 rect2 velocity =
        let inter = Rectangle.Intersect(rect1,rect2)
        let mutable (newVel:Vector2) = velocity
        if inter.Height > inter.Width 
            then do newVel.X <- 0.f
        if inter.Width > inter.Height 
            then do newVel.Y <- 0.f
        newVel

    let FindOptimumCollision a b =
        match a.ActorType,b.ActorType with
        | Player(h), Obstacle -> 
            match a.BodyType, b.BodyType with
            | Dynamic (s), Static -> 
                { 
                    a with 
                        BodyType = Dynamic((FindNewVelocity a.NextBounds b.Bounds s));
                        ActorType = Player(Nothing);
                }
            | _ -> a
        | _ -> a

    let rec FigureCollisions (actor:WorldActor) (sortedActors:WorldActor list) =
        match sortedActors with
        | [] -> actor
        | x :: xs -> 
            let a = 
                if actor.NextBounds.Intersects x.NextBounds 
                then FindOptimumCollision actor x
                else actor
            FigureCollisions a xs

    let rec FixCollisions (toFix:WorldActor list) (alreadyFixed:WorldActor list) =
        match toFix with
        | [] -> alreadyFixed
        | x :: xs -> 
            let a = FigureCollisions x alreadyFixed
            FixCollisions xs (a::alreadyFixed)

    FixCollisions dyn stc

let AddGravity (gameTime:GameTime) actor =
    let ms = gameTime.ElapsedGameTime.TotalMilliseconds
    let g = ms * 0.01
    match actor.BodyType with
    | Dynamic(s) -> 
        let d = Vector2(s.X, s.Y + (float32 g))
        { actor with BodyType = Dynamic(d); }
    | _  -> actor

let AddFriction actor = 
    match actor.BodyType with
    | Dynamic (v) -> 
        let newV = Vector2(v.X*0.95f, v.Y)
        { actor with BodyType = Dynamic(newV) }
    | _ -> actor

let ResolveVelocities actor =
    match actor.BodyType with
    | Dynamic (s) -> { actor with Pos = actor.Pos + s }
    | _ -> actor
