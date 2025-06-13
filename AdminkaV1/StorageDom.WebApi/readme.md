To start project from Visual Studio:

1. Set the environment variable ENV_DEVDEBUG=DEVDEBUG in system PATH (restart VS)
2. Set the build property UseSqlLite to true (find the line <UseSqlLite>false</UseSqlLite>) in ConnectionsStorageWebApi.proj(optional)

These steps should not change the production compilation (`Release` for C#, `npm run build` for building react)