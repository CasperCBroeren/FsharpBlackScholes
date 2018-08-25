FROM microsoft/dotnet:2.1-runtime
COPY FsharpBlackScholesCore/bin/Debug/netcoreapp2.1/publish/ ./
EXPOSE 8081
ENTRYPOINT ["dotnet", "FsharpBlackScholesCore.dll"]