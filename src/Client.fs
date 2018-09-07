namespace WebSharperTerminalSample

open WebSharper
open WebSharper.JavaScript
open WebSharper.JQuery
open WebSharper.UI
open WebSharper.UI.Html
open WebSharper.UI.Client
open WebSharper.UI.Templating
open WebSharper.JQuery
open WebSharper.JQueryTerminal

[<JavaScript>]
module Client =

    type FormTemplate = Templating.Template<"wwwroot/index.html">

    let intOpt =
        InterpreterOptions(
            Name = "No2",
            Prompt = "+ "
        )
    let i2 = (fun (this: Terminal) command ->
            match command with
            | "asd" -> this.Echo "kek"
            | "switch" -> this.Pop()
            | _ -> this.Echo "..."
        )


    let (|Help|_|) (command: string) =
        if command = "help" then
            Some ()
        else
            None

    let (|Clear|_|) (command: string) =
        if command = "clear" then
            Some()
        else
            None

    let (|Template|_|) (command: string) =
        if command = "template" then
            Some()
        else
            None

    let (|Switch|_|) (command: string) =
        if command = "switch" then
            Some()
        else
            None

    let (|Blank|_|) (command: string) =
        if command = "" then
            Some()
        else
            None

    let mutable numOfTemplates = 0
    let interpreter =
        FuncWithThis<Terminal, string->Unit>(fun this command ->
            match command with
            | Help -> this.Echo "Commands: help, clear, template"
            | Template ->
                numOfTemplates <- numOfTemplates + 1
                let id = "template" + string numOfTemplates
                let rvProjName = Var.Create ""
                let html =
                    FormTemplate.Form()
                        .ProjName(rvProjName)
                        .EventProp([on.keyPress (fun _ ev -> ev.StopPropagation()); on.keyDown (fun _ ev -> ev.StopPropagation()); on.keyUp (fun _ ev -> ev.StopPropagation()); on.input (fun _ ev -> ev.StopPropagation())])
                        .Download(fun _ -> JS.Alert(rvProjName.Value))
                        .Doc()
                this.EchoHtml ("<div id=\"" + id + "\"></div>")
                html
                |> Doc.RunById id
            | Clear -> this.Clear()
            | Switch -> this.Push (i2, intOpt)
            | Blank -> this.Echo ""
            | _ -> this.EchoHtml("Unknown command")
        )

    let Opt =
        Options(
            Name = "Terminal1",
            Prompt = "> ",
            Greetings = "Welcome to the Terminal Test Page! See 'help' for the list of commands.",
            OnInit = (fun (t:Terminal) -> t.Enable(); t.Echo("Hey Dood, it's workin'!"))
        )

    [<SPAEntryPoint>]
    let Main() =
        Terminal("#main", interpreter, Opt)
