# FsharpBlackScholes
An implementation of the black scholes formula. 

### Steps to run  it
1. dotnet publish
2. docker build . -t blackscholes
3. docker run --rm -d -p 8081:8081 blackscholes:latest
4. Goto localhost:8081
5. fiddle arround

### Things to keep in mind
1. Still vague which volatility number to take; from the instrument or from VIX and which time period? 1 month back, 1 year?
2. No clue which interest rate to take
3. Have to figure out how to convert days to year. Should I just do days/365.25 ?  or take business days only
