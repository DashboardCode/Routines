# From bower to webpack
if npm is not working from PM put C:\Program Files (x86)\Microsoft Visual Studio\2017\Community\Web\External to path variable

npm init -y // in pr.folder creates package.json
npm install npm@latest
npm install webpack --save-dev

# Update node.js (started from v8)
PM> Node -v
v22.14.0
download "recommended" https://nodejs.org/en/ install to C:\Program Files\nodejs
Check `PM> node -v` again; this folder should be just first in a path env.var. I'm not sure that VS utilize use the same node (there 
is also legacy C:\Program Files (x86)\Microsoft Visual Studio 14.0\Web\External). 
May be that VS options is responcible for: Tools>Options>Projects and Solutions>Web Package Management>External Web Tools but I left it unchanged.

# Update NPM (started from 5.8):
npm -v
11.1.0
npm install npm@latest -g  

> Get-Command npm
C:\Program Files\nodejs\npm.cmd

# Update packages
check for updates
> npm outdated

# this updates file package.json with new versions
> npx npm-check-updates -u

> npm install --save-dev package
> npm rebuild --save-dev package

be carefull with bettas, you should update them manually checking new versions with
> npm view package versions 

those warnings should be ignored 
npm WARN optional SKIPPING OPTIONAL DEPENDENCY: fsevents@1.1.3 (node_modules\fsevents):

# node updates 
Something like this problem you will meet every time you update node
"Module build failed: Error: Missing binding D:\cot\DashboardCode\Routines\AdminkaV1\Injected.AspCore.MvcApp\node_modules\node-sass\vendor\win32-x64-57\binding.node"
First try 'npm rebuild node-sass'
If doesn't help do the same manually: go to https://github.com/sass/node-sass/releases/tag/v4.8.3 (put your version there) and get the file that is pointed in message, in case of "win32-x64-57\binding.node" that is win32-x64-57_binding.node rename it to the binding.node and put to the  \node_modules\node-sass\vendor\win32-x64-57 folder

# solving npm conflicts
https://github.com/webpack/webpack/issues/8656
analyzing npm ls packageName

npm install webpack@4.28.4
npm install acorn-dynamic-import@4.0.0
npm update acorn --depth 20  - updates till the level
npm dedupe


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

TODO hot reloading 
https://markus.oberlehner.net/blog/setting-up-a-vue-project-with-webpack-4-and-babel-7/

TODO
//using AspNetCore.RouteAnalyzer; // list all routes
//but is still not migrated for Core 3.