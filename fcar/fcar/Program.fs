open TestPlatformerGame

#if TARGET_MAC
open MonoMac.AppKit
open MonoMac.Foundation
#endif

#if TARGET_MAC
type AppDelegate() = 
    inherit NSApplicationDelegate()
    
    override x.FinishedLaunching(notification) =
        let game = new Cargame()
        game.Run()
    
    override x.ApplicationShouldTerminateAfterLastWindowClosed(sender) =
        true
        
module main =
    [<EntryPoint>]
    let main args =
        NSApplication.Init ()
        using (new NSAutoreleasePool()) (fun n -> 
            NSApplication.SharedApplication.Delegate <- new AppDelegate()
            NSApplication.Main(args) )
        0
    
#else
[<EntryPoint>]
let main args =
    let game = new Cargame()
    game.Run()
    0
    
#endif