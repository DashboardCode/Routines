WCF project is csproj with header
<Project ToolsVersion="15.0" DefaultTargets="Build" xmlns="http://schemas.microsoft.com/developer/msbuild/2003">

When all used Standard libraries are csproj of 
<Project Sdk="Microsoft.NET.Sdk">

Therefore packages referenced in Standard libraries can't be observed by wcf project. Therefore they should be referenced in the WCF project directly.