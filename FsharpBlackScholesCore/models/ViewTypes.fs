module ViewTypes

type Instrument = {
        Name: string
        IexCode: string
    }
    
type BlackScholesModel = {
    Instrument:Instrument 
    CurrentPrice:float 
    StrikePrice:float 
    InterestRate:float 
    DaysValid:int
    Volatility:float

    CalculatedVolatility: float
    PutValue:float
    CallValue: float 
    Instruments: List<Instrument>
}

let noInstrumentCode = "-1"
let instrumentSet = [
        {Name= "None"; IexCode=noInstrumentCode};
        {Name= "AEX"; IexCode="12272"};
        {Name= "ING"; IexCode="11773"};
    ]