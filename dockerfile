FROM microsoft/dotnet:core  
COPY FsharpBlackScholesCore/bin/Debug/netcoreapp2.1/ ./
EXPOSE 8081
ENTRYPOINT ["dotnet", "FsharpBlackSholesCore.dll"] 