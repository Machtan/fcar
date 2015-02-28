module CarActor

open Microsoft.Xna.Framework
open Microsoft.Xna.Framework.Graphics
open Microsoft.Xna.Framework.Content

type ActorType =
    | Player of int * float32 * Vector2
    | Active of Vector2 * Vector2
    | Obstacle

type ColGeom =
    | Circle of float32
    | Rect of float32 * float32
    | Compound of ColGeom * ColGeom

type Actor =
    {
        Id      : int;
        Type    : ActorType;
        Pos     : Vector2;
        Texture : Texture2D option;
        Geom    : ColGeom;
    }

let CreateActor (content:ContentManager) (id, texture, atype, pos, geom) =
    let tex =
        if not (System.String.IsNullOrEmpty texture)
        then Some(content.Load texture)
        else None
    {
        Id = id; 
        Type = atype;
        Pos = pos;
        Texture = tex; 
        Geom = geom;
    }