There are potentially a lot of erors like:

Found conflicts between different versions of "System.Reflection.Metadata" that could not 
be resolved. There was a conflict between "System.Reflection.Metadata, Version=5.0.0.0, 
Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a" and "System.Reflection.Metadata, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a".

To solve this 
1) install the latest version of package (System.Reflection.Metadata) from NuGet. 
2) set in app.config bindingRedirect to the latest version of the package.
	if with latest version you still see the error experiment with the version from the error message