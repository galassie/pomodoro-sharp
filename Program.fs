open FsSpectre
open Spectre.Console

let focusTime =
    AnsiConsole.Prompt
    <| selectionPrompt {
        title "[lime]Focus time[/]"
        choices [| "25 minutes"; "30 minutes"; "45 minutes"; "1 hour" |]
    }

AnsiConsole.MarkupLine($"You have [lime]{focusTime}[/] of focus time!")

let breakTime =
    AnsiConsole.Prompt
    <| selectionPrompt {
        title "[aqua]Break time[/]"
        choices [| "5 minutes"; "10 minutes"; "15 minutes"; "20 minutes" |]
    }

AnsiConsole.MarkupLine($"You have [aqua]{breakTime}[/] of break time!")
