open System
open System.Threading.Tasks
open FsSpectre
open Spectre.Console

let focusColor = Color.Lime.ToMarkup()
let breakColor = Color.Aqua.ToMarkup()

[<RequireQualifiedAccess>]
type FocusTime =
    | ``25Mins``
    | ``30Mins``
    | ``45Mins``
    | ``1Hour``

    override this.ToString() : string =
        match this with
        | FocusTime.``25Mins`` -> "25 minutes"
        | FocusTime.``30Mins`` -> "30 minutes"
        | FocusTime.``45Mins`` -> "45 minutes"
        | FocusTime.``1Hour`` -> "1 hour"

    member this.ToSeconds() : float =
        match this with
        | FocusTime.``25Mins`` -> 1500
        | FocusTime.``30Mins`` -> 1800
        | FocusTime.``45Mins`` -> 2700
        | FocusTime.``1Hour`` -> 3600

[<RequireQualifiedAccess>]
type BreakTime =
    | ``5Mins``
    | ``10Mins``
    | ``15Mins``
    | ``20Mins``

    override this.ToString() : string =
        match this with
        | BreakTime.``5Mins`` -> "5 minutes"
        | BreakTime.``10Mins`` -> "10 minutes"
        | BreakTime.``15Mins`` -> "15 minutes"
        | BreakTime.``20Mins`` -> "20 minutes"

    member this.ToSeconds() : float =
        match this with
        | BreakTime.``5Mins`` -> 300
        | BreakTime.``10Mins`` -> 600
        | BreakTime.``15Mins`` -> 900
        | BreakTime.``20Mins`` -> 1200

let focusTime =
    AnsiConsole.Prompt
    <| selectionPrompt {
        title $"[{focusColor}]Focus time[/]"

        choices
            [| FocusTime.``25Mins``
               FocusTime.``30Mins``
               FocusTime.``45Mins``
               FocusTime.``1Hour`` |]
    }

AnsiConsole.MarkupLine($"You have [{focusColor}]{focusTime}[/] of focus time!")

let breakTime =
    AnsiConsole.Prompt
    <| selectionPrompt {
        title $"[{breakColor}]Break time[/]"

        choices
            [| BreakTime.``5Mins``
               BreakTime.``10Mins``
               BreakTime.``15Mins``
               BreakTime.``20Mins`` |]
    }

AnsiConsole.MarkupLine($"You have [{breakColor}]{breakTime}[/] of break time!\n")

let mutable pause: bool = false
let mutable skip: bool = false
let mutable quit: bool = false
let mutable cmd: option<ConsoleKey> = None

let write =
    Task.Run(fun () ->
        while not quit do
            match Console.ReadKey(true).Key with
            | ConsoleKey.P -> pause <- true
            | ConsoleKey.R -> pause <- false
            | ConsoleKey.S -> skip <- true
            | ConsoleKey.Q -> quit <- true
            | _ -> ())

 
let focusBreakCycle =
    Task.Run(
        Func<Task>(fun () ->
            task {
                while not quit do
                    skip <- false
                    pause <- false

                    do!
                        progressAsync {
                            auto_clear true

                            start (fun (ctx) ->
                                task {
                                    let description = $"[{focusColor}]Focus[/]"
                                    let task = ctx.AddTask(description, true, focusTime.ToSeconds())

                                    while (not ctx.IsFinished && not quit && not skip) do
                                        do! Task.Delay(1000)
                                        let incrementTime = if pause then 0.0 else 1.0
                                        task.Increment(incrementTime)

                                        task.Description <- if pause then "Pause" else description

                                    return ()
                                })
                        }

                    skip <- false
                    pause <- false

                    do!
                        progressAsync {
                            auto_clear true

                            start (fun (ctx) ->
                                task {
                                    let description = $"[{breakColor}]Break[/]"
                                    let task = ctx.AddTask(description, true, breakTime.ToSeconds())

                                    while (not ctx.IsFinished && not quit && not skip) do
                                        do! Task.Delay(1000)
                                        let incrementTime = if pause then 0.0 else 1.0
                                        task.Increment(incrementTime)

                                        task.Description <- if pause then "Pause" else description

                                    return ()
                                })

                        }
            })
    )

AnsiConsole.WriteLine("Running...\n")
AnsiConsole.MarkupLine("[grey]Press 'p' to pause[/]")
AnsiConsole.MarkupLine("[grey]Press 'r' to resume[/]")
AnsiConsole.MarkupLine("[grey]Press 's' to skip[/]")
AnsiConsole.MarkupLine("[grey]Press 'q' to quit[/]")

focusBreakCycle.Wait()

AnsiConsole.WriteLine("\nFinished!\n")
