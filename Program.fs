open System.Threading.Tasks
open FsSpectre
open Spectre.Console

let focusColor = Color.Lime.ToMarkup()
let breakColor = Color.Aqua.ToMarkup()

let focusTime =
    AnsiConsole.Prompt
    <| selectionPrompt {
        title $"[{focusColor}]Focus time[/]"
        choices [| "25 minutes"; "30 minutes"; "45 minutes"; "1 hour" |]
    }

AnsiConsole.MarkupLine($"You have [{focusColor}]{focusTime}[/] of focus time!")

let breakTime =
    AnsiConsole.Prompt
    <| selectionPrompt {
        title $"[{breakColor}]Break time[/]"
        choices [| "5 minutes"; "10 minutes"; "15 minutes"; "20 minutes" |]
    }

AnsiConsole.MarkupLine($"You have [{breakColor}]{breakTime}[/] of break time!")

let progress = AnsiConsole.Progress()

let result =
    progress.StartAsync(fun (ctx) ->
        task {
            let task1 = ctx.AddTask($"[{focusColor}]Focus[/]")
            let task2 = ctx.AddTask($"[{breakColor}]Break[/]")

            while (not ctx.IsFinished) do
                do! Task.Delay(250)
                task1.Increment(1.5)
                task2.Increment(0.5)

            return 0
        })

Task.WaitAll(result)

AnsiConsole.WriteLine("Finished!")
