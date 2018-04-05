# From bower to webpack
if npm is not working from PM put C:\Program Files (x86)\Microsoft Visual Studio\2017\Community\Web\External to path variable

npm init -y // in pr.folder creates package.json
npm install npm@latest
npm install webpack --save-dev

# Update node.js
PM> Node -v
v7.3.0
download "recommended" https://nodejs.org/en/ install to C:\Program Files\nodejs
Check `PM> node -v` again; this folder should be just first in a path env.var. I'm not sure that VS utilize use the same node (there 
is also legacy C:\Program Files (x86)\Microsoft Visual Studio 14.0\Web\External). 
May be that VS options is responcible for: Tools>Options>Projects and Solutions>Web Package Management>External Web Tools but I left it unchanged.

# Update NPM:
npm -v
5.8.0
npm install npm@latest -g  

# Update packages
check for 
npm outdated
edit package.json and SE/pakcage.json/CM/
be carefull with bettas, you should update them manually checking new versions with
> npm view bootstrap versions 



1. ASP HTML
To calculate subresource integrity (SRI) with https://www.srihash.org/ 

2. CDN priorities
https://ajax.aspnetcdn.com
https://www.bootstrapcdn.com
https://cdnjs.com

3. Migrate bootstrap
http://upgrade-bootstrap.bootply.com/


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


# check the proj file, control folder includes,
ideally it should point only bundle located in dist
 <ItemGroup>
    <Folder Include="wwwroot\dist\" />
  </ItemGroup>

# webpack
webpack     : default values means webpack --mode development ./src/index.js --output ./dist/main.js
