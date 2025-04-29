#r "nuget: FuzzySharp, 2.0.2"

open System
open FuzzySharp
open FuzzySharp.SimilarityRatio

let vimCommands =
    [
        "move to beginning of line", "^"
        "move to end of line", "$"
        "insert at beginning of line", "I"
        "insert at end of line", "A"
        "page down", "Ctrl+f"
        "page up", "Ctrl+b"
        "half page down", "Ctrl+d"
        "half page up", "Ctrl+u"
        "save file", ":w"
        "quit", ":q"
        "save and quit", ":wq"
        "delete current line", "dd"
        "undo last change", "u"
        "redo change", "Ctrl+r"
        "copy line", "yy"
        "paste below", "p"
        "paste above", "P"
        "visual mode", "v"
        "visual line mode", "V"
    ]
    |> Map.ofList

let fuzzyLookup (query: string) : string =
    let query = query.Trim().ToLower()
    let candidates =
        vimCommands
        |> Map.toList
        |> List.map fst

    let matches = Process.ExtractTop(query, candidates, null, limit = 3)

    match matches |> Seq.toList with
    | [] -> "Command not found."
    | [bestMatch] ->
        let cmd = vimCommands.[bestMatch.Value]
        $"Best match: {bestMatch.Value} → {cmd}"
    | many ->
        many
        |> List.map (fun m ->
            let cmd = vimCommands.[m.Value]
            $"• {m.Value}: {cmd}")
        |> String.concat "\n"



let rec mainLoop () =
    Console.Write("\nAsk for a Vim command (type 'exit' to quit): ")
    let input = Console.ReadLine()
    match input with
    | null | "" -> mainLoop()
    | input when input.Trim().ToLower() = "exit" ->
        printfn "Goodbye!"
    | query ->
        let result = fuzzyLookup query
        printfn $"Result:\n{result}"
        mainLoop()

printfn "Welcome to the Vim Assistant!"
mainLoop()
