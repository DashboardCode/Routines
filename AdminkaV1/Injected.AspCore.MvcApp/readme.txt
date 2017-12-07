1. ASP HTML
To calculate subresource integrity (SRI) with https://www.srihash.org/ 

2. CDN priorities
https://ajax.aspnetcdn.com
https://www.bootstrapcdn.com
https://cdnjs.com

3. Migrate bootstrap
http://upgrade-bootstrap.bootply.com/

4. from bower to webpack
npm init -y // in pr.folder creates package.json
npm install npm@latest
npm install webpack --save-dev

3.node.js
there are two node.js installations
C:\Program Files\nodejs
C:\Program Files (x86)\Microsoft Visual Studio 14.0\Web\External

how to configure: through %path% and VS options: Tools>Options>Projects and Solutions>Web Package Management>External Web Tools

5. tsconfig.js

6. NUGET (only from command line, not from PMC)
> nuget locals all -list

> nuget locals http-cache -clear        #Clear the 3.x+ cache
> nuget locals packages-cache -clear    #Clear the 2.x cache
> nuget locals global-packages -clear   #Clear the global packages folder
> nuget locals temp -clear              #Clear the temporary cache
> nuget locals all -clear               #Clear all caches

Top config file folder: %ProgramFiles(x86)%\NuGet\Config 
then -configFile option (more: https://docs.microsoft.com/en-us/nuget/consume-packages/configuring-nuget-behavior)

7. Core "User Secrets""
Proj file content: 
	<PropertyGroup>
	  <UserSecretsId>{GUID}</UserSecretsId>
	</PropertyGroup>
	<ItemGroup>
	  <DotNetCliToolReference Include="Microsoft.Extensions.SecretManager.Tools" Version="2.0.0" />
	</ItemGroup>

Set the variable:
	pmc> dotnet user-secrets set Authentication:Google:AppId
	or: 
	VS2017 UI> Solution Explorer > Project Context Menu > Manage User Secrets

File location (secrets.json): 
	%APPDATA%\microsoft\UserSecrets\<userSecretsId>\secrets.json

8. Material Design Icons https://material.io/icons/
   alternatives: https://tagliala.github.io/vectoriconsroundup/ , https://octicons.github.com/