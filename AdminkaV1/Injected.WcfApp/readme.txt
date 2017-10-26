WCF project is csproj with header
<Project ToolsVersion="15.0" DefaultTargets="Build" xmlns="http://schemas.microsoft.com/developer/msbuild/2003">

When all used Standard libraries are csproj of 
<Project Sdk="Microsoft.NET.Sdk">

Therefore packages referenced in Standard libraries can't be observed by wcf project. 
This issue also named as "no transitive dependencies".

Therefore all NUGET packages should be referenced in the WCF project directly.

https://github.com/NuGet/Home/issues/4488

empty .svc file added for 1) simple browse in browser (folder browsinf is allowed in web.config ) 2) simple "discover in project" referencing (for clients)