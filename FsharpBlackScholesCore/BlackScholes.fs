module BlackScholes
    open MathNet.Numerics.Distributions

    type PutCallFlag = Put | Call
    
    let pow x n = exp(n * log(x))

    let cumulativeDistribution x = 
        // Abromowitz and Stegun approximation
        let alpha1 =  0.31938153
        let alpha2 = -0.356563782
        let alpha3 =  1.781477937
        let alpha4 = -1.821255978
        let alpha5 =  1.330274429
        let pi = 3.141592654
        let l = abs(x)
        let k = 1.0 / (1.0 + 0.2316419 * l )
        let w = (1.0-1.0/sqrt(2.0*pi) * exp(-l*l/2.0) * 
                 (alpha1 * k + alpha2 * (pow k 2.0) + alpha3 * (pow k 3.0) + alpha4 * (pow k 4.0) + alpha5 * (pow k 5.0)))
        if x < 0.0 then 
            1.0 - w 
        else 
            w
    
    let blackScholes callPutFlag stockPrice strikePrice expirationInYears interstRate volatility = 
        let d1 = (log(stockPrice / strikePrice) + (interstRate + volatility*volatility*0.5)*expirationInYears) / (volatility * sqrt(expirationInYears))
        let d2 = d1 - volatility * sqrt(expirationInYears)
    
        match callPutFlag with 
            | Put -> strikePrice*exp(-interstRate * expirationInYears)*cumulativeDistribution(-d2)-stockPrice*cumulativeDistribution(-d1)
            | Call -> stockPrice*cumulativeDistribution(d1)-strikePrice*exp(-interstRate * expirationInYears) * cumulativeDistribution(d2)
 
    let daysToYear days =
        float(days) / 365.25
        //float(days) / 257.25
    
    let monthsToYear months =
        float(months) / 12.0
    
    let normd = new Normal(0.0, 1.0)

    let delta callPutFlag stockPrice strikePrice expirationInYears interstRate volatility = 
        let d1 = (log(stockPrice/strikePrice) + (interstRate+volatility*volatility*0.5)*expirationInYears) / (volatility *sqrt(expirationInYears))
        match callPutFlag with
            | Put -> cumulativeDistribution(d1) - 1.0
            | Call -> cumulativeDistribution(d1)
    
    let gamma stockPrice strikePrice expirationInYears interstRate volatility = 
        let d1 = (log(stockPrice/strikePrice) + (interstRate + volatility*volatility*0.5)*expirationInYears) / (volatility *sqrt(expirationInYears))
        normd.Density(d1) / stockPrice * strikePrice * sqrt(expirationInYears)
    
    let vega stockPrice strikePrice expirationInYears interstRate volatility = 
        let d1 = (log(stockPrice/strikePrice) + (interstRate + volatility*volatility*0.5)*expirationInYears) / (volatility *sqrt(expirationInYears))
        stockPrice * normd.Density(d1) * sqrt(expirationInYears) 
    
    let theta callPutFlag stockPrice strikePrice expirationInYears interstRate volatility = 
        let d1 = (log(stockPrice / strikePrice) + (interstRate + volatility*volatility*0.5)*expirationInYears) / (volatility * sqrt(expirationInYears))
        let d2 = d1 - volatility * sqrt(expirationInYears)
         
        match callPutFlag with
            | Put -> - (stockPrice * normd.Density(d1)*volatility) / (2.0 * sqrt(expirationInYears)) + interstRate * strikePrice + 
                       exp(-interstRate * expirationInYears) * cumulativeDistribution(-d2)
            | Call -> - (stockPrice * normd.Density(d1)*volatility) / (2.0 * sqrt(expirationInYears)) + interstRate * strikePrice + 
                       exp(-interstRate * expirationInYears) * cumulativeDistribution(d2)
    
    let rho callPutFlag stockPrice strikePrice expirationInYears interstRate volatility = 
        let d1 = (log(stockPrice / strikePrice) + (interstRate + volatility*volatility*0.5)*expirationInYears) / (volatility * sqrt(expirationInYears))
        let d2 = d1 - volatility * sqrt(expirationInYears)
         
        match callPutFlag with 
            | Put -> - strikePrice *  expirationInYears  * exp(-interstRate*expirationInYears) * cumulativeDistribution(-d2)
            | Call -> - strikePrice *  expirationInYears  * exp(-interstRate*expirationInYears) * cumulativeDistribution(d2)
    
    let stddev (values: seq<float>) = 
        values 
        |> Seq.fold (fun acc x -> acc + (1.0 / float ( Seq.length values)) * (x - (Seq.average values)) ** 2.0) 0.0
        |> sqrt
    
    let calcDailyReturns (prices: seq<float>) = 
        prices 
        |> Seq.pairwise 
        |> Seq.map (fun (x,y) -> log (x /y ))
    
    let annualVolatility(someReturns:seq<float> option) = 
        match someReturns with
        | None -> 0.0
        | Some returns ->
            let daily = calcDailyReturns(returns)
            let sd = stddev daily
            let days = Seq.length returns
            sd * sqrt(float days)

 