open BlackScholes
open StockMarketData
open Suave
open Suave.Filters
open Suave.Operators
open Suave.DotLiquid
open ViewTypes
open System
open System.IO
open System.Threading 
open System.Net
    
 
let defaultBsModel = {Instrument = instrumentSet.[0]; CurrentPrice = 551.40; StrikePrice = 550.0; DaysValid=35; InterestRate=0.001; Volatility=0.0125; CalculatedVolatility = annualVolatility (loadPastRatesfromIEX "-1" DateTime.Now) ;PutValue=0.0; CallValue=0.0;  Instruments      = instrumentSet}
 
let calculate(model:BlackScholesModel) = 
          let vol = if (model.Instrument = instrumentSet.[0]) then model.Volatility else model.CalculatedVolatility        
          {
            model with 
                CallValue = blackScholes Call model.CurrentPrice model.StrikePrice (daysToYear model.DaysValid) model.InterestRate  vol
                PutValue = blackScholes Put model.CurrentPrice model.StrikePrice (daysToYear model.DaysValid) model.InterestRate vol 
          }
let ToModel(request:HttpRequest) = 
    let get key =
        match request.formData key with
        | Choice1Of2 x  -> x
        | _             -> ""
    let fromYear = DateTime.Now.AddDays(-10.0) 
    let pastRates = loadPastRatesfromIEX (get "Instrument") fromYear 
    let theInstrument = List.find(fun x -> x.IexCode = get "Instrument") instrumentSet
    let toFloat(s) = s |> String.replace "," "." |> float
    {
        Instrument      = theInstrument
        CurrentPrice    = toFloat (get "CurrentPrice")
        StrikePrice     = toFloat (get "StrikePrice")
        InterestRate    = toFloat (get "InterestRate")
        DaysValid       = int (get "DaysValid")
        Volatility      = toFloat (get "Volatility")
        CalculatedVolatility = annualVolatility pastRates 
        PutValue        = 0.0
        CallValue       = 0.0
        Instruments      = instrumentSet
    }
  
let app =  
    choose 
        [ GET >=>  choose [ path  "/" >=> page "index.html" defaultBsModel]
          POST >=>  choose [ path  "/" >=> request (fun r -> page "index.html" (calculate (ToModel r)))]]

let cfg =
  { defaultConfig with
      bindings =
        [ HttpBinding.create HTTP IPAddress.Any 8081us  ]
      listenTimeout = TimeSpan.FromMilliseconds 3000. }

let emo t =
        if t % 2 = 0 then ":)" else ":("

[<EntryPoint>]
let main argv =     
    Console.OutputEncoding <- System.Text.Encoding.UTF8;
    setTemplatesDir "./templates"
    setCSharpNamingConvention() 

    let cst = new CancellationTokenSource()
    let conf = { cfg with cancellationToken = cst.Token; homeFolder = Some (Path.GetFullPath "./public") } 
    let _, server = startWebServerAsync conf app
    
    Async.Start(server, cst.Token)  

    while true do 
        printf "\b\b%s" (emo System.Environment.TickCount)
        System.Threading.Thread.Sleep(100)
    0 