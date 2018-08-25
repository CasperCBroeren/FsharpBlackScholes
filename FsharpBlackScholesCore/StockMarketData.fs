module StockMarketData
     
    open System.Net
    open System

    let toDateTime (epoch:float) = 
        let startEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)
        let result = startEpoch.AddMilliseconds(epoch)
        result 

    let fetchUrlAsync url =        
        async {                             
            let req = WebRequest.Create(Uri(url)) :?> HttpWebRequest
            req.AutomaticDecompression <- (DecompressionMethods.Deflate ||| DecompressionMethods.GZip)
            use! resp = req.AsyncGetResponse()   
            use stream = resp.GetResponseStream() 
            use reader = new IO.StreamReader(stream) 
            let html = reader.ReadToEnd() 
            printfn "finished downloading %s" url 
            return html
            }

    let loadPastRatesfromIEX m from = 
        match m with 
            | "-1" -> None  
            | _ ->
                let rawString = fetchUrlAsync ("https://www.iex.nl/Ajax/ChartData/interday.ashx?id=" + m + "&callback=ChartData")
                                    |> Async.RunSynchronously
                
                let cleanedString = rawString.Replace("""ChartData({"HistoricalData" : [""", "").Replace("""]});""", "").Replace("""],[""", "|").Replace("""]""", "").Replace("""[""", "")
    
                let points = cleanedString.Split('|')
                                |> Seq.map (fun x -> x.Split(','))
                                |> Seq.map (fun x -> (float (x.[1]), ( toDateTime (float x.[0]))))
                                |> Seq.filter( fun x-> snd x > from )
                                |> Seq.map (fun x -> fst x)
                Some(points)

 