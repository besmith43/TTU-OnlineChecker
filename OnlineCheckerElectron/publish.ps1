electronize build /target win # runs 64 bit, but needs the publishReadyToRun or it won't compile in dotnet 5
#electronize build /target win /electron-arch arm64
#electronize build /target win-arm64 /PublishReadyToRun false /electron-arch arm64